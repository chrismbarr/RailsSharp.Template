var UsersListPageState = (function () {
    function UsersListPageState() {
    }
    return UsersListPageState;
})();

var UsersListCtrl = (function () {
    function UsersListCtrl($scope, eventRegistry) {
        $scope.usersListState = new UsersListPageState();

        (function () {
            eventRegistry.hook(UserEvents.ListResponse, function (resp) {
                $scope.$apply(function () {
                    $scope.usersListState.users = resp.users;
                });
            });

            //rails# 1c) this extends to our Requests and Responses as well
            // adding a property to our request will give us a compile time warning here that we have not fulfilled the contract.
            var req = {};
            eventRegistry.raise(UserEvents.ListRequest, req);
        })();
    }
    return UsersListCtrl;
})();

UsersListCtrl.$inject = ['$scope', 'eventRegistry'];
//# sourceMappingURL=UsersListCtrl.js.map
