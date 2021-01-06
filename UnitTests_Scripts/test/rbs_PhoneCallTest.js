var edge = require('edge-js');
global.XMLHttpRequest = require("xmlhttprequest").XMLHttpRequest;
var xrmFakedContext = require('fakexrmeasy.2016');
xrmFakedContext.setProxyPath('./Assemblies/FakeXrmEasy.EdgeProxy.v365');
var queryHelper = require('../webresources/queryHelper.js');
var controlJS = require('../Framework/Control/Control.js'); 
var tabJS = require('../Framework/Tabs/Tab.js'); 
var sectionJS = require('../Framework/Sections/Section.js'); 
var formContextJS = require('../Framework/XrmPage/FormContext.js');
global.Xrm = {
    Utility: { 
        getGlobalContext() {
            return global.GetGlobalContext();
        }
    }
};
var mainCodeJS = require('../../SolutionFiles/Unmanaged/CustomerDueDiligence/WebResources/rbs_PhoneCall.js');
var assert = require('chai').assert;
var Guid = require('guid');

describe("rbs_PhoneCallTest.js", function () {
  
    it("cddScripts.PhoneCall.FormFunctions.OnLoad.SetCaseStatus(): System Generated PhoneCall: When Regarding is a Case with Case Status is Blank", function (done) {       

        var caseStatus = {
            id: Guid.create(),            
            logicalName: "rbs_case_activity",
            name: "Case Status 1"
        };
        
        var incident = {
            id: Guid.create(),
            rbs_caseactivity: caseStatus,
            logicalName: "incident"
        };
        
        var pageControls = new Array();
        pageControls.push(controlJS.CreateControlWithAttribute("regardingobjectid", [{
            id: incident.id.toString(),            
            name: "Case 1",
            entityType: "incident"
        }], "required", "Regarding", false, true));
        
        pageControls.push(controlJS.CreateControlWithAttribute("rbs_casestatus", null, "required", "Case Status", false, true));
        pageControls.push(controlJS.CreateControlWithAttribute("rbs_nextreminderon", new Date(), "required", "Case Status", false, true));
        
        global.FormContext = new formContextJS.FormContextMock("{" + Guid.create() + "}", pageControls, null, 2);
        global.ExecutionContext = new formContextJS.ExecutionContextMock(FormContext);
        
        xrmFakedContext.initialize(
            [
               caseStatus,
               incident
            ]
         );        
        
        cddScripts.PhoneCall.FormFunctions.OnLoad.SetCaseStatus(ExecutionContext);       
        
        setTimeout(function() {            
            assert.equal(FormContext.getAttribute("rbs_casestatus").getValue()[0].id, caseStatus.id); 
            done();
        },1000);
    });    
});