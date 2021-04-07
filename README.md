# Qisge
Playing Python Games in Unity

# Installation

You can either Download the project. ([Qisge folder](https://github.com/TigrisCallidus/Qisge/archive/refs/heads/main.zip)) 
And then open it in Unity.

Alternatively you can make open a (new) Unity project and download and import the [unity package](https://github.com/TigrisCallidus/Qisge/blob/main/MainUnityPackage.unitypackage) 

Add TextMeshPro to your Unity Project if it does not include it already (Window>PackatManager>TextMeshPro>Install)

Copy a working python environment into the Folder '/Assets/StreamingAssets/.q'. This folder also contains a 'requirements.txt' that can be used to set up a suggested environment. The setup for this works the same as in the [QCU](https://github.com/TigrisCallidus/QCU) project.

# How to Use

In the scene "Simple Unity Example" is a simple example (SimpleExample) which is a Unity script which simmulates python communication, you can use that for reference to implement a python script.

In the scene "QisgeMain" we have already implemented a working python example.

In the same scene there is a GameManager on it you can specify how the files are called, which are used for communication. 
The default file name is "sprite" for the file with the sprite changes (and all other changes from the python side). "input" is the default name for the file with the input changes and "run" for the pythonfile which should be run by Unity. (These are also the names which are used by our python example which is in the project)

In the folder StreaingAssets/Exchange are the specified files, as well as a data folder. 
If you want to use your own code you can overwrite the game file which the run file links to with your own code and use the DataFolder to put sprites in or other data you need in the game.

However, we would advice to take a look at the current python example and the [Quickstart Guide](https://github.com/TigrisCallidus/Qisge/blob/main/QuickStartGuide.md) where this is described more in detail.

The run.py will automatically be started when the unity scene runs by the GameManager. You can deactivate this by deactivating "Run Python File" on the GameManager.

If python is used to read the input.txt you have to make sure to replace the file content with an empty string (this is used to make sure that the input was read).
The same way unity will overwrite the "sprite.txt" file with an empty file, so you can check that in python to know if the changes were actually read by unity.

# Test new stuff

The folder Assets/Testing can be used for all kinds of test scripts, since the content will be ignored and not commited.

Same with the folder Assets/StreamingAssets/PythonScripts this folder can be used for testing own pythonscripts.
