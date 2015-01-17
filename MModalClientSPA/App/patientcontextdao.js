//  Filename:  patientcontextdao.js
//  Author:  Kenneth Watts

var patientContextDao = (function () {

    //  Constants
    const DB_NAME = 'patientcontext';
    const DB_VERSION = 2;
    const DB_STORE_NAME = 'contexts';

    var db;

    //  DB operations
    function openDb() {
        console.log('patientcontextdao::openDb...');
        var req = indexedDB.open(DB_NAME, DB_VERSION);

        req.onsuccess = function (evt) {
            db = this.result;
            console.log('patientcontextdao::openDb.onsuccess DONE');
        };

        req.onerror = function (evt) {
            console.error('patientcontextdao::openDb.onerror: ', evt.target.errorCode);
        };

        req.onupgradeneeded = function (evt) {
            console.log('patientcontextdao::openDb.onupgradeneeded');
            var db = evt.currentTarget.result;

            if (db.objectStoreNames.contains(DB_STORE_NAME)) {
                db.deleteObjectStore(DB_STORE_NAME);
            }

            var store = db.createObjectStore(
                DB_STORE_NAME, { autoIncrement: true });
        };
    }

    function getObjectStore(store_name, mode) {
        var tx = db.transaction(store_name, mode);
        return tx.objectStore(store_name);
    }

    function clearObjectStore(store_name) {
        var store = getObjectStore(store_name, 'readwrite');
        var req = store.clear();

        req.onsuccess = function (evt) {
            console.log('Store cleared');
        };

        req.onerror = function (evt) {
            console.error('clearObjectStore: ', evt.target.errorCode);
        };
    }

    //  Initialization
    openDb();

    //  CRUD
    return {

        savePatientContext: function (context) {
            //  return a new promise
            return new Promise(function (resolve, reject) {
                var store = getObjectStore(DB_STORE_NAME, 'readwrite');
                var req;

                try {
                    req = store.add(context);
                } catch (e) {
                    console.error(e.name, e.message);
                    throw e;
                }

                req.onsuccess = function (evt) {
                    console.log('savePatientContext SUCCESSFUL new key: ', evt.target.result);
                    resolve(evt.target.result);
                };

                req.onerror = function () {
                    console.error('savePatientContext error', this.error);
                    reject(Error('Promse reject: ' + this.error));
                };
            });
        },

        deletePatientContext: function (key) {
            // return a new Promise
            return new Promise(function (resolve, reject) {
                var store = getObjectStore(DB_STORE_NAME, 'readwrite');

                var req = store.get(key);
                req.onsuccess = function (evt) {
                    var record = evt.target.result;
                    console.log('record: ', record);
                    if (typeof record === 'undefined') {
                        console.error('No matching record found: ', key);
                        return;
                    }

                    req = store.delete(key);
                    req.onsuccess = function (evt) {
                        console.log('delete SUCCESSFUL for key: ', evt.target);
                        resolve('true');
                    };
                    req.onerror = function (evt) {
                        console.error('deletePatientContext:', evt.target.errorCode);
                        reject(Error(evt.target.errorCode));
                    };
                };
                req.onerror = function (evt) {
                    console.error('deletePatientContext:', evt.target.errorCode);
                };
            });
        },

        getPatientContext: function (key, success_callback) {
            //  return a new Promise
            return new Promise(function (resolve, reject) {
                var store = getObjectStore(DB_STORE_NAME, 'readwrite');
                var req = store.get(key);
                req.onsuccess = function (evt) {
                    var value = evt.target.result;
                    resolve(value);
                };
                req.onerror = function (evt) {
                    reject(Error(evt.message));
                };
            });

        }
    };
})();