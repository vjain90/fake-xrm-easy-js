﻿function createAccount(clientURL) {
    var req = new XMLHttpRequest()
    req.open("POST", encodeURI(clientURL + "/api/data/v8.0/accounts"), true);
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.onreadystatechange = function () {
        if (this.readyState == 4/* complete */) {
            req.onreadystatechange = null;
            if (this.status == 204) {
                var accountUri = this.getResponseHeader("OData-EntityId");
                console.log("Created account with URI: " + accountUri)
            }
            else {
                var error = JSON.parse(this.response).error;
                console.log(error.message);
            }
        }
    };
    req.send(JSON.stringify({ name: "Sample account" }));
}