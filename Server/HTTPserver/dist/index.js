"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const MongoDB_1 = require("./MongoDB");
var express = require('express');
var app = express();
var cors = require('cors');
const port = process.env.HTTP_PORT || 3000;
const corsOptions = {
    origin: '*',
    optionsSuccessStatus: 200 // some legacy browsers (IE11, various SmartTVs) choke on 204
};
app.use(cors(corsOptions));
app.get('/info', function (req, res) {
    try {
        const callback = (msg) => { res.send(msg); };
        MongoDB_1.getAPPInfo(callback);
    }
    catch (err) {
        console.log("Error ", err);
        res.send("err");
    }
});
setTimeout(() => {
    /*app.listen(port, function () {
        console.log('HTTP server started on port ',port);
    });*/
}, 10000);
//# sourceMappingURL=index.js.map