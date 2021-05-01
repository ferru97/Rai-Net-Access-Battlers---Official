import { Room } from './const';
import { logError, getDateTime } from './utils'
var ObjectId = require('mongodb').ObjectID;
var MongoClient = require('mongodb').MongoClient;

const {
    MONGO_USERNAME,
    MONGO_PASSWORD,
    MONGO_HOSTNAME,
    MONGO_PORT,
    MONGO_DB
  } = process.env;

var url = `mongodb://${MONGO_USERNAME}:${MONGO_PASSWORD}@${MONGO_HOSTNAME}:${MONGO_PORT}/?authSource=admin`;

var db;
const USERS_COLLECTION = "users"
const MATCHES_COLLECTION = "matches"
const INFO_COLLECTION = "info"



setTimeout(() => 
{
    MongoClient.connect(url,{ reconnectTries: 60, reconnectInterval: 1000 }, function(err, database) {
        if (err){
            logError("\n\nMongo Error 01: " + String(err))
            return
        }
        console.log("MongoDB connected!")

        db = database.db(MONGO_DB);
        db.listCollections().toArray(function(err, collInfos) {
            if(collInfos.length < 3){
                console.log("Creating database")
                db.createCollection(USERS_COLLECTION)
                db.createCollection(MATCHES_COLLECTION)
                db.createCollection(INFO_COLLECTION)
                db.collection(INFO_COLLECTION).insertOne({v:"0.0", msg:"--", nv_msg:"--"}, function(err, res) {});
            }
        });
    });
},
10000);

async function insertUser(uid: string){
    const query = {uid: uid}
    db.collection(USERS_COLLECTION).findOne(query, function(err, result) {
        if (err){
            logError("\n\nMongo Error 02: " + String(err))
            return
        }
        if(result == null){
            const date = Date.now();
            const new_user = {uid:uid, time:date, username:""}
            db.collection(USERS_COLLECTION).insertOne(new_user, function(err, res) {
                if (err){
                    console.log("Mongo Error 03:", err)
                    return
                }
                console.log("New user registered!", getDateTime());
              });
        }
      });
}


async function insertMatch(room: Room, private_code: string, callBack: (match_id: string)=>void){
    const date = Date.now();
    const new_match = {player1_uid:room.player1.uid, 
                      player2_uid:room.player2.uid,
                      creation_time: room.timestamp,
                      start_time: date,
                      private_code: private_code,
                      winner: ""}

    db.collection(MATCHES_COLLECTION).insertOne(new_match, function(err, res) {
        if (err){
            logError("\n\nMongo Error 03: " + String(err))
            return
        }
        callBack(res.insertedId)
    });
}

async function endMatch(uid: string, status: string, match_id:string){
    if(status=="win"){
        const query = { _id:ObjectId(match_id) }
        const operation = { $set:{winner: uid} }
        db.collection(MATCHES_COLLECTION).updateOne(query, operation, function(err, res) {
            if (err){
                logError("\n\nMongo Error 04: " + String(err))
                return
            }
        });
    }

}

export {insertMatch, insertUser, endMatch}