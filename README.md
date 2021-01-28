# Qisge
Playing Python Games in Unity

# Installation

Download the project.

Open it in Unity.

Add TextMeshPro (Window>PackatManager>TextMeshPro>Install)

Copy a working python environment into the Folder /Assets/StreamingAssets/.q

# How to Use

In the scene QisgeMain is a simple example (SimpleExample) which is a unityscript which simmulates python communication, use that for reference to implement a python script.

In the same scene there is a GameManager on it you can specify how the files are called, which are used for communication. 
Default value is sprite for the file with the sprite changes. input for the file with the input changes and run for the pythonfile which should be run.

In the folder StreaingAssets/Exchange are the specified files, as well as a data folder. Overwrite the run file with your own code and use the DataFolder to put sprites in
or other data needed in the game.

The run.py will automatically be started when the unity scene runs. If you want to use the pythonfile for communication instead of the unity example, just deactivate the object "SimpleExample"

# Test new stuff

The folder Assets/Testing can be used for all kinds of test scripts, since the content will be ignored and not commited.

Same with the folder Assets/StreamingAssets/PythonScripts this folder can be used for testing own pythonscripts.
