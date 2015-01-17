//  recordplaybackfactory.js

app.factory('RecordPlayback', function RecordPlayback() {
        
    RecordPlayback.volume = 33;
    RecordPlayback.globalWaveVolume = 100;
    RecordPlayback.disableRecord = true;
    RecordPlayback.disablePlay = true;
    RecordPlayback.disableStop = true;
    RecordPlayback.disableRewind = true;
    RecordPlayback.disableFastForward = true;
    RecordPlayback.disableBeginning = true;
    RecordPlayback.disableEnd = true;
    RecordPlayback.recorderStatus = 'Stopped';
    RecordPlayback.soundLength = 1;
    RecordPlayback.soundPosition = 1;
    

    return RecordPlayback;
});