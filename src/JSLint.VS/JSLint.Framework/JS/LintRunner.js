var getGlobalObject = function() {
    return (function(){
        return this;
    }).call(null);
}

var lintRunner = function (linterName, dataCollector, javascript, options) {
    var success,
		data,
		strs,
        report = {},
        linterFunction = getGlobalObject()[linterName];

    success = linterFunction(javascript, options);
    data = linterFunction.data();

    if (data) {
        report.errors = data.errors;
        report.unused = data.unused;
        // only passing errors and unused because of bug in Noesis Javascript.NET
        // which errors on { '1' : true } - objects with integer fields
    }

    dataCollector.ProcessData(report);
};