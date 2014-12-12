interface IRootScope extends ng.IScope {
	pageState: PageState;
}

class PageState {
	constructor() {
		this.isLoading = false;
		this.pathname = window.location.pathname;
	}
	isLoading: boolean;
	pathname: string;
}

class RootCtrl {
	constructor($scope: IRootScope, eventRegistry: jMess.IEventRegistry) {
		$scope.pageState = new PageState();
	}
}

RootCtrl.$inject = ['$scope', 'eventRegistry']; 