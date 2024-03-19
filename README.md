[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/ferru97)

# Rai-Net Access Battlers
This is the official repository of Rai-Net Access Battlers, the digital version of the famous board game downloadable from [www.rainetdigital.com](https://www.rainetdigital.com/), and available for Android, Windows, and Linux

The project is composed of two main part: 
- The game source code located in *Game/*
- The multiplayer server located in *Server/*

The game is currently developed using *Unity 2020.3.2f1*

For any information and support join our [Discord Channel](https://discord.com/invite/f22dhpu)!

## Multiplayer server architecture
The multiplayer is implemented using real-time client-server communication via WebSockets using *Socket.IO* available with *Node.js*.
The backend implements four services:

-  **HTTP server** that exposes a single end-point */info* used by the application to retrieve up-to-date information such as the latest version and custom message to show to the user. The source code of the server is located in *Server/HTTPserver*
- **MongoDB server** used to store persistent data about users and matches information. This server is implemented with a docker container using the latest MongoDB image
- **Mongo Express server** an optional service used to get quick access to the MongoDB database. For more info about Mongo Express click [HERE](https://github.com/mongo-express/mongo-express). Also, this server is implemented with a docker container using the latest Mongo Express image
- **Matchmaking Server**, the actual service that provides matchmaking functionality using real-time client-server communication via WebSockets using [Socket.IO](http://socket.io/).

At run-time, each one of these services is dockerized and run via *docker-compose*

## Start a local multiplayer server
Thanks to docker containers and docker-compose starting a local multiplayer server is pretty simple, follow these steps:
1. (Optional) Change the credentials information on the  **.env** file
2. Open the game project with Unity and open the script **NetInfo.cs** located in *Assets/Script/NetInfo.cs*. Here change the value of the variables  `HTTP_SERVER_INFO` and `MATCHMAKING_SERVER` replacing *192.168.1.20* with your local machine IP
3. Compile the game and generate the build for your device
4. Open a terminal in *Server/* and run `docker-compose up -d --build` to instantiate all the services

### Important!
The client game application and the server must have the same game version otherwise the client connection will be refused. To set the game version on the server set the *GAME_VERSION* variable on the *Server/.env* file. To set the game version of the client game application set the public variable *Version* of the *Main_Menu* gameobject.

## Contributions
Feel free to contribute to this project!
The main improvements concern:

- Implement an efficient AI for single player
- Clean and comment the code
- General improvements

Open a PL on the *DEV* branch to propose a change or open an Issue to suggest a change, report a bug, or discuss a feature

## Donation
If you like this project, you can give me a cup of coffee :) 

## License
The content of this project itself is licensed under the [GNU General Public License v3.0](https://www.gnu.org/licenses/gpl-3.0.html)
