## Start command
- {bind http://ip:port} {path to configs}
- ex. http://127.0.0.1:8080/ ~/Sources/C#/MonopoListGameServer/bin/Debug/net6.0/MonopoListGameServer/

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
### /room/new
- Headers:
  - ID, RoomName
- Response: `{"RoomCreated": true} or {"RoomCreated": false}`
### /room/lobby
- Headers:
  - RoomName or RoomID
- Response: `$"{"RoomName": "{RoomName}", "IsGameStarted": {IsGameStarted}, "CurrentPlayers": {CurrentPlayers}, "MaxPlayers": {MaxPlayers}, "Player_{I}": "{Players[I].NickName}"}`  or `null`
### /room/state
- Headers:
  - ID, RoomName or RoomID
- Response: `$"{"PlayerWaitingId": {PlayerWaitingId}, "PlayerWaitTime": {PlayerWaitedTime}, "IsGameStarted": {IsGameStarted}, "PlayerIsBankrupt_{I}": {Players[I].Bankrupt}, "PlayerCellPosition_{I}": {Players[I].CellPosition}"`  or `null`
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

Client: https://github.com/baHHbaSh/monopoLIST
