const fs = require('fs');

export function logError(err: string){
    fs.appendFile('logs.txt', err , function (err) {
        console.log('Error log saved!');
    });
}
