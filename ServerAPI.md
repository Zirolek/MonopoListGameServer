## MonopoListGameServer API

### /exit
- Headers: Not Requeared
- Response: `""`

### /register
- Headers:
  - user, pass
- Response: `{"ID": Id, NickName: "NickName", "Password": "Password"}`
### /login
- Headers:
  - user, pass
- Response: `{"ID": Id, NickName: "NickName", "Password": "Password"}`
### /save
- Headers: Not Requeared
- Response: Not Responsing
### /rooms
- Headers: Not Requeared
- Response: `{"ID": "RoomInit.ToJsonForLobby()"}`
- Response Example: `{"0": {"RoomName": "Gay", "IsGameStarted": "false", "CurrentPlayers": 1, "MaxPlayers": 2, "Player_0": "cat"}, "1": {"RoomName": "Gay1", "IsGameStarted": "true", "CurrentPlayers": 1, "MaxPlayers": 2, "Player_0": "mouse"}}`
### /room/new
- Headers:
  - ID, RoomName
- Response: `{"RoomCreated": true} or {"RoomCreated": false}`
### /room/lobby
- Headers:
  - RoomName or RoomID
- Response: `$"{"RoomName": "{RoomName}", "IsGameStarted": {IsGameStarted}, "CurrentPlayers": {CurrentPlayers}, "MaxPlayers": {MaxPlayers}, "Player_{I}": "{Players[I].NickName}"}`  or `null`
- Response Example: `{"RoomName": "Gay", "IsGameStarted": "false", "CurrentPlayers": 1, "MaxPlayers": 2, "Player_0": "cat"}`
### /room/state
- Headers:
  - ID, RoomName or RoomID
- Response: `$"{"PlayerWaitingId": {PlayerWaitingId}, "PlayerWaitTime": {PlayerWaitedTime}, "IsGameStarted": {IsGameStarted}, "PlayerIsBankrupt_{I}": {Players[I].Bankrupt}, "PlayerCellPosition_{I}": {Players[I].CellPosition}"`  or `null`
- Response Example: `{"PlayerWaitingId": 0, "PlayerWaitTime": 0, "IsGameStarted": "false", "PlayerIsBankrupt_0": "false", "PlayerCellPosition_0": 0}`
### /room/join
- Headers:
  - ID, RoomName or RoomID
- Response: `{"Joined": true}` or `{"Joined": false}` or `{"Joined": null}`
### /room/leave
- Headers:
  - ID, RoomName or RoomID
- Response: `{"Leaved": true}` or `{"Leaved": false}` or `{"Leaved": null}`
### /room/move
- Headers:
  - ID, RoomName or RoomID
- Response: `$"{"FirstCubeResult": {FirstCubeResult}, "SecondCubeResult": {SecondCubeResult}"}"`
- Response Example: `{"FirstCubeResult": 2, "SecondCubeResult": 0}`
