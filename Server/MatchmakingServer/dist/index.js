"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const MongoDB_1 = require("./MongoDB");
const utils_2 = require("./utils");
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const utils_1 = require("./utils");
const http_1 = require("http");
const Server = require("socket.io");
const httpServer = http_1.createServer();
const io = new Server(httpServer, { pingTimeout: 60000, pingInterval: 4000 });
const port = process.env.MATCHMAKING_PORT || 8000;
var rooms = [];
var privateRooms = new Map();
var player_opponent = new Map();
function getPublicRoom(player) {
    let i = 0;
    while (rooms.length > 0) {
        if (rooms[i].player1.socket.connected) {
            if (rooms[i].player1.uid == player.uid) {
                return rooms[i];
            }
            else {
                let room = rooms.pop();
                room.player2 = player;
                return room;
            }
        }
        else
            rooms.pop();
        i = i + 1;
    }
    let now = Math.floor(Date.now() / 1000);
    let newRoom = { player1: player, player2: null, timestamp: now };
    rooms.push(newRoom);
    return newRoom;
}
io.on("connection", (socket) => {
    console.log("New client connected: ", utils_1.getDateTime());
    socket.join(socket.id);
    socket.on("join_public", (request) => {
        try {
            let data = JSON.parse(request);
            MongoDB_1.insertUser(data.uid);
            if (!data.uid.includes("ms"))
                socket.disconnect();
            else {
                console.log(data.uid + " join a public match");
                let newPlayer = { socket: socket, uid: data.uid, nick: data.nick };
                let room = getPublicRoom(newPlayer);
                if (room.player2 != null && room.player2.uid == data.uid) {
                    const callBack = (match_id) => {
                        console.log("Match start!");
                        room.player1.socket.emit("ready", { opponent: room.player2.socket.id, roundPlayer: 'A', opponent_nick: room.player2.nick, match_id: match_id });
                        room.player2.socket.emit("ready", { opponent: room.player1.socket.id, roundPlayer: 'B', opponent_nick: room.player1.nick, match_id: match_id });
                        player_opponent.set(room.player2.socket.id, room.player1.socket.id);
                        player_opponent.set(room.player1.socket.id, room.player2.socket.id);
                    };
                    MongoDB_1.insertMatch(room, "", callBack);
                }
            }
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 01: " + String(error));
            socket.disconnect();
        }
    });
    socket.on("create_private", (request) => {
        try {
            let data = JSON.parse(request);
            MongoDB_1.insertUser(data.uid);
            if (!data.uid.includes("ms"))
                socket.disconnect();
            else {
                let mid = utils_2.genMatchID();
                while (privateRooms.has(mid))
                    mid = utils_2.genMatchID();
                let now = Math.floor(Date.now() / 1000);
                let player = { socket: socket, uid: data.uid, nick: data.nick };
                let newRoom = { player1: player, player2: null, timestamp: now };
                privateRooms.set(mid, newRoom);
                console.log("Private match creatd:", mid);
                socket.emit("private_room_created", { code: mid });
            }
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 02: " + String(error));
            socket.disconnect();
        }
    });
    socket.on("join_private", (request) => {
        try {
            let data = JSON.parse(request);
            MongoDB_1.insertUser(data.uid);
            if (!data.uid.includes("ms"))
                socket.disconnect();
            else {
                if (privateRooms.has(data.mid)) {
                    let player = { socket: socket, uid: data.uid, nick: data.nick };
                    let room = privateRooms.get(data.mid);
                    privateRooms.delete(data.mid);
                    const callback = (match_id) => {
                        room.player2 = player;
                        room.player1.socket.emit("ready", { opponent: room.player2.socket.id, roundPlayer: 'A', opponent_nick: room.player2.nick, match_id: match_id });
                        room.player2.socket.emit("ready", { opponent: room.player1.socket.id, roundPlayer: 'B', opponent_nick: room.player1.nick, match_id: match_id });
                        player_opponent.set(room.player2.socket.id, room.player1.socket.id);
                        player_opponent.set(room.player1.socket.id, room.player2.socket.id);
                        console.log("Private match started:", data.mid);
                    };
                    MongoDB_1.insertMatch(room, data.mid, callback);
                }
                else {
                    console.log("Match not found:", data.mid);
                    socket.emit("NF", {});
                }
            }
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 03: " + String(error));
            socket.disconnect();
        }
    });
    socket.on("next_round", (data) => {
        try {
            let action = JSON.parse(data);
            io.to(action.to).emit("next_round", action);
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 04: " + String(error));
            socket.disconnect();
        }
    });
    socket.on("update_place", (data) => {
        try {
            let placement = JSON.parse(data);
            io.to(placement.to).emit("update_place", placement);
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 05: " + String(error));
            socket.disconnect();
        }
    });
    socket.on("left", (uid) => {
        try {
            io.to(uid).emit("left", {});
            socket.disconnect();
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 06: " + String(error));
            socket.disconnect();
        }
    });
    socket.on("disconnecting", (reason) => {
        try {
            console.log("Client disconnected! ", reason);
            if (player_opponent.has(socket.id)) {
                console.log("Notify opponet of disconnection");
                let opponent = player_opponent.get(socket.id);
                io.to(opponent).emit("left", {});
                player_opponent.delete(socket.id);
                player_opponent.delete(opponent);
            }
            rooms = rooms.filter(r => r.player1.socket.id != socket.id); //optimize
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 07: " + String(error));
        }
    });
    socket.on("match_end", (request) => {
        console.log("match end");
        try {
            let info = JSON.parse(request);
            if (player_opponent.has(socket.id)) {
                let opponent = player_opponent.get(socket.id);
                player_opponent.delete(socket.id);
                player_opponent.delete(opponent);
            }
            MongoDB_1.endMatch(info.uid, info.status, info.match_id);
        }
        catch (error) {
            utils_2.logError("\n\nMatchmaking Error 08: " + String(error));
            socket.disconnect();
        }
    });
    socket.send("Hi mad scientist!");
});
httpServer.listen(port);
console.log("RAB server started on port ", port);
//# sourceMappingURL=index.js.map