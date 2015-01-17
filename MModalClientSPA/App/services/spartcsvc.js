//  Filename: spartcsvc.js

'use strict';

app.service('spaRtcSvc', function ($rootScope, PatientEncounterVisit, JobState, RecordPlayback, GlobalProperties) {

    var mMClientHubProxy = $.connection.mMClientHub;

    var notifyRtcChanged = function () {
        $rootScope.$broadcast('rtcChanged', null);
    };
    
    //
    //  Client Methods
    //

    mMClientHubProxy.on('spaPing', function () {
        mMClientHubProxy.server.spaPong();
    });

    mMClientHubProxy.on('setPatientContext', function (context) {
        console.log('SPA.setPatientContext: ' + JSON.stringify(context));

        GlobalProperties.dictationContext = context;
        notifyRtcChanged();
    });

    mMClientHubProxy.on('setRequestContext', function (context) {
        console.log('SPA.setRequestContext: ' + JSON.stringify(context));

        GlobalProperties.requestContext = context;
        notifyRtcChanged();
    });

    mMClientHubProxy.on('setPatientInfo', function (context) {
        console.log('SPA.setPatientInfo: ' + JSON.stringify(context));

        
        PatientEncounterVisit.pevPatientId = context.patientId;
        PatientEncounterVisit.pevFirstName = context.firstName;
        PatientEncounterVisit.pevLastName = context.lastName;
        PatientEncounterVisit.pevBirthDate = context.birthDate;
        PatientEncounterVisit.pevAge = context.age;
        PatientEncounterVisit.pevSex = context.sex;
        PatientEncounterVisit.pevOfficeLocation = context.officeLocation;
        PatientEncounterVisit.pevDateOfService = context.dateOfService;
        PatientEncounterVisit.pevDictationContext = context.dictationContext;
        PatientEncounterVisit.pevDictationJobId = context.dictationJobId;        
        PatientEncounterVisit.pevReviewRequired = context.reviewRequired;

        JobState.dictationJobId = context.dictationJobId;
        JobState.reviewRequired = context.reviewRequired;
        
        notifyRtcChanged();
    });

    mMClientHubProxy.on('setCurrentDateTime', function (value) {
        //  GNDN
    });

    mMClientHubProxy.on('setVolume', function (volume) {        
        RecordPlayback.volume = volume;
        $rootScope.$broadcast('rtcVolumeChanged', volume);
    });

    mMClientHubProxy.on('setSoundLength', function (length) {
        RecordPlayback.soundLength = length;

        $rootScope.$broadcast('rtcSoundLenChanged', length);
    });

    mMClientHubProxy.on('setSoundPosition', function (position) {
        RecordPlayback.soundPosition = position;

        $rootScope.$broadcast('rtcSoundPosChanged', position);
    });

    mMClientHubProxy.on('setRecordingTimeDisplay', function (text) {
        $rootScope.$broadcast('rtcTimeDisplay', text);
    });

    mMClientHubProxy.on('setInstructionsCanExecute', function (disable) {
        //  GNDN        
    });

    mMClientHubProxy.on('setRecordCanExecute', function (disable) {
        RecordPlayback.disableRecord = disable;

        notifyRtcChanged();
    });

    mMClientHubProxy.on('setPlayCanExecute', function (disable) {
        RecordPlayback.disablePlay = disable;

        notifyRtcChanged();
    });

    mMClientHubProxy.on('setStopCanExecute', function (disable) {
        RecordPlayback.disableStop = disable;

        notifyRtcChanged();
    });

    mMClientHubProxy.on('setRewindCanExecute', function (disable) {
        RecordPlayback.disableRewind = disable;

        notifyRtcChanged();
    });

    mMClientHubProxy.on('setFastForwardCanExecute', function (disable) {
        RecordPlayback.disableFastForward = disable;

        notifyRtcChanged();
    });

    mMClientHubProxy.on('setBeginningCanExecute', function (disable) {
        RecordPlayback.disableBeginning = disable;
        notifyRtcChanged();
    });

    mMClientHubProxy.on('setEndCanExecute', function (disable) {
        RecordPlayback.disableEnd = disable;
        notifyRtcChanged();
    });

    //
    //  Connection lifetime events
    //
    $.connection.hub.starting(function () {
        console.log('SPA: starting');
    });

    $.connection.hub.received(function (data) {
        console.log('SPA: received ==> ' + JSON.stringify(data));
    });

    $.connection.hub.connectionSlow(function () {
        console.log('SPA: connectionSlow');
    });

    $.connection.hub.reconnecting(function () {
        console.log('SPA: reconnecting');
    });

    $.connection.hub.reconnected(function () {
        console.log('SPA: reconnected');
    });

    $.connection.hub.stateChanged(function (stateObj) {
        console.log('SPA: stateChanged ==> oldState: ' + stateObj.oldState + ' newState: ' + stateObj.newState);
    });

    $.connection.hub.disconnected(function () {
        console.log('SPA: disconnected');
    });

    //
    //  Recorder Server Methods
    //

    var setRecorderStatus = function (status) {
        RecordPlayback.recorderStatus = status;
        notifyRtcChanged();
    };

    var setRecorderError = function (error) {
        RecordPlayback.recorderStatus = 'Error: ' + error;
        notifyRtcChanged();
    };

    var setGlobalWaveVolume = function () {
        mMClientHubProxy.server.setGlobalWaveVolume(RecordPlayback.globalWaveVolume)
        .done(function () {
            //  nothing to see here, move along
        })
        .fail(function (error) {
            console.log('spaRtcSvc::setGlobalWaveVolume failed: ' + error);
        });
    }

    var doRecord = function () {
        mMClientHubProxy.server.recordExecute()
        .done(function () {
            setRecorderStatus('Recording');
        })
        .fail(setRecorderError(error));
    };

    var doPlay = function () {
        mMClientHubProxy.server.playExecute()
        .done(function () {
            setRecorderStatus('Playing');
        })
        .fail(setRecorderError(error));
    };

    var doStop = function () {
        mMClientHubProxy.server.stopExecute()
        .done(function () {
            setRecorderStatus('Stopped');
        })
        .fail(setRecorderError(error));
    };

    var doRewind = function () {
        mMClientHubProxy.server.rewindExecute()
        .done(function () {
            setRecorderStatus('Rewinding');
        })
        .fail(setRecorderError(error));
    };

    var doFastForward = function () {
        mMClientHubProxy.server.fastForwardExecute()
        .done(function () {
            setRecorderStatus('Fast Forwarding');
        })
        .fail(setRecorderError(error));
    };

    var doBeginning = function () {
        mMClientHubProxy.server.beginningExecute()
        .done(function () {
            setRecorderStatus('Go to Beginning');
        })
        .fail(setRecorderError(error));
    };

    var doEnd = function () {
        mMClientHubProxy.server.endExecute()
        .done(function () {
            setRecorderStatus('Go to End');
        })
        .fail(setRecorderError(error));
    };

    //
    //  Save/Pend/Cancel
    //
    var doSave = function () {
        mMClientHubProxy.server.saveDictation({
            dictationJobId: JobState.dictationJobId,
            stat: JobState.stat,
            reviewRequired: JobState.reviewRequired})
        .done(function () {
            setRecorderStatus('Dictation Saved');
        })
        .fail(setRecorderError(error));
    };

    var doPend = function () {
        mMClientHubProxy.server.pendDictation({
            dictationJobId: JobState.dictationJobId,
            stat: JobState.stat,
            reviewRequired: JobState.reviewRequired
        })
        .done(function () {
            setRecorderStatus('Dictation Pended');
            $.connection.hub.stop();
        })
        .fail(setRecorderError(error));
    };

    var doCancel = function () {
        mMClientHubProxy.server.cancelDictation(JobState.dictationJobId)
        .done(function () {
            setRecorderStatus('Dictation Cancelled');
            $.connection.hub.stop();
        })
        .fail(setRecorderError(error));
    };


    var initialize = function () {
        console.log('spaRtcSvc::initialize');

        $.connection.hub.qs = { 'client': 'SPA-' + rtcGroup, 'group': rtcGroup };

        $.connection.hub.start()
            .done(function () {
                mMClientHubProxy.invoke('getPatientContext');
            })
            .fail(function () {
                console.log('spaRtcSvc::initialize - hub.start failed');
            });
    };

    var closeServerChannel = function () {
        console.log('spaRtcSvc::closeServerChannel');
        $.connection.hub.stop();
    };

    var cancelDictation = function (jobid) {
        mMClientHubProxy.server.cancelDictation(jobid);
        closeServerChannel();
    };

    return {
        initialize: initialize,
        closeServerChannel: closeServerChannel,
        cancelDictation: cancelDictation,
        doRecord: doRecord,
        doPlay: doPlay,
        doStop: doStop,
        doRewind: doRewind,
        doFastForward: doFastForward,
        doBeginning: doBeginning,
        doEnd: doEnd,
        doSave: doSave,
        doPend: doPend,
        doCancel: doCancel,
        setGlobalWaveVolume: setGlobalWaveVolume
    };

});