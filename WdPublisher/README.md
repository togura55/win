# WdPublisher
## Description
Socket client application for reading raw data of WILL devices and transferring stroke packets to WdBroker  
## Build Instruction
1. Install libraries of WILL Ink 2.1, WILL Devices 1.0
2. Load sln by Visual Studio 2017   
3. Build it   
## Functions/Features
- Handling PHU-111 over USB interface  
- Accessing WdBroker's TCP/IP Socket communication
- Accepting the app configuration by WdController on BLE
## Environment
UWP platforms  
## License
xxx  
## Author(s)
* Tsuyoshi Ogura (togura55@gmail.com)  
## ToDo/Known Issues
*   Not be functional by clicking [Suspend] button  
## History
* 1.1.x  
- Export log file  
- Handling event of the device's barcode scan  
- Support getting device's firmware version  
* 1.0.15
  - Stable version for the demo use, with WdBroker 1.0.9 and WdController 1.0.2
  - debug option for executing stand-alone mode (non-IP, non-BT)
  - Since this version of program was based on the debug build, need to be pre-installed packages of Dependencies\x86\Microsoft.VCLibs.x86.Debug.14.00.appx
* 1.0.10 
 - Support communicating with WdController
 - Confirmed to be run on the Intel Stick environment
* 1.0.6 - Implemented command/response scheme
* 1.0.3 - 1st release
