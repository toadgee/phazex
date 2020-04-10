README.TXT - Last updated September 30, 2005
Versions:
	PhazeX Command Line Client : 0.3.6
	PhazeX Server Library      : 0.3.6
	PhazeX Game Library        : 0.5.3








-------------------------
| 0 - TABLE OF CONTENTS |
-------------------------
0     TABLE OF CONTENTS
I     RELEASE NOTES
II    SYSTEM REQUIREMENTS
III   INSTALLATION / UNINSTALLATION
 IIIa Installing
 IIIb Uninstalling
IV    COMPILATION
 IVa  Compilation under Windows
 IVb  Compilation under Mono
--    UNORGANIZED
--    HOW TO PLAY PHAZEX
--    Misc













---------------------
| I - RELEASE NOTES |
---------------------
Client can now host -- command line server is gone; replaced with server
library.














----------------------------
| II - SYSTEM REQUIREMENTS |
----------------------------

Microsoft Windows PC:
	Windows 2000 / XP
	500MHz Pentium III
	64MB RAM
	.NET Framework installed OR Mono

Linux:
	Linux 2.4 or higher
	500MHz Pentium III
	64MB RAM
	Mono

Mac OSX:
	Mac OSX 10.3 Panther or higher
	500MHz G3
	128MB RAM
	Mono


For all systems, broadband internet is recommended. Slower connections result
in slower game play.

These should be adequate, but I would recommend higher just in case. Basically
as long as you can run the operating system and Mono (if necessary), you
should be fine. PhazeX shouldn't tax the resources.


Mono is the necessary platform to run the PhazeX programs on if running Mac OSX
or Linux. It can also be installed on Windows, although the Microsoft .NET 
framework is also available. Either will work, although the Windows instructions
assume you have the .NET framework installed. If a critical exception occurs,
try installing the .NET framework. It can be found at

http://download.microsoft.com

as the redistributable package, or you can install it on Windows Update.

Mono can be found at the following URL:

http://www.mono-project.com











---------------------------------------
| III - INSTALLATION / UNINSTALLATION |
---------------------------------------

IIIa - Installing
-----------------
After the package has been downloaded, extract the package as is. It will
extract to the px folder. This may be anywhere, but remember where it is
installed. You must be able to navigate to this directory either through a
shell or Windows explorer.

IIIb - Uninstalling
-------------------
To remove all of PhazeX from your system, simply delete the px folder that was
created when you extracted the zip file. Also delete the px folder. Everything
is contained in this folder.















--------------------
| IV - COMPILATION |
-------------------


IVa - Compilation under Windows
-------------------------------
The executables in the $PROGRAM$\bin\Debug and $PROGRAM\bin\Release folder
will be compiled every time the package is released.

However, if you wish to recompile it, all source code files and Visual Studio
solution files are supplied in the package. Simply open up the Solution file in
Visual Studio .NET 2003 and build all the projects. This can be done through the
batch build process.


IVb - Compilation under Mono
----------------------------
1. Make sure that Mono is installed. Make sure that the Mono directory is in
   your path. The shell scripts depend on this. To check that this works, open
   a new shell (Terminal in OSX) and type :
   mono --version
   mcs --version
   
   If it *IS* installed correctly, something like the following will appear:
   
   Mono C# compiler version 1.1.8.0 (for mono)
   Mono JIT compiler version 1.1.8.1 (etc.; for mcs)
   
   If it *IS NOT* installed correctly, something like the following will appear:
   
   -bash: mcs: No such file or directory
   -bash: mono: No such file or directory

2. Open a shell and change to the directory that you extracted PhazeX to.
   Example:
   compname:~ username$ cd ~/Desktop/px
   compname:~/Desktop/px username$ 
   
3. Start the debug compilation process by typing:
   sh ./compile.sh
   
   This will compile the three separate pieces needed. This will always compile
   properly, because I am teh awesomeness at programming!!1!

4. To compile in release mode, type:
   sh ./make_release.sh
   
   This will compile the three separate pieces needed. This turns the debug
   and tracing flags off (not recommended).





---------------
| UNORGANIZED |
---------------





-- - Non standard rules
-----------------------
Optional rules that can be played in PhazeX:
- different card points system
- skip the next available player
- wild cards can change colors
- reverse cards can be used to reverse the direction of play
- draw cards can be used to draw another card from the deck
- adjustable phazes
- color runs can be used in phazes (runs of all the same color)
- hands with more/less than 10 cards

The rules of a game can be determined before joining by using the Game Finder.



































-------------
| -- - MISC |
-------------






Server Information:
   There are two parts to the PhazeX server: a rules server and a game server.
   The rules server allows people to connect and see the rules of your game
   before they connect and join. The rules server stops running as soon as
   the game officially starts. The game server stops accepting connections
   as soon as the game officially starts. The server uses port 8819 for the game
   server and port 8820 for the rules server.

Starting the Server (simple):
   To start the server, open another terminal window and navigate to the PhazeX
   top directory (example: ~/px). Type the following to start the server:
   sh ./pxserver.sh
   
   Only one instance of the server may be run at a time.
   
   Once the server starts, you may enter commands to monitor it. Enter ? to see
   available options.
   
Starting the Server (advanced):
   To the start the server, you must run the PXServer/bin/mono/PXServer.exe
   executable through the mono interpreter. This is taken care of automatically
   by the shell script (which runs it in debug mode) in the simple section.
   However, this does not allow to add command line options.
   
   Command line options are as follows:
  
  -connport=[connection port]
    This specifies the port which will accept connections to the game.
  -rulesport=[rules port]
    This specifies the port for the rules server. 
  --version
    This displays the version and exits. 
   
   
   
   
   
   

Client Information:
   To do, because I'm lazy right now after writing most of this document.

Starting a Client (simple):
   To start a client, open a terminal window and navigate to the PhazeX top
   directory (example: ~/px). Type the following to start the client:
   sh ./pxcc.sh
   
   The client will ask you for the hostname (or IP address) to connect to,
   the port to connect to (default is 8819), and name to connect as (cannot
   be blank).
   
   Once the game connects, you hit enter when you are ready to start. The
   game will not start until all clients are ready.
   
   You may only enter commands into the client when it is your turn. Enter ?
   for help within the client.
   
   Note that you may have as many clients as the game supports.
   
Starting a Client (advanced):
   To start the client, you must run the PXCmdClient/bin/mono/PXCmdClient.exe
   executable through the mono interpreter. This is taken care of automatically
   by the shell script (which runs it in debug mode) in the simple section.
   However, this does not allow to add command line options (although you may
   edit the shell script to add in command line arguments).
   
   Command line options are as follows:
   
  -hostname=[hostname or ip address]
    The hostname or IP address to connect to
  -port=[port number]
    The port to connect to on the target server
  -name=[your name, obviously]
    The name to connect as.
  --version
    This displays the program version and exits.

   Example:
   mono PXCmdClient.exe -hostname=localhost -port=8819 -name=username   

   If any of the above are specified, the client will not ask for input for
   them. If an argument is supplied that is not known, the client will display
   this to the console.
   
   
   




Saving:
   ADVANCED USERS ONLY
   A python script is included that will automatically compile the release
   versions of the suite, zip up the entire structure, and move it to the
   save path. The filename prefix can be set in the options section of the
   Python script. The save path can also be set in the options section of
   the Python script. **Make sure that the save path exists** It will not
   be created automatically. All output from the save process will be saved
   in the save.log file.




   
   
-fin-
buzzkeystudios, 2005
