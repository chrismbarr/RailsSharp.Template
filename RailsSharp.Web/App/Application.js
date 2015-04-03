var ServerEvents = {
    OnError: "ServerOnError"
};
var SignalRConnectionEvents = {
    InitialConnection: "InitialConnection",
    ConnectionLost: "ConnectionLost",
    AttemptingReconnect: "AttemptingReconnect",
    ConnectionReestablished: "ConnectionReestablished"
};
(function () {
    var app = angular.module('app', ['ngCookies']);
    var logger = new LogR(logConfig);
    var eventRegistry = new jMess.EventRegistry(logger);
    eventRegistry.register(ServerEvents);
    eventRegistry.register(UserEvents);
    app.factory('eventRegistry', function () {
        return eventRegistry;
    });
    app.controller('RootCtrl', RootCtrl);
    app.controller('UsersListCtrl', UsersListCtrl);
    var barker = new jMess.EventBarker(eventRegistry, logger);
    barker.startBarking();
    var signalRExtInvoker = new SignalRExternalInvoker();
    var dataAccess = new DAL.DataAccess(eventRegistry, signalRExtInvoker);
    function signalRStateName(state) {
        //POSSIBLE STATES: { connecting: 0, connected: 1, reconnecting: 2, disconnected: 4 }
        if (state === 4) {
            //there are 4 states, but there is no state #3, only 0,1,2, and 4
            //So if state #4 was passed in, we want the name of the 3rd one.
            state = 3;
        }
        return Object.keys($.signalR.connectionState)[state];
    }
    var previouslyConnected = false;
    signalRExtInvoker.onStateChange = function (change, connection) {
        logger.custom("SignalR state changed from " + signalRStateName(change.oldState) + " (" + change.oldState + ")" + " to " + signalRStateName(change.newState) + " (" + change.newState + ")", change);
        var eventResponse = {
            connection: connection
        };
        if (change.newState === $.signalR.connectionState.disconnected) {
            //Disconnected
            eventRegistry.raise(SignalRConnectionEvents.ConnectionLost, eventResponse);
        }
        else if (change.newState === $.signalR.connectionState.reconnecting) {
            //Attempting to reconnect
            eventRegistry.raise(SignalRConnectionEvents.AttemptingReconnect, eventResponse);
        }
        else if (previouslyConnected && change.newState === $.signalR.connectionState.connected) {
            //Connected when previously disconnected or reconnecting
            eventRegistry.raise(SignalRConnectionEvents.ConnectionReestablished, eventResponse);
        }
        else if (!previouslyConnected && change.newState === $.signalR.connectionState.connected) {
            //First Connection
            previouslyConnected = true;
            eventRegistry.raise(SignalRConnectionEvents.InitialConnection, eventResponse);
        }
    };
})();
//# sourceMappingURL=Application.js.map