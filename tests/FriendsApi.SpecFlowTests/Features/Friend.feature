Feature: FriendFeature

@write
Scenario: Friends Query and Crud
	Given an http client
	#And A valid bearer token in the authorization header
	
	# GET  /friends -> [] > 0
	When I send a friends request 
	Then I receive a friends response
	And The response contains friends
	
	# POST /friends -> { id > 0, name.length > 0}
	When I send a create friend request
	Then I receive a new friend response
	And the new friend response is valid

	# GET /friends/{newId} -> {id == new.id, name == new.name}
	When I send a get by id friend request using the created one
	Then I receive a valid friend response
	And The friend is equal to the created one

	# PUT /friends/{newId} -> 204
	When I Send an update friend request using the created one
	Then I receive a success nocontent code
	
	# GET /friends/{newId} -> {name == updated.name}
	When I send a get by id friend request using the created one
	Then I receive a valid friend response
	And The friend is equal to the updated one

	# DELETE /friends/{newId} -> 204
	When I Send a delete friend request using the created one
	Then I receive a success nocontent code

	# GET /friends/{newId} -> 404
	When I send a get by id friend request using the deleted one
	Then I receive not found response	