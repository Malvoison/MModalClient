//  Filename:  mmodalrtcspa.js

var mMClientHubProxy = $.connection.mMClientHub;

//
//  Client Methods
//

mMClientHubProxy.on('spaPing', function () {
    mMClientHubProxy.server.spaPong();
});

mMClientHubProxy.on('setPatientContext', function (context) {
    outputDebugString('SPA.setPatientContext: ' + JSON.stringify(context));

    globalProperties.dictationContext = context;

    //patientContextDao.savePatientContext(context).then(function (retval) {
    //    console.log('Promise kept: ', retval);
    //}, function (error) {
    //    console.error('Promise broken', error);
    //});
});

mMClientHubProxy.on('setRequestContext', function (context) {
    outputDebugString('SPA.setRequestContext: ' + JSON.stringify(context));

    globalProperties.requestContext = context;
});

mMClientHubProxy.on('setPatientInfo', function (context) {
    outputDebugString('SPA.setPatientInfo: ' + JSON.stringify(context));

    patientEncounterVisit.patientId = context.patientId;
    patientEncounterVisit.firstName = context.firstName;
    patientEncounterVisit.lastName = context.lastName;
    patientEncounterVisit.birthDate = context.birthDate;
    patientEncounterVisit.age = context.age;
    patientEncounterVisit.sex = context.sex;    
    patientEncounterVisit.officeLocation = context.officeLocation;
    patientEncounterVisit.dateOfService = context.dateOfService;
    patientEncounterVisit.dictationContext = context.dictationContext;
    patientEncounterVisit.dictationJobId = context.dictationJobId;

    jobState.dictationJobId = context.dictationJobId;
    jobState.reviewRequired = context.reviewRequired;
});

mMClientHubProxy.on('setCurrentDateTime', function (value) {
    setCurrentDateTime(value);
});

mMClientHubProxy.on('setVolume', function (volume) {
    setVolume(volume);
});

mMClientHubProxy.on('setSoundLength', function (length) {
    setSoundLength(length);
});

mMClientHubProxy.on('setSoundPosition', function (position) {
    setSoundPosition(position);
});

mMClientHubProxy.on('setInstructionsCanExecute', function (can) {
    setInstructionsCanExecute(can);
});

mMClientHubProxy.on('setRecordCanExecute', function (can) {
    setRecordCanExecute(can);
});

mMClientHubProxy.on('setPlayCanExecute', function (can) {
    setPlayCanExecute(can);
});

mMClientHubProxy.on('setStopCanExecute', function (can) {
    setStopCanExecute(can)
});

mMClientHubProxy.on('setRewindCanExecute', function (can) {
    setRewindCanExecute(can);
});

mMClientHubProxy.on('setFastForwardCanExecute', function (can) {
    setFastForwardCanExecute(can);
});

mMClientHubProxy.on('setBeginningCanExecute', function (can) {
    setBeginningCanExecute(can);
});

mMClientHubProxy.on('setEndCanExecute', function (can) {
    setEndCanExecute(can);
});

//
//  Connection lifetime events
//
$.connection.hub.starting(function () {
    outputDebugString('SPA: starting');
});

$.connection.hub.received(function (data) {
    outputDebugString('SPA: received ==> ' + JSON.stringify(data));
});

$.connection.hub.connectionSlow(function () {
    outputDebugString('SPA: connectionSlow');
});

$.connection.hub.reconnecting(function () {
    outputDebugString('SPA: reconnecting');
});

$.connection.hub.reconnected(function () {
    outputDebugString('SPA: reconnected');
});

$.connection.hub.stateChanged(function (stateObj) {
    outputDebugString('SPA: stateChanged ==> oldState: ' + stateObj.oldState + ' newState: ' + stateObj.newState);
});

$.connection.hub.disconnected(function () {
    outputDebugString('SPA: disconnected');
});

function initServerChannel(group) {

    console.log('initServerChannel');
    //  Connection configuration/start
    //var client = getRtcClient();
    //var group = getRtcGroup();

    $.connection.hub.qs = { 'client': 'SPA-' + group, 'group': group };

    $.connection.hub.start()
        .done(function () {
            outputDebugString('SPA connected, connection ID=' + $.connection.hub.id);
            dotNetCallbackObj = mMClientHubProxy;
            requestPatientInfo();
        })
        .fail(function () {
            outputDebugString('Could not connect!');
        });
}

function closeServerChannel() {
    console.log('closeServerChannel');

    $.connection.hub.stop();
}

function requestPatientInfo() {
    mMClientHubProxy.invoke('getPatientContext');
}