//var xrmFakedContext = require('fakexrmeasy.2016');
//xrmFakedContext.setProxyPath('./Assemblies/FakeXrmEasy.EdgeProxy.v365');

(function(exports) {


ExecutionContextMock = function (_formContext, _saveMode) {
    this.getFormContext = function() {
        return _formContext;    
    };    
    this.getEventArgs = function() {        
        return new EventArgs(_saveMode);
    };  
};

EventArgs = function (savemode){
    this.getSaveMode = function() {
        return savemode;
    };
};

/*
 *@param formType 0 Undefined; 1 Create; 2 Update; 3 Read Only; 4 Disabled; 6 Bulk Edit
 */
FormContextMock = function (id, controls, tabs, formType) {
    this.ui = new FormContextUiMock(controls, tabs, formType);
    var attributes = new Array();
    for (var idx in controls) {
        attributes.push(controls[idx].getAttribute());
    };
    this.data = new FormContextDataMock(id, attributes);

    if(typeof(xrmFakedContext) != "undefined")
    {
        xrmFakedContext.data.forEach(function(ele){
        this.data.push(ele);
        });

        this.context = xrmFakedContext.Xrm.Page.context;
    }

    this.getAttribute = function (attributeName) {
        return this.data.entity.attributes.get(attributeName);
    };
    this.getControl = function (controlName) {
        return this.ui.controls.get(controlName);
    };
};

FormContextUiMock = function(controls, tabs, formType) {
    this.controls = {
        collection: controls,
        get: function (controlName) {
            for (var idx in this.collection) {
                if (this.collection[idx].getName() == controlName)
                    return this.collection[idx];
            };
            return null;
        },       
        forEach: function(callback) {
            for (var idx in this.collection) {                
                callback(this.collection[idx],idx);
            };            
        },
    };

    if(tabs != null){
        this.tabs = {
            collection: tabs,
            get: function (tabName) {
                for (var idx in this.collection) {
                    if (this.collection[idx].getName() == tabName)
                        return this.collection[idx];
                };
                return null;
            },       
            forEach: function() {
                throw Error("forEach is not ready")
            },
        };
    }

    this.formType = formType;
    this.getFormType = function(){
        return this.formType;
    }
}

FormContextDataMock = function (id, attributes) {
    this.refresh = function () {
        throw Error("Refresh is not ready")
    };
    this.save = function () {
        throw Error("Save is not ready")
    };
    this.entity = new FormContextDataEntityMock(id, attributes);
}


FormContextDataEntityMock = function (id, attributes) {
    this.id = id;
    this.getId= function () {
        return id;
    },
    this.attributes = {
        collection: attributes,
        get: function (attributeName) {
            for (var idx in this.collection) {
                if (this.collection[idx].getName() == attributeName)
                    return this.collection[idx];
            };
            return null;
        },       
        forEach: function() {
            throw Error("forEach is not ready")
        },
    };

    this.getIsDirty = function () {
        for (var idx in attributes) {
            if (attributes[idx].getIsDirty())
                return true;
        };
        return false;
    };
}

exports.FormContextMock = FormContextMock;
exports.ExecutionContextMock = ExecutionContextMock;

})(typeof exports === 'undefined' ? this['Controls.js'] = {} : exports);
