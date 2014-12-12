var PageState = (function () {
    function PageState() {
        this.isLoading = false;
        this.pathname = window.location.pathname;
    }
    return PageState;
})();

var RootCtrl = (function () {
    function RootCtrl($scope, eventRegistry) {
        $scope.pageState = new PageState();
    }
    return RootCtrl;
})();

RootCtrl.$inject = ['$scope', 'eventRegistry'];
//# sourceMappingURL=RootCtrl.js.map
