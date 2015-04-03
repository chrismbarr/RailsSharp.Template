var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var Str = (function () {
    function Str() {
    }
    Str.endsWith = function (input, suffix) {
        return input.indexOf(suffix, input.length - suffix.length) !== -1;
    };
    Str.trim = function (str) {
        return str.replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    };
    Str.format = function (formatString) {
        var args = arguments;
        return formatString.replace(/{(\d+)}/g, function (match, number) {
            var replacementIndex = parseInt(number, 10) + 1;
            return typeof args[replacementIndex] != 'undefined' ? args[replacementIndex] : match;
        });
    };
    return Str;
})();
var DAL;
(function (DAL) {
    var BaseExternalInvoker = (function () {
        function BaseExternalInvoker() {
            this.invoke = function (event, data) {
                if (event == null)
                    throw 'event is null or undefined';
                if (data == null)
                    throw 'data is null or undefined';
                throw 'Invoker.invoke has not been implemented, or you are using the base invoker.';
            };
            this.responder = function (event, data) {
                if (event == null)
                    throw 'event is null or undefined';
                if (data == null)
                    throw 'data is null or undefined';
                throw 'Invoker.responder delegate was not provided.';
            };
        }
        return BaseExternalInvoker;
    })();
    DAL.BaseExternalInvoker = BaseExternalInvoker;
    var DataAccess = (function () {
        function DataAccess(eventRegistry /* should be the singleton instance of the EventRegistry that your application uses */, externalInvoker /* should inherit BaseExternalInvoker */) {
            var allEvents = eventRegistry.getAvailableEvents();
            var allRequests = _.filter(allEvents, function (eventName) { return Str.endsWith(eventName, 'Request'); });
            if (allRequests.length > 0) {
                eventRegistry.hook(jMess.LifeCycleEvents.AfterRaise, function (data) {
                    var event = data[0];
                    if (_.any(allRequests, function (readEvent) { return event == readEvent; })) {
                        data = data[1];
                        externalInvoker.invoke(event, data);
                    }
                });
            }
            ;
            externalInvoker.responder = function (event, data) {
                eventRegistry.raise(event, data);
            };
        }
        return DataAccess;
    })();
    DAL.DataAccess = DataAccess;
})(DAL || (DAL = {}));
var DateTime = (function () {
    function DateTime() {
    }
    DateTime.NowString = function () {
        var now = new Date();
        var hoursTwentyForFormat = now.getHours();
        var hoursTwelveFormat;
        if (hoursTwentyForFormat == 0) {
            hoursTwelveFormat = 12;
        }
        else {
            hoursTwelveFormat = hoursTwentyForFormat > 12 ? hoursTwentyForFormat - 12 : hoursTwentyForFormat;
        }
        function addZero(num) {
            return (num >= 0 && num < 10) ? "0" + num : num + "";
        }
        var dateBits = addZero(now.getMonth() + 1) + '/' + addZero(now.getDate()) + '/' + now.getFullYear();
        var timeBits = addZero(hoursTwelveFormat) + ':' + addZero(now.getMinutes());
        var todBits = now.getHours() >= 12 ? "PM" : "AM";
        var strDateTime = dateBits + ' ' + timeBits + ' ' + todBits;
        return strDateTime;
    };
    return DateTime;
})();
var SignalRExternalInvoker = (function (_super) {
    __extends(SignalRExternalInvoker, _super);
    function SignalRExternalInvoker() {
        var _this = this;
        _super.call(this);
        var intervalLoop;
        var connection = $.hubConnection();
        var queue = [];
        var hub = connection.createHubProxy('eventHub');
        hub.on('eventReceived', function () {
            var msgs = [];
            for (var _i = 0; _i < arguments.length; _i++) {
                msgs[_i - 0] = arguments[_i];
            }
            var event = msgs[0], data = msgs[1];
            writeTransportLog(' ↓ Reply ↓ ', 'Event: ' + event);
            _this.responder(event, data);
        });
        var writeConnectionLog = function (connectionState) {
            var prefix = !logConfig.SupportCustomLogs ? '' : '%c';
            var suffix = !logConfig.SupportCustomLogs ? '' : 'background: #222; color: #bada55';
            logR.custom(prefix + ' © ' + connectionState + ' © ', suffix);
        };
        var writeTransportLog = function (transportType, details) {
            var prefix = !logConfig.SupportCustomLogs ? '' : '%c';
            var suffix = !logConfig.SupportCustomLogs ? '' : 'background: #222; color: #bada55';
            logR.custom(prefix + transportType, suffix, details);
        };
        connection.stateChanged(function (change) {
            //Call the optional function if it really is a function when the state is changed
            if ($.isFunction(_this.onStateChange)) {
                //Send the change state and the SignalR connection object
                _this.onStateChange.call(_this, change, connection);
            }
            switch (change.newState) {
                case $.signalR.connectionState.reconnecting:
                    writeConnectionLog('Re-connecting');
                    stopQueue();
                    break;
                case $.signalR.connectionState.connected:
                    writeConnectionLog('Connected');
                    startQueue();
                    break;
                case $.signalR.connectionState.disconnected:
                    writeConnectionLog('Disconnected');
                    stopQueue();
                    break;
                default:
            }
        });
        connection.start();
        var stopQueue = function () {
            writeTransportLog(' ! Queue Stopped ! ', DateTime.NowString());
            clearInterval(intervalLoop);
        };
        var startQueue = function () {
            var sendRequest = function (qi) {
                if ($.signalR.connectionState.connected) {
                    writeTransportLog(' ↑ Request ↑ ', 'Event: ' + qi.Event);
                    hub.invoke('raiseEvent', qi.Event, qi.Data);
                }
                else {
                    stopQueue();
                }
            };
            var process = function () {
                for (var i = 0; i < queue.length; i++) {
                    var qi = queue.splice(0, 1)[0];
                    if (qi) {
                        sendRequest(qi);
                    }
                    else {
                        break;
                    }
                }
            };
            intervalLoop = setInterval(process, 10);
        };
        this.invoke = function (event, data) {
            queue.push({
                Event: event,
                Data: data
            });
        };
    }
    SignalRExternalInvoker.prototype.onStateChange = function (change, connection) {
    };
    return SignalRExternalInvoker;
})(DAL.BaseExternalInvoker);
var QueueItem = (function () {
    function QueueItem() {
    }
    return QueueItem;
})();
var HexUid = (function () {
    function HexUid() {
        var uid = [];
        var hexChars = '0123456789abcdef'.split('');
        for (var i = 0; i < 24; i++) {
            uid[i] = Math.floor(Math.random() * 0x10);
        }
        for (var j = 0; j < 24; j++) {
            uid[j] = hexChars[uid[j]];
        }
        this.Value = uid.join('');
    }
    return HexUid;
})();
//# sourceMappingURL=NeedsToMoveSomeday.js.map