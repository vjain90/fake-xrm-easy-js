(function(exports) {   
    
    function CreateTabWithSections(name, label, disabled, visible, sections) {
        
        var tab = new Tab(name, label, disabled, visible, sections);
        return tab;
    };
    
        
    Tab = function (_name, _label, _disabled, _visible, _sections) {
        this.name = _name;
        this.label = _label;
        this.visible = _visible !== 'undefined' ? _visible : true;
        this.disabled = _disabled !== 'undefined' ? _disabled : true;
        
        if(_sections != null){
            this.sections = {
                collection: _sections,
                get: function (sectionName) {
                    for (var idx in this.collection) {
                        if (this.collection[idx].getName() == sectionName)
                            return this.collection[idx];
                    };
                    return null;
                },       
                forEach: function() {
                    throw Error("forEach is not ready")
                },
            };
        }

        this.getName = function () {
            return this.name;
        };
        this.getLabel = function () {
            return this.label;
        };
        this.setLabel = function (newLabel) {
            this.label = newLabel;
        };
        this.getVisible = function () {
            return this.visible;
        };
        this.setVisible = function (newVisible) {
            this.visible = newVisible;
        };
        this.setDisabled = function (newDisable) {
            this.disabled = newDisable;
        };
        this.getDisabled = function () {
            return this.disabled;
        };
    }
    
    exports.CreateTabWithSections = CreateTabWithSections;
    
    })(typeof exports === 'undefined' ? this['Tab.js'] = {} : exports);