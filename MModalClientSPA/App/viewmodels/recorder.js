define(function (require) {

    var app = require('durandal/app'),
        ko = require('knockout'),
        observable = require('plugins/observable');
                            
    return {

        //  Header Information
        pevPatientId: observable(patientEncounterVisit, 'patientId'),
        pevFirstName: observable(patientEncounterVisit, 'firstName'),
        pevLastName: observable(patientEncounterVisit, 'lastName'),
        pevBirthDate: observable(patientEncounterVisit, 'birthDate'),
        pevAge: observable(patientEncounterVisit, 'age'),
        pevSex: observable(patientEncounterVisit, 'sex'),
        pevOfficeLocation: observable(patientEncounterVisit, 'officeLocation'),
        pevDateOfService: observable(patientEncounterVisit, 'dateOfService'),
        pevDictationContext: observable(patientEncounterVisit, 'dictationContext'),

        leTraceMessages: observable(traceMessages, 'messageList'),
        progressPercent: observable(deviceEvents, 'volume').subscribe(function (value) {
            var newProgressSuccess = value + '%';

            $('#vuMeter').css('width', newProgressSuccess);
            outputDebugString('deviceEvents.volume fired');
        }),

        progressSoundPosition: observable(deviceEvents, 'soundPosition').subscribe(function (value) {
            
            //  must be less than or equal to sound length
            if (value > deviceEvents.soundLength)
                return;

            var newSoundPosition = value;
                        
            $('#ex1').slider('setValue', newSoundPosition * 1);
        }),
        progressSoundLength: observable(deviceEvents, 'soundLength').subscribe(function (value) {
            
            outputDebugString('soundLength: ' + value);

            var newMax = (value == 0) ? 1 : value;

            outputDebugString('newMax: ' + newMax);

            $('#ex1').slider('setAttribute', 'max', newMax * 1);
        }),

        progressRecorderStatus: observable(deviceEvents, 'recorderStatus'),
        progressRecorderTime: observable(deviceEvents, 'currentDateTime'),

        canInstructions: observable(recordPlayback, 'instructionsCanExecute'),
        canRecord: observable(recordPlayback, 'recordCanExecute'),
        canPlay: observable(recordPlayback, 'playCanExecute'),
        canStop: observable(recordPlayback, 'stopCanExecute'),
        canRewind: observable(recordPlayback, 'rewindCanExecute'),
        canFastForward: observable(recordPlayback,'fastForwardCanExecute'),
        canBeginning: observable(recordPlayback, 'beginningCanExecute'),
        canEnd: observable(recordPlayback, 'endCanExecute'),

        //  STAT and Review Required
        doStat: observable(jobState, 'stat'),
        doReviewRequired: observable(jobState, 'reviewRequired'),

        //  Test Code
        canRequestAuthor: observable(globalProperties, 'requestContext').subscribe(function(value) {
            if (value == '')
                return false;
            else
                return true;
        }),
        doRequestAuthor: function () {
            jQuery.support.cors = true;
            $.ajax({
                url: 'http://localhost:37574/api/Author/FindByFFTAuthorId/8564',
                type: 'POST',
                data: JSON.stringify(globalProperties.requestContext),
                contentType: 'application/json',
                success: function (data) {
                    console.log('Author object: ', data);
                },
                error: function (x, y, z) {
                    console.error('Author error: ', x + '\n' + y + '\n' + z);
                }
            });
        },

        doSubmitDictation: function () {
            jQuery.support.cors = true;
            $.ajax({
                url: 'http://localhost:37574/api/DictationJobs/PostDictationJob',
                type: 'POST',
                data: JSON.stringify(globalProperties.dictationContext),
                contentType: 'application/json',
                success: function (data) {
                    console.log('Dictation object: ', data);
                },
                error: function (x, y, z) {
                    console.error('Dictation error: ', x + '\n' + y + '\n' + z);
                }
            });
        },

        doInstructions: function () {
            recordPlayback.instructionsExecute();
        },
        doRecord: function () {
            recordPlayback.recordExecute();
        },
        doPlay: function () {
            recordPlayback.playExecute();
        },
        doStop: function () {
            recordPlayback.stopExecute();
        },
        doRewind: function () {
            recordPlayback.rewindExecute();
        },
        doFastForward: function () {
            recordPlayback.fastForwardExecute();
        },
        doBeginning: function () {
            recordPlayback.beginningExecute();
        },
        doEnd: function () {
            recordPlayback.endExecute();
        },

        doSave: function () {
            recordPlayback.saveDictation();
        },

        doPend: function () {
            recordPlayback.pendDictation();
        },

        doCancel: function () {            
            recordPlayback.cancelDictation();
        },

        hostAlert: function (alertMessage) {
            alert('hostAlert');            
        },
        activate: function () {
            //  GNDN            
            outputDebugString('shell view model: activate');
        },
        attached: function () {
            outputDebugString('shell view model: attached');
            $('#ex1').slider({
                formatter: function (value) {
                    return 'Current value: ' + value;
                }
            });

            $('#ex2').slider({
                formatter: function (value) {
                    return 'Current value: ' + value;
                }
            });

            $('.slider.slider-horizontal').css('width', '100%');

            $('.slider.slider-horizontal').not(':first-child').css('width', '40%');

            $('.slider-handle').css('background', '#0000FF');

            $('.slider-selection').css('background', '#000000');

            $('.well').css('background-color', '#A32638');

            $('#ex1').slider('setValue', 0);

            $('#ex1').slider('setAttribute', 'max', 1);

            $('#ex2').slider('setValue', deviceEvents.globalWaveVolume);

            $('#ex2').slider('setAttribute', 'max', 100);

            $('#ex2').on('slide', function (slideEvt) {                
                recordPlayback.globalWaveVolume = slideEvt.value;
                recordPlayback.setGlobalWaveVolume();
            });
        }
    };    
});

