import { logError } from './utils'
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
const INFO_COLLECTION = "info"

setTimeout(() => 
{
    MongoClient.connect(url,{ reconnectTries: 60, reconnectInterval: 1000 }, function(err, database) {
        if (err){
            logError("\n\nMongo Error 01: "+String(err))
            return
        }
        console.log("MongoDB connected!")
        db = database.db(MONGO_DB);
    });
},
10000);



async function getAPPInfo(callback: (msg: string)=>void){
    db.collection(INFO_COLLECTION).findOne({}, { fields: {v: 1, msg: 1, nv_msg:1, _id: 0} } ,function(err, result) {
        if (err){
            console.log("Mongo Error 02:", err)
            callback("err")
            return
        }
        callback(result)
      });
}

export {getAPPInfo}