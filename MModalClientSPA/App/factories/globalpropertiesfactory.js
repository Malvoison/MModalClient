// Filename: globalpropertiesfactory.js

app.factory('GlobalProperties', function GlobalProperties() {
    
    
    GlobalProperties.hostMessage = 'I am Sam.';
    GlobalProperties.appUrl = '';
    GlobalProperties.apiUrl = '';
    GlobalProperties.rtcUrl = '';
    GlobalProperties.rtcClient = 'TEST';
    GlobalProperties.rtcGroup = 'TEST';
    GlobalProperties.requestContext = '';
    GlobalProperties.dictationContext = '';
    GlobalProperties.patientContext = '';
    
    return GlobalProperties;
});