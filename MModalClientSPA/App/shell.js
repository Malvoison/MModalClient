//  Filename: shell.js
//  Author:  Ken Watts

define(['plugins/router'], function (router) {

    var vmRouter = router;
    
    return {
        router: vmRouter,
        activate: function () {
            outputDebugString('shell view model: activate');

            router.map([
                { route: '', moduleId: 'viewmodels/recorder', title: 'Recorder', nav: true },
                { route: 'test', moduleId: 'viewmodels/testjig', title: 'Test Jig', nav: true }
            ]).buildNavigationModel();

            return router.activate();
        }
    };
});