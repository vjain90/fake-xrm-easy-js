(function(exports) {   

/*
 *@param requiredLevel:  none; required; recommended
 *@param disabled true/false
 * @param visible true/false
 */

var attributeJS = require('../Attributes/Attribute.js'); 

function CreateSimpleControlWithAttribute(name, value, label) {
    return CreateControlWithAttribute(name, value, 'none', label, false, true);
};

function CreateControlWithAttribute(name, value, requiredLevel, label, disabled, visible,_options=[]) {
    var submitmode = 'dirty';
    if (disabled || (visible == false))
        submitmode = 'never';

    var attr = new attributeJS.Attribute(name, value, requiredLevel, submitmode);
    var ctrl = new Control(attr, name, label, disabled, visible,_options);
    return ctrl;
};

    
Control = function (_attribute, _name, _label, _disabled, _visible,_options) {
    this.name = _name;
    this.label = _label;
    this.visible = _visible !== 'undefined' ? _visible : true;
    this.disabled = _disabled !== 'undefined' ? _disabled : true;
    this.attribute = new Object();
    this.attribute = _attribute;
    this.options=_options;
    this.getAttribute = function() {
        return this.attribute;
    };
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
    this.getControlType = function () {
        return "attribute";
    };
    this.getOptions=function()
    {
        return this.options;
    };
    this.removeOption=function(option)
    {
        var index =this.options.indexOf(option);
    if (index !== -1)
    this.options.splice(index, 1); 
     
    };
    
}

exports.CreateControlWithAttribute = CreateControlWithAttribute;

})(typeof exports === 'undefined' ? this['Controls.js'] = {} : exports);