# WinApi.Demo
Simple demo project showing off capabilities of Windows API in internet connection via HTTP requests, RPC and WebSockets.
Feel free to download the source code and use as a guide in your projects.

[Click here](https://github.com/Maissae/WinApi.Demo/archive/master.zip) to download source code!

## HTTP Requests
Done in C# with `wininet` and PInvoke. Signatures are contained in `lib/WinApi.Demo.Signatures/Wininet.cs`. 
Library is loaded during runtime of the application and based on the signatures, methods are called from it.
Application has limited capabilites but it is able to do simple HTTP operations such as `GET`, `POST`, `PUT` and `DELETE`.
With application also comes simple API that can be used for testing the calls.

## WebSockets
Done in C# as well with `ws2_32` and PInvoke. You can find functions signatures in `lib/WinApi.Demo.Singatures/Ws2_32.cs`.
Just like above library is loaded during the runtime of application and methods are called based on the declared signatures.
As for application it is a simple chat that can be used over network.
It requires a server that handles a communication between all the users, it's included into projects as well.
Server can handle multiple users as for each one of them thread is created.
It'll broadcast a message to all users that are connected to it.

## RPC
TODO
