//  Filename:  recorderController.js

app.controller('recorderController', function recorderController($scope, PatientEncounterVisit, RecordPlayback, JobState, spaRtcSvc) {
    
    //$rootScope.pevPatientId = 12345;
    //$rootScope.pevFirstName = 'Johnny';
    //$rootScope.pevLastName = 'Patient';
    //$rootScope.pevBirthDate = '01/01/2014';
    //$rootScope.pevAge = '1';
    //$rootScope.pevSex = 'M';
    //$rootScope.pevOfficeLocation = '112 Ocean Ave';
    //$rootScope.pevDateOfService = '09/11/2001';
    //$rootScope.pevDictationContext = 'OP Note';

    $scope.pev = PatientEncounterVisit;

    //$scope.disableRecord = true;
    //$scope.disablePlay = true;
    //$scope.disableStop = true;
    //$scope.disableRewind = true;
    //$scope.disableFastForward = true;
    //$scope.disableBeginning = true;
    //$scope.disableEnd = true;

    $scope.recorder = RecordPlayback;

    $scope.jobState = JobState;

    $scope.recordingTimeDisplay = '00:00/00:00';

    //$scope.isStat = true;
    //$scope.isReviewRequired = true;

    //$scope.recorderStatus = 'Fubar';

    $scope.doSave = function() {
        spaRtcSvc.doSave();
    };

    $scope.doPend = function () {
        spaRtcSvc.doPend();
    };

    $scope.doCancel = function () {
        spaRtcSvc.doCancel();
    };

    $scope.doRecord = function () {
        spaRtcSvc.doRecord();
    };

    $scope.doPlay = function () {
        spaRtcSvc.doPlay();
    };

    $scope.doStop = function () {
        spaRtcSvc.doStop();
    };

    $scope.doRewind = function () {
        spaRtcSvc.doRewind();
    };

    $scope.doFastForward = function () {
        spaRtcSvc.doFastForward();
    };

    $scope.doBeginning = function () {
        spaRtcSvc.doBeginning();
    };

    $scope.doEnd = function () {
        spaRtcSvc.doEnd();
    };

    //  Broadcast Handler(s)
    $scope.$on('rtcChanged', function (event, data) {
        $scope.$apply();
    });

    $scope.$on('rtcSoundPosChanged', function (event, data) {        
        //  must be less than or equal to sound length
        if (data > RecordPlayback.soundPosition)
            return;

        var newSoundPosition = data;

        $('#ex1').slider('setValue', newSoundPosition * 1);
    });

    $scope.$on('rtcTimeDisplay', function (event, data) {
        $scope.recordingTimeDisplay = data;
        $scope.$apply();
    });

    $scope.$on('rtcSoundLenChanged', function (event, data) {        
        var newMax = (data == 0) ? 1 : data;
        
        $('#ex1').slider('setAttribute', 'max', newMax * 1);
    });

    $scope.$on('rtcVolumeChanged', function (event, data) {       
        var newProgressSuccess = data + '%';

        $('#vuMeter').css('width', newProgressSuccess);
    });

    //  Initialization Code
    (function () {
        
        spaRtcSvc.initialize();
        //  perform useful work here
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

        $('#ex2').slider('setValue', RecordPlayback.globalWaveVolume);

        $('#ex2').slider('setAttribute', 'max', 100);

        $('#ex2').on('slide', function (slideEvt) {
            RecordPlayback.globalWaveVolume = slideEvt.value;
            spaRtcSvc.setGlobalWaveVolume();
        });
    })()

});