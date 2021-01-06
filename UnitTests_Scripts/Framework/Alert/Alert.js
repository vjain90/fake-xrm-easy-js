(function(exports) {
Alert = {
    message: "",
    label:"",
    Buttons: [],    
    clickButton(clickedButtonLabel){
        for (var button in this.Buttons) {
            if (this.Buttons[button].label == clickedButtonLabel) {
                this.Buttons[button].click();                
            }
        };
    },
    show(title, message, buttons, icon, width, height, baseUrl, preventCancel, padding) {
        this.message = title;        
    }
};

Alert.Button = function(label, callback, setFocus, preventClose) {    
    this.label = label;
    this.click = callback;
    Alert.Buttons.push(this);
};

exports.Alert = Alert;
exports.Alert.Button = Alert.Button;

})(typeof exports === 'undefined' ? this['Controls.js'] = {} : exports);