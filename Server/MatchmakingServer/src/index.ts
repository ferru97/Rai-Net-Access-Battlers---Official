import { Room, Action, PlacementInfo, JoinPrivateReq, Player, MatchEndInfo, ID_Nick } from './const';
import { insertMatch, insertUser,endMatch } from './MongoDB'
import { genMatchID, logError, getDateTime } from './utils'
"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const utils_1 = require("./utils");
const http_1 = require("http");
const Server = require("socket.io");
const httpServer = http_1.createServer();
const io = new Server(httpServer, { upgrade: false, transports: ['websocket'], pingTimeout: 60000, pingInterval: 4000 });
const port = process.env.MATCHMAKING_PORT || 8000
const game_version = process.env.GAME_VERSION 

var rooms: Room[] = [];
var privateRooms: Map<string,Room> = new Map<string,Room>();
var player_opponent: Map<string,string> = new Map<string,string>();

function getPublicRoom(player) {
    let i = 0;
    while (rooms.length > 0) {
        if (rooms[i].player1.socket.connected) {
            if (rooms[i].player1.uid == player.uid){
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
    rooms.push(newRoom)
    return newRoom;
}


io.on("connection", (socket) => {
    console.log("New client connected: ", utils_1.getDateTime());
    socket.join(socket.id)

    socket.on("join_public", (request: string) => {
        try{
            let data: ID_Nick = JSON.parse(request)
            insertUser(data.uid)
            if(!data.uid.includes("ms") || data.v!=game_version )
                socket.disconnect()
            else{
                let newPlayer: Player = { socket: socket, uid: data.uid, nick:data.nick };
                let room: Room = getPublicRoom(newPlayer);
                if (room.player2 != null && room.player2.uid == data.uid) {
                    const callBack = (match_id) => {
                        console.log("Public match started ",getDateTime())
                        room.player1.socket.emit("ready", { opponent: room.player2.socket.id, roundPlayer:'A', opponent_nick:room.player2.nick, match_id:match_id });
                        room.player2.socket.emit("ready", { opponent: room.player1.socket.id, roundPlayer:'B', opponent_nick:room.player1.nick, match_id:match_id });
                        player_opponent.set(room.player2.socket.id, room.player1.socket.id);
                        player_opponent.set(room.player1.socket.id, room.player2.socket.id);
                    } 
                    insertMatch(room, "", callBack)
                }
            }
        }catch(error){
            logError("\n\nMatchmaking Error 01: " + String(error))
            socket.disconnect()
        }
    });

    socket.on("create_private", (request: string) => {
        try{
            let data: ID_Nick = JSON.parse(request)
            insertUser(data.uid)
            if(!data.uid.includes("ms") || data.v!=game_version)
                socket.disconnect()
            else{
                let mid = genMatchID();
                while(privateRooms.has(mid))
                    mid = genMatchID();
                
                let now = Math.floor(Date.now() / 1000);
                let player: Player = { socket: socket, uid: data.uid, nick:data.nick };
                let newRoom: Room = { player1: player, player2: null, timestamp: now };
                privateRooms.set(mid, newRoom);
        
                socket.emit("private_room_created",{code:mid});
            }
        }catch(error){
            logError("\n\nMatchmaking Error 02: " + String(error))
            socket.disconnect()
        }
    });

    socket.on("join_private", (request: string) => {
        try{
            let data: JoinPrivateReq = JSON.parse(request)
            insertUser(data.uid)
            if(!data.uid.includes("ms") || data.v!=game_version)
                socket.disconnect()
            else{
                if(privateRooms.has(data.mid)){
                    let player:Player = { socket: socket, uid: data.uid, nick: data.nick };
                    let room:Room = privateRooms.get(data.mid)
                    room.player2 = player;
                    privateRooms.delete(data.mid);
                    
                    const callback = (match_id: string) => {
                        room.player1.socket.emit("ready", { opponent: room.player2.socket.id, roundPlayer:'A', opponent_nick:room.player2.nick, match_id:match_id});
                        room.player2.socket.emit("ready", { opponent: room.player1.socket.id, roundPlayer:'B', opponent_nick:room.player1.nick, match_id:match_id });
                        player_opponent.set(room.player2.socket.id,room.player1.socket.id);
                        player_opponent.set(room.player1.socket.id, room.player2.socket.id);
                        console.log("Private match started ", getDateTime())
                    }
                    insertMatch(room, data.mid,callback)
                }else{
                    socket.emit("NF",{})
                }
            }
        }catch(error){
            logError("\n\nMatchmaking Error 03: " + String(error))
            socket.disconnect()
        }
    });


    socket.on("next_round", (data: string) => {
        try{
            let action: Action = JSON.parse(data)
            io.to(action.to).emit("next_round",action)
        }catch(error){
            logError("\n\nMatchmaking Error 04: " + String(error))
            socket.disconnect()
        }
    });


    socket.on("update_place", (data: string) => {
        try{
            let placement: PlacementInfo = JSON.parse(data)
            io.to(placement.to).emit("update_place", placement)
        }catch(error){
            logError("\n\nMatchmaking Error 05: " + String(error))
            socket.disconnect()
        }
    });


    socket.on("left", (uid: string) => {
        try{
            io.to(uid).emit("left",{})
            socket.disconnect()
        }catch(error){
            logError("\n\nMatchmaking Error 06: " + String(error))
            socket.disconnect()
        }
    });


    socket.on("disconnecting", (reason) => {
        try{
            if(player_opponent.has(socket.id)){
                console.log("Notify opponet of disconnection")
                let opponent = player_opponent.get(socket.id)
                io.to(opponent).emit("left",{})
                player_opponent.delete(socket.id)
                player_opponent.delete(opponent)
            }
            rooms = rooms.filter(r => r.player1.socket.id != socket.id) //optimize
        }catch(error){
            logError("\n\nMatchmaking Error 07: " + String(error))
        }
    });

    socket.on("match_end", (request) => {
        try{
            let info: MatchEndInfo = JSON.parse(request)
            console.log("match end ",info.match_id)
            if(player_opponent.has(socket.id)){
                let opponent = player_opponent.get(socket.id)
                player_opponent.delete(socket.id)
                player_opponent.delete(opponent)
            }
            endMatch(info.uid, info.status, info.match_id)
        }catch(error){
            logError("\n\nMatchmaking Error 08: " + String(error))
            socket.disconnect()
        }
    });

    socket.send("Hi mad scientist!");
});
httpServer.listen(port);
console.log("RAB matchmaking server started on port ",port);