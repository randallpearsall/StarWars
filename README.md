# StarWars (consumes REST API)
A console application that consumes third party StarWars REST API (https://swapi.dev/)<br/>
The application uses four different threading techniques so you can compare return value speeds.
## What you need to do before running the application.
***
1. Download the project.
2. On your local machine, locate the executable file in the project ...\bin\Release\ folder.
3. There are parameters which must be passed to the executable file, so you need to create a batch file to call the executable file with parameters.
4. Somewhere on your computer create a DOS batch file, for example, <b>StarWars.bat</b>.<br/>
   Edit the batch file and add the following lines of code.

   <b>@ECHO OFF<br />
   CD "C:\<path...>\bin\Release\"<br />
   STARWARS "The Phantom Menace" characters name<br/>
   PAUSE</b>

5. Save the file.
6. Run the file.

## What happens when you run the batch file (which calls the application).
***
When you run the batch file, it calls the application executable (StarWars.exe) with a set of parameters defined within the batch file.
The executable calls a public RESTful API developed by a third party for public use located at (https://swapi.dev/).
The REST API returns values to your computer as you will see displayed in a command window.

The executable actually calls the REST API four (4) times.
Each call is done with a different type of threading technology, so that you can see the difference between the response times as a comparison.

Call #1 uses SYNCHRONOUS threading. (Calls to the REST api are not made until the previous call has completed and returns the result).<br />
Call #2 uses ASYNCHRONOUS (THREAD) threading. (Calls to the REST api are made without waiting for previous calls to complete).<br />
Call #3 uses ASYNCHRONOUS (THREADPOOL) threading. (Calls to the REST api are made without waiting for previous calls to complete).<br />
Call #4 uses ASYNCHRONOUS (TASK) threading. (Calls to the REST api are made without waiting for previous calls to complete).<br />

You can see the difference between each of the threading technologies to get an idea of how fast each threading process takes.

## Can I supply other parameters?
***
Yes. If you want to supply other parameters to the REST API, you can do that. Go to https://swapi.dev/ for more information. You'll get the hang of it.
For example, you could edit the BAT file and make a call such as...

<b>StarWars "Return of the Jedi" starships manufacturer</b>

## My App
***
Remember, I created this application to make the calls for you (in a simplified fashion).
Their website requires you to follow their rules for making a call to return a JSON file.
I simply took it to the next level to do all the work for you in a console application.
