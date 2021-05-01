import { getAPPInfo } from './MongoDB'
import { logError } from './utils'
var express = require('express');
var app = express();
var cors = require('cors')

const port = process.env.HTTP_PORT || 3000

const corsOptions = {
    origin: '*',
    optionsSuccessStatus: 200 // some legacy browsers (IE11, various SmartTVs) choke on 204
  }

app.use(cors(corsOptions))

app.get('/info', function (req, res) {
    try{
        const callback = (msg:string) => { res.send(msg); };
        getAPPInfo(callback)
    }catch(err){
        logError("\n\HTTP server Error 01: "+String(err))
        res.send("err")
    }
});


app.listen(port, function () {
    console.log('HTTP server started on port ',port);
});