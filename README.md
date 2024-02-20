## GeneratorAnalysis
# Steps to Run the Application
Before launching the application, create a folder named C:\BRADY, which contains both the input and output folders for XML files.

Verify the App.config file located at GeneratorAnalysis\Energy\App.config. Ensure that it contains the following keys:

InputFolder: Directory path for reading files.
OutputFolder: Directory path for creating output files.
Note: Ensure that the application has read and write access to the specified paths (C:\BRADY\Input and C:\BRADY\Output).

The application utilizes the FileSystemWatcher .NET Framework Class:

Purpose: The FileSystemWatcher class enables the application to monitor changes in a specified directory or file.
Functionality: It raises events when the specified directory or file is changed in some way (e.g., when files are created, modified, deleted, or renamed).
Usage: Subscribe to various events such as Changed, Created, Deleted, and Renamed, and respond to these events with custom logic in your application.
Supports Asynchronous Operations.
After successfully reading XML files from the input folder, the files are deleted from the input folder.

The output folder maintains a log of all output XML files generated by the system.

## Pending Tasks
Reading static values (EmissionsFactor, ValueFactor) from ReferenceData.xml. Currently, hardcoded values are used.
Structuring the project.
Covering more positive and negative scenarios related to file operations.
Implementing dependency injection, logging, and improving read and write operation performance by using async calls or parallel calls.



