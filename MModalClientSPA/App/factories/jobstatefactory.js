//  Filename: jobstatefactory.js

app.factory('JobState', function JobState() {

    JobState.dictationJobId = -1;
    JobState.stat = false;
    JobState.reviewRequired = false;

    return JobState;
});