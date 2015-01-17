//  Filename: pevfactory.js

app.factory('PatientEncounterVisit', function PatientEncounterVisit() {

    //  As a note of interest, these are static properties
    PatientEncounterVisit.pevPatientId = '';
    PatientEncounterVisit.pevFirstName = 'Smith';
    PatientEncounterVisit.pevLastName = 'Johnson';
    PatientEncounterVisit.pevBirthDate = '';
    PatientEncounterVisit.pevAge = '';
    PatientEncounterVisit.pevSex = '';
    PatientEncounterVisit.pevOfficeLocation = '';
    PatientEncounterVisit.pevDateOfService = '';
    PatientEncounterVisit.pevDictationContext = '';
    PatientEncounterVisit.pevDictationJobId = '';
    PatientEncounterVisit.pevReviewRequired = '';


    return PatientEncounterVisit;
});