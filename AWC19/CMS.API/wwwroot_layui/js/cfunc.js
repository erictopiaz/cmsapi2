
function dateonly(value) {
    if (!value) return '--';
    //if (isNaN(value)) return '--';

    var d = new Date(value);
    var m = d.getMonth() + 1;
    var y = d.getDate();
    var hh = d.getHours();
    var mm = d.getMinutes();
    var ss = d.getSeconds();
    if (m < 10) m = "0" + m;
    if (hh < 10) hh = "0" + hh;
    if (mm < 10) mm = "0" + mm;
    if (ss < 10) ss = "0" + ss;
    if (y < 10) y = "0" + y;
    var dddd = d.getFullYear() + '-' + m + '-' + y;
    var tttt = hh + ":" + mm + ":" + ss;
    return dddd;
}

function ArrayToObject(arr) {
    var obj = {};
    for (var i = 0; i < arr.length; i++)
        if (arr[i].value != undefined && arr[i].value != null && arr[i].value != '')
            obj[arr[i].name] = arr[i].value;
        else
            obj[arr[i].name] = null;
    return obj;
}

function standarddate(value) {
    try {
        if (!value) return '--';
        //if (isNaN(value)) return '--';

        var d = new Date(value);
        var m = d.getMonth() + 1;
        var y = d.getDate();
        var hh = d.getHours();
        var mm = d.getMinutes();
        var ss = d.getSeconds();
        if (m < 10) m = "0" + m;
        if (hh < 10) hh = "0" + hh;
        if (mm < 10) mm = "0" + mm;
        if (ss < 10) ss = "0" + ss;
        if (y < 10) y = "0" + y;
        var dddd = d.getFullYear() + '-' + m + '-' + y;
        var tttt = hh + ":" + mm + ":" + ss;
        return dddd + " " + tttt;

    } catch (e) {
        return '--';
    }
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function tailingunit(value) {
    if (!value) return '0次';
    value = value.toString();
    return value + '次';
}
function currency(value) {
    if (!value) return '--';
    value = value.toString();
    return '¥ ' + value;
}

function percentage(value, decimals) {
    if (!value) value = 0;
    if (!decimals) decimals = 0;

    value = value * 100;
    return Math.round(value * Math.pow(10, decimals)) / Math.pow(10, decimals) + '%';
}


function postwithauth(apipath, datatopost, dofunctionhere, doerrorfunction) {
    var token = localStorage.getItem('acow8unasvef');
    const headers = {
        'Content-Type': 'application/json',
        'Authorization': token
    };

    return axios.post(apipath, datatopost,
        { headers: headers })
        .then(dofunctionhere)
        .catch(function (error) {
            if (!apipath) {
                console.error("No APIPATH Defined");
                return false;
            }
            console.log(error);
            if (doerrorfunction(error));

        });
}



function loginusername() {
    return localStorage.getItem('acow8unasvefu');
}