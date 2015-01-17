
//  Globals for host/page communications
//var sysTrayHubConnection = $.hubConnection('http://localhost:8666');
//var sysTrayHubProxy = sysTrayHubConnection.createHubProxy('sysTrayHub');

//sysTrayHubProxy.on('setAppUrl', function (url) {
//    globalProperties.appUrl = url;
//});

//sysTrayHubProxy.on('setFileData', function (data) {
//    dictationContext.requestObject.waveData = data;
//});

//  IIFE
//(function startSysTrayConnection() {
//    sysTrayHubConnection.start()
//        .done(function () {
//            console.log('sysTrayHubConnection complete, ID=' + sysTrayHubConnection.id);
//            //  Start the server channel...but only if we succeded...
//            initServerChannel();
//        })
//        .fail(function () {
//            console.log('sysTrayHubConnection could not connect');
//        });
//})();

//  Connection lifetime events

var globalProperties = {
    hostMessage: 'I am Sam.',
    appUrl: '',
    apiUrl: '',
    rtcUrl: '',
    rtcClient: 'TEST',
    rtcGroup: 'TEST',
    requestContext: '',
    dictationContext: '',
    patientContext: ''
};

var patientEncounterVisit = {
    patientId: -1,
    firstName: '',
    lastName: '',
    birthDate: '',
    age: '',
    sex: '',
    officeLocation: '',
    dateOfService: '',
    dictationContext: '',
    dictationJobId: -1
};

function doConsoleLog(message) {
    console.log(message);
}

function getHostMessage() {
    return globalProperties.hostMessage;
}

function setHostMessage(newMessage) {
    globalProperties.hostMessage = newMessage;    
}

function getAppUrl() {
    sysTrayHubProxy.invoke('getAppUrl').done(function (url) {
        console.log('invoke: getAppUrl succeeded');
        globalProperties.appUrl = url;
        return globalProperties.appUrl;
    }).fail(function (error) {
        console.log('invoke: getAppUrl failed. Error: ' + error);
        return '';
    });
}

function getApiUrl() {
    sysTrayHubProxy.invoke('getApiUrl').done(function (url) {
        console.log('invoke: getApiUrl succeeded');
        globalProperties.apiUrl = url;
        return globalProperties.apiUrl;
    }).fail(function (error) {
        console.log('invoke: getApiUrl failed. Error: ' + error);
        return '';
    });
}

function getRtcUrl() {
    sysTrayHubProxy.invoke('getRtcUrl').done(function (url) {
        console.log('invoke: getRtcUrl succeeded');
        globalProperties.rtcUrl = url;
        return globalProperties.rtcUrl;
    }).fail(function (error) {
        console.log('invoke: getRtcUrl failed. Error: ' + error);
        return '';
    });
}

function getRtcClient() {
    sysTrayHubProxy.invoke('getRtcClient').done(function (client) {
        console.log('invoke: getRtcClient succeeded');
        globalProperties.rtcClient = client;
        return globalProperties.rtcClient;
    }).fail(function (error) {
        console.log('invoke: getRtcClient failed. Error: ' + error);
        return '';
    });
}

function getRtcGroup() {
    //sysTrayHubProxy.invoke('getRtcGroup').done(function (group) {
    //    console.log('invoke: getRtcGroup succeeded');
    //    globalProperties.rtcGroup = group;
    //    return globalProperties.rtcGroup;
    //}).fail(function (error) {
    //    console.log('invoke: getRtcGroup failed. Error: ' + error);
    //    return '';
    //});

    return sysTrayHubProxy.invoke('getRtcGroup');
}

function setMainWindowSize(width, height) {
    window.resizeTo(width, height);
    window.moveTo((screen.availwidth - width) / 2, (screen.availheight - height) / 2);
    window.focus();
}

function setMainWindowTitle(title) {
    // gndn
}

function outputDebugString(message) {
    console.log(message);
}

//  Device Events
//
var deviceEvents = {
    volume: 33,
    soundLength: 1,
    soundPosition: 1,
    recorderStatus: 'Stopped',
    currentDateTime: 'your mama'
    
};

var jobState = {
    dictationJobId: -1,
    stat: false,
    reviewRequired: false
};

function setVolume(volume) {
    deviceEvents.volume = volume;
}

function setSoundLength(length) {
    deviceEvents.soundLength = length;
}

function setSoundPosition(position) {
    deviceEvents.soundPosition = position;
}

function setCurrentDateTime(value) {
    console.log('setCurrentDateTime' + ' ' + value);
    deviceEvents.currentDatetime = value;
}

//  Record/Playback commands
//
var dotNetCallbackObj = null;

var recordPlayback = {
    volume: 33,
    globalWaveVolume: 100,
    setGlobalWaveVolume: function () {
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.setGlobalWaveVolume(this.globalWaveVolume)
            .done(function () {
                //  nothing to see here, move along
            })
            .fail(function (error) {
                outputDebugString('SetGlobalWaveVolume failed: ' + error);
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    instructionsCanExecute: false,
    instructionsExecute: function () {        
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.instructionsExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Instructions';
            })
            .fail(function (error) {
                outputDebugString('InstructionsExecute failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    recordCanExecute: false,
    recordExecute: function () {        
        if (dotNetCallbackObj) {         
            dotNetCallbackObj.server.recordExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Recording';
            })
            .fail(function (error) {
                outputDebugString("RecordExecute failed: " + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    playCanExecute: false,
    playExecute: function () {        
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.playExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Playing';
            })
            .fail(function (error) {
                outputDebugString('PlayExecute failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }   
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    stopCanExecute: false,
    stopExecute: function () {        
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.stopExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Stopped';
            })
            .fail(function (error) {
                outputDebugString('StopExecute failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    rewindCanExecute: false,
    rewindExecute: function () {        
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.rewindExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Rewinding';
            })
            .fail(function (error) {
                outputDebugString('RewindExecute failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    fastForwardCanExecute: false,
    fastForwardExecute: function () {        
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.fastForwardCanExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Fast Fowarding';
            })
            .fail(function (error) {
                outputDebugString('FastForwardExecute failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    beginningCanExecute: false,
    beginningExecute: function () {
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.beginningExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Go to Beginning';
                deviceEvents.recorderStatus = 'Stopped';
            })
            .fail(function (errr) {
                outputDebugString('BeginningExecute failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }
    },

    endCanExecute: false,
    endExecute: function () {        
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.endExecute()
            .done(function () {
                deviceEvents.recorderStatus = 'Go to End';
                deviceEvents.recorderStatus = 'Stopped';
            })
            .fail(function (error) {
                outputDebugString('EndExecute failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
        }        
    },

    saveDictation: function () {
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.saveDictation(jobState)
            .done(function () {
                deviceEvents.recorderStatus = 'Dictation saved';
                closeServerChannel();

            })
            .fail(function (error) {
                outputDebugString('SaveDictation failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');            
        }
    },

    pendDictation: function () {
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.pendDictation(jobState)
            .done(function () {
                deviceEvents.recorderStatus = 'Dictation pended';
                closeServerChannel();

            })
            .fail(function (error) {
                outputDebugString('PendDictation failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
            logSpaError('NullReferenceException', 'dotNetCallbackObj is null');
        }
    },

    cancelDictation: function () {
        if (dotNetCallbackObj) {
            dotNetCallbackObj.server.cancelDictation(jobState.dictationJobId)
            .done(function () {
                deviceEvents.recorderStatus = 'Dictation cancelled';
                closeServerChannel();
                //window.open('', '_self', '');
                //window.close();
            })
            .fail(function (error) {
                outputDebugString('CancelDictation failed: ' + error);
                deviceEvents.recorderStatus = 'Error';
            });
        }
        else {
            outputDebugString('dotNetCallbackObj is null');
            logSpaError('NullReferenceException', 'dotNetCallbackObj is null');
        }
    }
};

function setInstructionsCanExecute(canExecute) {
    recordPlayback.instructionsCanExecute = canExecute;
}

function setRecordCanExecute(canExecute) {
    recordPlayback.recordCanExecute = canExecute;
}

function setPlayCanExecute(canExecute) {
    recordPlayback.playCanExecute = canExecute;
}

function setStopCanExecute(canExecute) {
    recordPlayback.stopCanExecute = canExecute;
}

function setRewindCanExecute(canExecute) {
    recordPlayback.rewindCanExecute = canExecute;
}

function setFastForwardCanExecute(canExecute) {
    recordPlayback.fastForwardCanExecute = canExecute;
}

function setBeginningCanExecute(canExecute) {
    recordPlayback.beginningCanExecute = canExecute;
}

function setEndCanExecute(canExecute) {
    recordPlayback.endCanExecute = canExecute;
}


//  Debug-related stuff
var traceMessages = {
    messageList: []
};

function addEventTrace(trace) {
    traceMessages.messageList.push(trace);
}

//  Get file blob
function setFileBlob(encstr) {
    debugger;
   
    var typedArray = B64.decode(encstr);

    var blob = new Blob([typedArray], { type: 'application/octet-binary' });
    var url = URL.createObjectURL(blob);
    alert(url);
}

