var ServerEvents = {
	OnError: "ServerOnError"
};

(() => {
	var app = angular.module('app', ['ngCookies']);

	var logger = new LogR(logConfig);
	var eventRegistry = new jMess.EventRegistry(logger);
	eventRegistry.register(ServerEvents);
	eventRegistry.register(UserEvents);
	app.factory('eventRegistry', () => { return eventRegistry; });
	
	app.controller('RootCtrl', RootCtrl);
	app.controller('UsersListCtrl', UsersListCtrl);
	
	var barker = new jMess.EventBarker(eventRegistry, logger);
	barker.startBarking();
	var dataAccess = new DAL.DataAccess(eventRegistry, new SignalRExternalInvoker());
})();