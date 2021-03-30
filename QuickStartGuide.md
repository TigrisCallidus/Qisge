# Qisge Quick Start Guide



## Getting Started

To start making your game, you’ll first need to download the [Qisge folder](https://github.com/TigrisCallidus/Qisge/archive/refs/heads/main.zip). What you do with this folder then depends on whether you are editing your game files, or trying to run your game.


### Editing your game

Use your normal filesystem browser to find the subfolder at ['Assets/StreamingAssets/Exchange/Data/game'](Assets/StreamingAssets/Exchange/Data/game). This is the folder where your game lives. It should contain all the assets you will use (such as image and sound files). It should also contain a Python file called 'game.py', which is where your game program is written. Most of the rest of this tutorial is a guide on what to write in this file.

You find an example game already there. You can just get rid of that!

### Running your game

You can just open the scene "QisgeMain" and press the play button in Unity in order to run your game.
If you want to make a build, see [QCU](https://github.com/TigrisCallidus/QCU) for reference, since Qisge is based on QCU for running python files.

If you want to try to make several different examples, or make your own example with leaving our example working, you can copy the QisgeMain scene and rename it e.g. QisgeMain2.
Open your new scene and select the "GameManager". There you find the fields "Input File", "Sprite File" and "Python Base File".
These stand for the files in Assets/StreamingAssets/Exchange with the same names. You can leave the "Input File" and "Sprite File" as is,
but you can make a copy of the "run" file and rename it e.g. run2. You can then change the "Python Base File" to run2 to link the correct file.

Then you can make a copy of the folder "game" under Assets/StreamingAssets/Exchange/Data/game and rename the new folder e.g. to game2.
Then in the python file "run2" you can change the path from sys.path.append(sys.path[0]+'/Data/game') to sys.path.append(sys.path[0]+'/Data/game2').

Save the Unity scene "QisgeMain2" and you can now work in the folder game2 on the game file and data (as described below), without interfering with the game of the original Unity Scene "QisgeMain"


If there is a problem with your game which causes it to break, you will get an error message. If this message appears on screen when running the game, then it is a problem with the Python programming of the game or the game engine. If it appears in the Unity console, it is probably due to the system used to run the game. For the latter case, definitely tell us about it!




## Writing Your Game

### Importing tools

In Python, we need to import tools that we want to use. You’ll definitely want to use `qisge`, because this is the game engine that will allow your game to run. You can import it by using the line

```
import qisge
```

Other packages you might need are:
* `numpy`: used for mathematical functions;
* `time`: used to measure how long things take or create pauses;
* `random`: randomness;
* `PIL`: manipulating images.

You’ll also want to import Qiskit in some form, because using quantum computation with Qiskit is what this game engine is all about. You can do this with

```
import qiskit
```

or just import the specific Qiskit tools you want to use with a line like this

```
from qiskit import QuantumCircuit, assemble, run Aer
```
Alternatively, you can import a tool built on top of Qiskit, such as `quantumblur`.

However you choose to import `qiskit`, note that it takes a while to do. So you might want to create a 'loading' message for your player while it loads.


### Handling inputs and outputs

Any game needs to get inputs from the player, and display outputs. This is done using the function `qisge.update()`. Specifically, whoever you use a line such as

```
input = qisge.update()
```

It will act upon all the changes you’ve made to sprites, text and sounds (see sections below for how to do this). It also looks at what inputs have been given since the last update and returns them as a dictionary, here called `input`.

For key presses use `input[‘key_presses’]`. This gives a list of the keys that have been pressed, each represented by a number:
* ** list the numbers**


Your game will need to act on the button presses from

### Displaying images

Suppose you have a couple of images, 'player.png' and 'enemy.png' in your 'game' folder, and you want to make them appear on screen. First you need to load them by creating an `ImageList`. You can call this list whatever you like, but here we’ll call it `images`.

```
images = qisge.ImageList(['game/player.png','game/enemy.png'])
```

Lists in Python are indexed from 0. This means that the 'game/player.png' is at position 0 in this list, and 'game/enemy.png' is at position 1. If we loaded another image it would be at position 2, and so on.

Now you have loaded the images, you can create sprite objects with them. This is done with `qisge.Sprite(image_id)`.

The `image_id` that needs to be provided here is the index of the desired image in the `ImageList`. For the image list we defined above, that means we should use a 0 to use 'game/player.png' and a 1 for 'game/enemy.png'. A sprite for the player can therefore be defined as follows

```
player = qisge.Sprite(0)
```

And a couple of sprites for enemies could be defined as

```
enemy_a = qisge.Sprite(1)
enemy_b = qisge.Sprite(1)
```

The sprite objects have various attributes which affect how they are displayed. The most important two are `x` and `y`, which define the position on screen. The screen is 28x16 blocks in size. So to put the player in the middle of the screen we can write

```
player.x = 14
player.y = 16
```

There is also a `z` position, which determines the order in which sprites are drawn when they are at the same position (x,y). All these values can be integers or floats.

You can also change the image associated with a sprite using the attribute `image_id`. So if your player becomes an enemy, for example, you can change the image with

```
player.image_id = 1
```

Note that your sprite will only appear when you next call `qisge.update()`. Similarly, changes will appear only when you next call `qisge.update()`.


### Displaying text

To define a block of text use `qisge.Text(text,width,height)`, where `text` is the text to display, and `width` and `height` are the width and height in cells. These can be changed at any time using the attributes `text`, `width` and `height`. A block of text also has attributes `x`, `y` and `z`, use for positions in the same way as for sprites.

For example, some text to tell the player that Qiskit is loading can be defined as

```
loading = qisge.Text('Loading Qiskit',16,2)
loading.x = 6
loading.y = 8
```

Once Qiskit has loaded, the text can be hidden by setting its width to zero

```
loading.width = 0
```

Note that `qisge.update()` must be called at each point, to display the updates.

The font and the border of the text box are black by default, and the background is black. These can be changed with the methods `set_font_color(rgb)`, set_background_color(rgb)` and set_border_color(rgb)`. The desired colour `rub` should be given as an RGB tuple. For example, to make the loading text blue use

```
loading.set_font_color( (0,255,0) )
```

### Playing sounds

**add details on this**
