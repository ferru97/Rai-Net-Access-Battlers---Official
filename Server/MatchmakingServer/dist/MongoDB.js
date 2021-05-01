"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.endMatch = exports.insertUser = exports.insertMatch = void 0;
const utils_1 = require("./utils");
var MongoClient = require('mongodb').MongoClient;
const { MONGO_USERNAME, MONGO_PASSWORD, MONGO_HOSTNAME, MONGO_PORT, MONGO_DB } = process.env;
var url = `mongodb://${MONGO_USERNAME}:${MONGO_PASSWORD}@${MONGO_HOSTNAME}:${MONGO_PORT}/?authSource=admin`;
var db;
const USERS_COLLECTION = "users";
const MATCHES_COLLECTION = "matches";
setTimeout(() => {
    MongoClient.connect(url, { reconnectTries: 60, reconnectInterval: 1000 }, function (err, database) {
        if (err) {
            utils_1.logError("\n\nMongo Error 01: " + String(err));
            return;
        }
        console.log("MongoDB connected!");
        db = database.db(MONGO_DB);
    });
}, 10000);
function insertUser(uid) {
    return __awaiter(this, void 0, void 0, function* () {
        const query = { uid: uid };
        db.collection(USERS_COLLECTION).findOne(query, function (err, result) {
            if (err) {
                utils_1.logError("\n\nMongo Error 02: " + String(err));
                return;
            }
            if (result == null) {
                const date = Date.now();
                const new_user = { uid: uid, time: date, username: "" };
                db.collection(USERS_COLLECTION).insertOne(new_user, function (err, res) {
                    if (err) {
                        console.log("Mongo Error 03:", err);
                        return;
                    }
                    console.log("New user registered!");
                });
            }
        });
    });
}
exports.insertUser = insertUser;
function insertMatch(room, private_code, callBack) {
    return __awaiter(this, void 0, void 0, function* () {
        const date = Date.now();
        const new_match = { player1_uid: room.player1.uid,
            player2_uid: room.player2.uid,
            creation_time: room.timestamp,
            start_time: date,
            private_code: private_code,
            winner: "" };
        db.collection(MATCHES_COLLECTION).insertOne(new_match, function (err, res) {
            if (err) {
                utils_1.logError("\n\nMongo Error 03: " + String(err));
                return;
            }
            callBack(res.insertedId);
        });
    });
}
exports.insertMatch = insertMatch;
function endMatch(uid, status, match_id) {
    return __awaiter(this, void 0, void 0, function* () {
        if (status == "win") {
            const query = { _id: match_id };
            const operation = { $set: { winner: uid } };
            db.collection(MATCHES_COLLECTION).updateOne(query, operation, function (err, res) {
                if (err) {
                    utils_1.logError("\n\nMongo Error 04: " + String(err));
                    return;
                }
            });
        }
    });
}
exports.endMatch = endMatch;
//# sourceMappingURL=MongoDB.js.map