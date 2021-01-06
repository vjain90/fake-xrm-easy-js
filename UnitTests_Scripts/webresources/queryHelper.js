﻿(function (exports) {

    function getRecords(query) {
        var req = new XMLHttpRequest();
        
        var clientUrl = Xrm.Page.context.getClientUrl();

        req.open("GET", encodeURI(clientUrl + "/api/data/v9.1/" + query), false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.onreadystatechange = function () {
            if (this.readyState == 4/* complete */) {
                req.onreadystatechange = null;
                if (this.status == 200) {
                    var accountUri = this.getResponseHeader("OData-EntityId");
                    console.log("Created account with URI: " + accountUri)
                }
                else {
                    var error = JSON.parse(this.response).error;
                    console.log(error.message);
                }
            }
        };
        req.send({});
    }

    function getMultipleRecords(query, successCallback, errorCallback) {
        var req = new XMLHttpRequest();

        var clientUrl = Xrm.Page.context.getClientUrl();        

        req.open("GET", encodeURI(clientUrl + "/api/data/v9.1/" + query), false);
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.onreadystatechange = function () {
            if (this.readyState == 4/* complete */) {
                req.onreadystatechange = null;
                if (this.status == 200) {
                    var data = JSON.parse(this.response);
                    if (successCallback) {
                        successCallback(data);
                    }
                }
                else {
                    var error = JSON.parse(this.response).error;
                    if (errorCallback) {
                        errorCallBack(error.message);
                    }
                    else
                        console.log(error.message);
                }
            }
        };
        req.send({});
    }
    
    exports.get = getRecords;
    exports.retrieveMultiple = getMultipleRecords;

})(typeof exports === 'undefined' ? this['queryHelper'] = {} : exports);


