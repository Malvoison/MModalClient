//  Filename: dictationcontextmodel.js
//  Author: Ken Watts

//  Highlander
//
var dictationContextModel = (function () {
    var instance;

    function init() {

        var dictationContextId = -1;

        return {
            createDate: '',
            patientId: -1,
            patientLastName: '',
            patientFirstName: '',
            patientSex: '',
            patientDOB: '',
            dateOfService: '',
            encounterId: -1,
            visitId: -1,
            workType: '',
            fftAuthorId: -1,
            dictationUniqueId: '',
            base64Wave: '',

            setDictationContextId: function (key) {
                //  perform useful work here
            }
        };
    };

    return {
        getInstance: function () {
            if (!instance) {
                instance = init();
            }
            return instance;
        }
    };


})();