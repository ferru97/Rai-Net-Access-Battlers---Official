"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.getDateTime = void 0;
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
//# sourceMappingURL=utils.js.map