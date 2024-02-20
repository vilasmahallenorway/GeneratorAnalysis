# GeneratorAnalysis

# following are the steps to run the applicaiton 

1. Please verify Appconfig below file, this file must contain 2 keys which is directory path for reading file and creating output file 
.	GeneratorAnalysis\Energy\App.config
    1) key="InputFolder" 2) key="OutputFolder"

    Note : Make sure appliaction has read and write access on specified path
.	
2. We have used FileSystemWatcher .NET Framework Class
   Purpose: The FileSystemWatcher class enables application to monitor changes in a specified directory or file.
   Functionality: It raises events when the specified directory or file is changed in some way (e.g., when files are created, modified, deleted, or renamed).
   Usage: You can subscribe to various events such as Changed, Created, Deleted, and Renamed, and respond to these events with custom logic in your application.
   FileSystemWatcher supports Asynchronous Operations
3. 


