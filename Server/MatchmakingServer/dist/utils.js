"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.logError = exports.genMatchID = exports.getDateTime = void 0;
const fs = require('fs');
const id_len = 5;
const id_chars = "0123456789abcdefghijklmnopqrstuvwxyz";
function getDateTime() {
    var currentdate = new Date();
    var datetime = "Last Sync: " + currentdate.getDate() + "/"
        + (currentdate.getMonth() + 1) + "/"
        + currentdate.getFullYear() + " @ "
        + currentdate.getHours() + ":"
        + currentdate.getMinutes() + ":"
        + currentdate.getSeconds();
    return datetime;
}
exports.getDateTime = getDateTime;
function genMatchID() {
    var result = '';
    for (var i = id_len; i > 0; --i)
        result += id_chars[Math.floor(Math.random() * id_chars.length)];
    return result;
}
exports.genMatchID = genMatchID;
function logError(err) {
    fs.appendFile('../logs.txt', err, function (err) {
        console.log('Log saved!');
    });
}
exports.logError = logError;
//# sourceMappingURL=utils.js.map