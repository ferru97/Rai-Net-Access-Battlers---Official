const fs = require('fs');
const id_len = 5;
const id_chars = "0123456789abcdefghijklmnopqrstuvwxyz"; 

export function getDateTime(): string{
    var currentdate = new Date(); 
    var datetime = "Last Sync: " + currentdate.getDate() + "/"
                + (currentdate.getMonth()+1)  + "/" 
                + currentdate.getFullYear() + " @ "  
                + currentdate.getHours() + ":"  
                + currentdate.getMinutes() + ":" 
                + currentdate.getSeconds();
    return datetime;
}

export function genMatchID() {
    var result = '';
    for (var i = id_len; i > 0; --i) result += id_chars[Math.floor(Math.random() * id_chars.length)];
    return result;
}

export function logError(err: string){
    fs.appendFile('logs.txt', err , function (err) {
        console.log('Error log saved!');
    });
}
