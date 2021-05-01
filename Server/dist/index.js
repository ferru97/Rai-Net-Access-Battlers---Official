"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const utils_2 = require("./utils");
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const utils_1 = require("./utils");
const http_1 = require("http");
const Server = require("socket.io");
const httpServer = http_1.createServer();
const io = new Server(httpServer, { pingTimeout: 60000, pingInterval: 4000 });
const port = 8000;
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
    socket.terminate();
    socket.on("join_public", (uid) => {
        if (!uid.includes("ms"))
            socket.close();
        else {
            console.log(uid + " join a public match");
            let newPlayer = { socket: socket, uid: uid };
            let room = getPublicRoom(newPlayer);
            if (room.player2 != null && room.player2.uid == uid) {
                console.log("Match start!");
                room.player1.socket.emit("ready", { opponent: room.player2.socket.id, roundPlayer: 'A' });
                room.player2.socket.emit("ready", { opponent: room.player1.socket.id, roundPlayer: 'B' });
                player_opponent.set(room.player2.socket.id, room.player1.socket.id);
                player_opponent.set(room.player1.socket.id, room.player2.socket.id);
            }
        }
    });
    socket.on("create_private", (uid) => {
        if (!uid.includes("ms"))
            socket.close();
        else {
            let mid = utils_2.genMatchID();
            while (privateRooms.has(mid))
                mid = utils_2.genMatchID();
            let now = Math.floor(Date.now() / 1000);
            let player = { socket: socket, uid: uid };
            let newRoom = { player1: player, player2: null, timestamp: now };
            privateRooms.set(mid, newRoom);
            console.log("Private match creatd:", mid);
            socket.emit("private_room_created", { code: mid });
        }
    });
    socket.on("join_private", (request) => {
        let data = JSON.parse(request);
        if (!data.uid.includes("ms"))
            socket.close();
        else {
            if (privateRooms.has(data.mid)) {
                let player = { socket: socket, uid: data.uid };
                let room = privateRooms.get(data.mid);
                privateRooms.delete(data.mid);
                room.player2 = player;
                room.player1.socket.emit("ready", { opponent: room.player2.socket.id, roundPlayer: 'A' });
                room.player2.socket.emit("ready", { opponent: room.player1.socket.id, roundPlayer: 'B' });
                player_opponent.set(room.player2.socket.id, room.player1.socket.id);
                player_opponent.set(room.player1.socket.id, room.player2.socket.id);
                console.log("Private match started:", data.mid);
            }
            else {
                console.log("Match not found:", data.mid);
            }
        }
    });
    socket.on("next_round", (data) => {
        let action = JSON.parse(data);
        io.to(action.to).emit("next_round", action);
    });
    socket.on("update_place", (data) => {
        let placement = JSON.parse(data);
        io.to(placement.to).emit("update_place", placement);
    });
    socket.on("left", (uid) => {
        io.to(uid).emit("left", {});
    });
    socket.on("disconnecting", (reason) => {
        console.log("Client disconnected! ", reason);
        if (player_opponent.has(socket.id)) {
            console.log("Notify opponet of disconnection");
            let opponent = player_opponent.get(socket.id);
            io.to(opponent).emit("left", {});
            player_opponent.delete(socket.id);
            player_opponent.delete(opponent);
        }
    });
    socket.on("match_end", (request) => {
        console.log("match end");
        if (player_opponent.has(socket.id)) {
            let opponent = player_opponent.get(socket.id);
            player_opponent.delete(socket.id);
            player_opponent.delete(opponent);
        }
    });
    socket.send("hi");
});
httpServer.listen(port);
console.log("RAB server started on port ", port);
//# sourceMappingURL=index.js.map