function postJSON(url, data, successFunc, errorFunc) {
    $.ajax({
        url: url,
        data: data,
        type: 'POST',
        success: function (d) {
            successFunc(d);
        },
        error: function (d) {
            errorFunc(d);
        },
        contentType: "application/json; charset=UTF-8"
    });
};

function getJSONWithoutDataSync(url, successFunc, errorFunc) {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (d) {
            successFunc(d);
        },
        error: function (e) {
            getErrorFromAjaxRequest(e);
        },
        async: false
    });
};

function getJSONWithoutData(url, successFunc, errorFunc) {
    $.ajax({
        url: url,
        type: 'GET',
        dataType: 'json',
        success: function (d) {
            successFunc(d);
        },
        error: function (e) {
            getErrorFromAjaxRequest(e);
        },
    });
};

getErrorFromAjaxRequest = function (xmlHttpRequest) {
    if (!isBlank(xmlHttpRequest.responseJSON)) {
        return xmlHttpRequest.responseJSON.Error;
    } else {
        return xmlHttpRequest.statusText;
    }
};

isBlank = function (str) {
    if (str === false) return false;
    return (!str || /^\s*$/.test(str));
};

function error(d) {
    alert(d);
}

function success(d) {
    console.log(d.Item);
}