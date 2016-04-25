//===============================================================================================
// Mod:     Core
// Author:  thien.l.do
// Ref:     http://splitterjsobj.sourceforge.net/
//          Use: jquery.splitter.js            
// Edit:    
// Create:  2011-06-22
// Modify:  yyyy-MM-dd
//================================================================================================
//EXAMPLE:
/*
var data = '' +
'album; artist; price\n' +
'"late\\"ralus"; "tool"; 13.00\n' +
'"aenima"; "tool"; 12.00\n' +
'"10,000 days"; "tool"; 14.00\n' +
'"down in it"; "nine, inch   nails"; 3.00\n' +
'"broken"; "nine;inch   nails"; 6.00';
//Or 
//data = 'Get data from service in success event';
var obj = data.CSV2JSON();
console.log(obj);
*/
//================================================================================================
var SeperatorData = '\n###\n';
function CCSV(data) {
    this.Header = [];
    this.Data = [];
    this.Options = {
        SeperatorColumn: ';',   //!This is important option
        SeperatorRow: '\n'      //!This is important option
    };

    this.ToJSON = function (headers, data) {
        var obj = {};
        try {
            for (i = 0; i < headers.length; i++) {
                obj[headers[i]] = data[i];
            }
        }
        catch (e) {
            if (console) {
                console.log('::ERROR in CCSV.ToJSON()');
                console.log('  ' + e);
                console.log('  Data: ' + data);
            }
        }
        return obj;
    };
    this.Init = function () {
        try {
            //debugger;
            var splitter = new Splitter();
            var Delimiters = [[/\r?\n/, undefined, false, undefined], [/\s*;\s*/, undefined, false, undefined]];
            var arrData = splitter.splitNlevels(data, Delimiters)[1];

            if (arrData.length > 0) {
                this.Header = arrData[0];
            }
            var i = 1;
            for (i = 1; i < arrData.length; i++) {
                var obj = this.ToJSON(this.Header, arrData[i]);
                this.Data.push(obj);
            }
        }
        catch (e) {
            if (console) {
                console.log('::ERROR in CCSV.Init()');
                console.log(e);
            }
            return null;
        }
    }
    this.Init();
}

String.prototype.CSV2JSON = function (seperator) {
    var csvData = this;
    var csvObj = new CCSV(csvData);
    return csvObj.Data;
}


String.prototype.CSV2JSON2 = function (seperator) {
    var csvDatas = this;
    var csvData = csvDatas.split(SeperatorData);
    //console.log(csvData);
    var ret = [];
    for (j = 0; j < csvData.length; j++) {
        //console.log('::i = ' + j);
        //console.log(csvData[j]);
        var csvObj = new CCSV(csvData[j]);
        //console.log(csvObj.Data);
        ret.push(csvObj.Data);
    }
    return ret;
}