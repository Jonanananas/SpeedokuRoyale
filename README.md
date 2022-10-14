# Speedoku Royale | Unity Project

The project's goal is to create a robust, refined, and stylish PC game which would be easy to use. The game Speedoku Royale is a Battle Royale style game where you compete against other players in solving sudokus. In the game, whenever a set time interval elapses, the person who has solved the least sudokus is eliminated. As the game progresses the sudokus get increasingly more difficult and the last person standing wins the game. The game also features a single player mode where you solve sudokus within a time limit.

Project team: Julia Köykkä, Jonathan Methuen, Vilhelm Niemi & Sylvester Salo

In the game's frontend development, we have used Unity and Visual Studio Code.

## Singleplayer

### Installation

1. Clone the repository.
2. If you have a 64-bit Windows 7, 10 or 11 operating system, you can try the latest build by running the `"SpeedokuRoyale.exe"` in the `"LatestBuild"` folder.
3. If the build does not work on your machine, you can build the game yourself using the Unity version `2021.3.8f1` which you can find here: https://unity3d.com/get-unity/download/archive   
Here's also a direct download link for Unity 2021.3.8f1: https://unity3d.com/get-unity/download?thank-you=update&download_nid=65750&os=Win
### How to start a singleplayer game

1. Run the built game.
2. [Click the "Play" button.](GuideImages/PressPlay.png)
2. [Click the "Solo" button.](GuideImages/PressSolo.png)
2. [Click the start button.](GuideImages/ClickToStart.png)

## Multiplayer

### Installation

Requirements for installing the backend and for following the backend installation guide:
1. `Docker Desktop` installed and running
2. `Visual Studio Code` installation with `"Dev Containers"` and `"Docker"` extensions.

To play locally hosted multiplayer, you have to do the following steps:
1. Clone the project's development environment repository `"SpeedokuRoyaleServerDE"` from https://gitlab.metropolia.fi/vilhelmn/SpeedokuRoyaleServerDE
2. Clone the project's server repository `"SpeedokuRoyaleServer"` from https://gitlab.metropolia.fi/vilhelmn/SpeedokuRoyaleServer into the `"server"` folder inside the `SpeedokuRoyaleServerDE` repository.
3. Follow the installation guide in the `README.md` in the `SpeedokuRoyaleServerDE` repository to install the server and database.
4. After you have the backend installed and the server running, if you have problems connecting to the server in game (registering/logging in fails etc.) make sure that the `"baseUrl"` field's value in the `Assets/server-settings.json` file in the `SpeedokuRoyale` repository is the same as the server's base URL.
### How to start a multiplayer game

1. Run the built game.
2. [Click the "Log in"   button.](GuideImages/LogIn.png)
3. [Click the "Register" button.](GuideImages/Register.png)
4. [Input a username and password and click "Register".](GuideImages/CreateUser.png)
5. [After a successful registration, click the "Main Menu" button.](GuideImages/ReturnToMainMenu.png)
6. [Click the "Play" button.](GuideImages/PressPlay.png)
7. [Click the "Online" button.](GuideImages/Online.png)
8. [Click the start button.](GuideImages/ClickToStart.png)   
[The server will add you to a game room.](GuideImages/WaitForGame.png) At this point, you have to join the game room with two more players to fill the room and start the multiplayer game session. To do this open two more instances of the built game and repeat the steps 2 to 8 with different user credentials. After doing so, the game should start.

### How to check out a user's multiplayer game data

1. Make sure you are logged in on a user.
2. [Click the "Statistics" button.](GuideImages/Statistics.png)
3. [Click the "Profile" button.](GuideImages/Profile.png)   
[Now you should see the user's name, victories and personal best score.](GuideImages/UserData.png)