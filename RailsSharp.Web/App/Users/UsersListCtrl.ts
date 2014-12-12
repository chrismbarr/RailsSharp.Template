interface IUsersListScope extends IRootScope {
	usersListState: UsersListPageState;
}

declare class User { }

class UsersListPageState {
	isLoading: boolean;
	users: User[];
}

class UsersListCtrl {
	constructor($scope: IUsersListScope, eventRegistry: jMess.IEventRegistry) {
		$scope.usersListState = new UsersListPageState();

		(() => {
			eventRegistry.hook(UserEvents.ListResponse, (resp: UserListResponse) => {
				$scope.$apply(() => {
					$scope.usersListState.users = resp.users;
				});
			});

			//rails# 1c) this extends to our Requests and Responses as well
			// adding a property to our request will give us a compile time warning here that we have not fulfilled the contract.
			var req: UserListRequest = {};
			eventRegistry.raise(UserEvents.ListRequest, req);
		})();
	}
}

UsersListCtrl.$inject = ['$scope', 'eventRegistry']; 