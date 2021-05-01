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
exports.getAPPInfo = void 0;
var MongoClient = require('mongodb').MongoClient;
const { MONGO_USERNAME, MONGO_PASSWORD, MONGO_HOSTNAME, MONGO_PORT, MONGO_DB } = process.env;
var url = `mongodb://ferrRAB:RABgogo0708@127.0.0.1:27017/?authSource=admin`;
var db;
const INFO_COLLECTION = "info";
setTimeout(() => {
    MongoClient.connect(url, { reconnectTries: 60, reconnectInterval: 1000 }, function (err, database) {
        if (err) {
            console.log("Mongo Error 01:", err);
            return;
        }
        console.log("MongoDB connected!");
        db = database.db(MONGO_DB);
    });
}, 10000);
function getAPPInfo(callback) {
    return __awaiter(this, void 0, void 0, function* () {
        db.collection(INFO_COLLECTION).findOne({}, function (err, result) {
            if (err) {
                console.log("Mongo Error 02:", err);
                callback("err");
                return;
            }
            callback(result);
        });
    });
}
exports.getAPPInfo = getAPPInfo;
//# sourceMappingURL=MongoDB.js.map