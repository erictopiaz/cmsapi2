var _API = 'https://awcmgt19.azurewebsites.net/api';
_API = 'http://localhost:65214/api';
//_API = '/api';

Vue.mixin({
    created: function () {
        var myOption = this.$options.myOption;
        //if (myOption) {
        //    console.log(myOption);
        //}
        //else {
        //    window.alert('hello from global mixin');
        //}
    },
    data: function () {
        return {
            // GLOBAL VARIABLES
            _API: ''
        };
    },
    filters: {
        capitalize: function (value) {
            if (!value) return '';
            value = value.toString();
            return value.charAt(0).toUpperCase() + value.slice(1);
        },
        tailingunit: function (value) {
            if (!value) return '0次';
            value = value.toString();
            return value + '次';
        },
        currency: function (value) {
            if (!value) return '--';
            value = value.toString();
            return '¥ ' + value;
        },

        percentage: function (value, decimals) {
            if (!value) value = 0;
            if (!decimals) decimals = 0;

            value = value * 100;
            return Math.round(value * Math.pow(10, decimals)) / Math.pow(10, decimals) + '%';
        },
        obscureend: function (x, startPosition) {
            if (x.length > startPosition)
                return x.substr(0, startPosition) + "......";
            return x;
        },

        ifempty: function (value) {
            if (!value) return '--N/A--';
            return value;
        },
        dateonly: function (value) {
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
        },

        standarddate: function (value) {
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
            //var dddd = d.getFullYear() + '年' + m + '月' + y;
            var tttt = hh + ":" + mm + ":" + ss;
            return dddd + " " + tttt;
        }

    },
    methods: {
        testmethod() {
            window.alert('called from testmethod');
        },
        editaccount(item_id) {
            WeAdminEdit('编辑', '/pages/member/edit.html?id=' + item_id, 1, 800, 400);
        },
        checklogin() {
            return this.postwithauth('/Login/CheckStatus', null, function () {
                return true;
            }, function (d) {
                console.info(window.href);
                console.info(window.location);
                if (window.location.pathname != '/login.html')
                    window.location = "/login.html";

            });
        },
        logout() {
            localStorage.setItem('acow8unasvef', null);
            localStorage.setItem('acow8unasvefu', null);
            //window.alert('Logged OUT');
            if (window.location.pathname != '/login.html')
                window.location = "/login.html";

        },
        getdate(ddd) {
            var d = new Date(ddd);
            var m = d.getMonth() + 1;
            var y = d.getDate();
            if (m < 10) m = "0" + m;
            if (y < 10) y = "0" + y;
            var x = d.getFullYear() + '-' + m + '-' + y;
            return x;
        },
        getdate_computed(daystosubtractfromtoday) {
            var result = new Date(new Date().setDate(new Date().getDate() - daystosubtractfromtoday));
            return this.getdate(result.toDateString());
        },
        prettydate(time) {
            //var date = new Date((time || "").replace(/-/g, "/").replace(/[TZ]/g, " ")),
            var date = new Date((time));
            diff = (((new Date()).getTime() - date.getTime()) / 1000),
                day_diff = Math.floor(diff / 86400);
            console.info(diff);
            if (isNaN(day_diff) || day_diff < 0 || day_diff >= 31) return;

            return day_diff == 0 && (
                diff < 60 && "刚刚" || diff < 120 && "1 分钟前" || diff < 3600 && Math.floor(diff / 60) + " 分钟前" || diff < 7200 && "1 小时前" || diff < 86400 && Math.floor(diff / 3600) + " 小时前") || day_diff == 1 && "昨天" || day_diff < 7 && day_diff + " 天前" || day_diff < 31 && Math.ceil(day_diff / 7) + " 礼拜前";
        },
        postheaders() {
            var token = localStorage.getItem('acow8unasvef');
            return headers = {
                'Content-Type': 'application/json',
                "Access-Control-Allow-Origin": "*",
                'Authorization': token
            };
        },
        postwithauth(apipath, datatopost, dofunctionhere, doerrorfunction) {
            var token = localStorage.getItem('acow8unasvef');
            const headers = {
                'Content-Type': 'application/json',
                'Authorization': token
            };

            return axios.post(_API + apipath, datatopost,
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
        },
        loginusername() {
            return localStorage.getItem('acow8unasvefu');
        },
        login(username, passwd, dofunctionhere) {
            //window.alert("Logging IN " + username + " " + passwd);
            //this.postwithauth('/Login', JSON.stringify(
            //    {
            //        account: username,
            //        password: passwd
            //    })
            //    , function (response) {
            //        if (response.status == 200) {
            //            console.info('Login Successful');
            //            localStorage.setItem('acow8unasvef', response.data);
            //            localStorage.setItem('acow8unasvefu', username.toLowerCase());
            //            dofunctionhere(true);
            //        }
            //        else {
            //            console.error('Login Failed');
            //            localStorage.setItem('acow8unasvef', null);
            //            dofunctionhere(false);
            //        }
            //    }
            //    , dofunctionhere(false));
            //return;

            var jsonString = JSON.stringify({
                'account': username,
                'password': passwd
            });
            axios.post(_API + '/Login', jsonString, {
                headers: {
                    "Content-Type": "application/json",
                    "Access-Control-Allow-Origin": "*"
                }
            })
                .then(function (response) {
                    if (response.status == 200) {
                        console.info('Login Successful');
                        localStorage.setItem('acow8unasvef', response.data);
                        localStorage.setItem('acow8unasvefu', username.toLowerCase());

                        // Setting all AJAX Globally
                        $.ajaxSetup({
                            beforeSend: function (xhr) {
                                xhr.setRequestHeader("Authorization", response.data);
                            }
                        });

                        dofunctionhere(true);
                    }
                    else {
                        console.error('Login Failed');
                        localStorage.setItem('acow8unasvef', null);
                        dofunctionhere(false);
                    }
                })
                .catch(function (error) {
                    console.log(error);
                    dofunctionhere(false);
                });
        },
        getParameterByName(name, url) {
            if (!url) url = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, ' '));
        },
        ArrayToObject(arr) {
            var obj = {};
            for (var i = 0; i < arr.length; i++)
                if (arr[i].value != undefined && arr[i].value != null && arr[i].value != '')
                    obj[arr[i].name] = arr[i].value;
                else
                    obj[arr[i].name] = null;
            return obj;
        },
        toqueryparams(obj, prefix) {
            var str = [],
                p;
            for (p in obj) {
                if (obj.hasOwnProperty(p)) {
                    var k = prefix ? prefix + "[" + p + "]" : p,
                        v = obj[p];
                    str.push((v !== null && typeof v === "object") ?
                        serialize(v, k) :
                        encodeURIComponent(k) + "=" + encodeURIComponent(v));
                }
            }
            return str.join("&");
        },
        paramstoobject() {
            var search = location.search.substring(1);
            return JSON.parse('{"' + decodeURI(search).replace(/"/g, '\\"').replace(/&/g, '","').replace(/=/g, '":"') + '"}')
        }
    }
});


