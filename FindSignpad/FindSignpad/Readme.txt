<Program Description>
This program is trying to find external STU signpad devices for periodically checking the device connection through the STU SDK method. 

<Environment>
Windows Desktops
.NET Framework 2.0 or higher
Wacom STU Signpad devices

<Setup and Execution>
- Install a COM library which is located under the COM folder by running Åginstall.batÅh script file with administration mode of command prompt.
- Make sure following files are located at the same directory of the Findsignpad.exe file.
Å@  productid.lst
Å@  venderid.lst
    Interop.wgssSTU.dll
    config.lst
- Run Findsignpad.exe file
- Press ÅgStartÅh button for starting the find devices.
- See a result contents output to a log.txt file which is crated at the same folder of Findsignpad.exe.

<User Parameters>
Parameter values are provided by a text file (config.lst) which is located at the same folder of an executable file. These parameters are read at the start up time by the application program and are effective during the program is running. Users can modify each values along with their demand.

Parameter Name: duration
Description: Duration of millisecond for polling STU devices are existed
Default Value: 1000

Parameter Name: showid
Description: Display Vender ID and Product ID on the result text area.
Default Value: no

Parameter Name: autostart
Description: Enable the automatic start of finding devices without clicking the start button
Default Value: no

Parameter Name: minimized
Description: Start the program with minimized window
Default Value: no

Parameter Name: logfile
Description: Specify the log file name which listed the result
Default Value: log.txt

Parameter Name: showbuildversion
Description: Display the build version number on the title bar
Default Value: yes

Parameter Name: appendlog
Description: Append the log text line to the previous log history
Default Value: yes

Parameter Name: logwhenchanged
Description: write a log when something are changed. Otherwise, write in every polling time.
Default Value: yes
