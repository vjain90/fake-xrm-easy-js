(function(exports) {   
    
    function CreateSection(name, label, disabled, visible) {
        
        var section = new Section(name, label, disabled, visible);
        return section;
    };
    
        
    Section = function (_name, _label, _disabled, _visible) {
        this.name = _name;
        this.label = _label;
        this.visible = _visible !== 'undefined' ? _visible : true;
        this.disabled = _disabled !== 'undefined' ? _disabled : true;
        
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
    
    exports.CreateSection = CreateSection;
    
    })(typeof exports === 'undefined' ? this['Section.js'] = {} : exports);