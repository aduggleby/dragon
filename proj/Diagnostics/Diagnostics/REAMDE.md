Dragon.Diagnostics
==================

Collects basic information about the network connectivity of the system.


Requirements
------------

### Compile
* Visual Studio 2013

### Package
* ILMerge (http://microsoft.com/en-us/download/details.aspx?id=17630)
* 7-Zip (http://www.7-zip.org/)

### Run
* .NET Framework 4.5
* A running instance of Dragon.Diagnostics.Web


Package
-------

* Change directory to scripts
* Run: package.bat


Usage
-----

Simply execute Dragon.Diagnostics.exe.

Use following switches to disable individual modules:
* -b: Browser
* -h: Http
* -n: NetworkInterface
* -o: Operating System
* -p: Ping
* -s: SSL
* -t: TraceRoute
* -w: WebSocket

Use -h to set the location of the running instance of Dragon.Diagnostics.Web.
