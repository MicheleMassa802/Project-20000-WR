# Virtual Batting Cage

Making use of Unity to simulate a real batting cage environment via some funky image analysis, Project 20000 WR captures the motion of a user’s “batting object” to position their in-game bat at the desired position over the plate, and detects the player's swinging motion (via the calculated speed of the batting object between frames) to trigger an in-game swing, resulting in an interactive batting-cage-like experience.

The inspiration for this project is Wii Sports baseball, and I can only wish I can get to such a good product given the restriction of not using sensors or a controller in general. After a base implementation is done (If you are reading this, a base implementation is indeed done! Check below for more info!), I would like to push this project into a VR setting.

# Demo of the Game:

If you just want to take a quick look over what the game is like, you can watch a demo of the core functionality here:
Demo Video! [TODO]

# Specifications (nerd stuff)

The idea for this project came up due to the fact that I have experience using OpenCV in Python, and I love making games in Unity.
So I wanted to find a way to combine these two in order to get an interesting game related to one of my non-coding interests, baseball.

Via OpenCV, I detect the two red components of my bat object in real-time, get their midpoint, and send that off as the bat's position to be transformed into in-game coordinates to move the bat around.
The tricky thing is that my OpenCV scripts work on Python, and the Unity project is clearly done in C#, and while it would be nice to buy the Unity OpenCV or Python scripting packages, they are worth money, which I did not want to spend. There is a reason why I'm using a piece of cardboard as my bat ¯\_(ツ)_/¯

So as any good CS student, I engineered my way through it.

What's a universal language both languages can speak to each other in? Bytes!
How can I send bytes between these languages in a frame-by-frame manner? A local server!
Does this not introduce a delay? Yes, but that delay is dominated by the delay arising from the real-time image processing, so it felt like a worthwhile trade! A couple frames for a functioning project!

And so the "BatTM" (not really a trademark) option of the game came into life, allowing you to use your recognizable bat object to swing at balls in the baseball field.

There are a bunch of options inside the game for adjusting pitching, and lots of UI components designed to help the user get comfortable like the strike zone overlay while batting or the in-game mini-cameras at the bottom right of your screen to get a better view of your batting and hit balls. Not to mention the robust event detection system that each ball goes through when trying to identify what happened with a ball thrown by the pitcher (was it a ball, a strike, a foul, an out, a hit, a homerun?), the system does its best within Unity's limitations to detect these events properly (as sometimes balls can be going at such high velocities when fouled off that they clip through the field without detecting collisions).

### Really Nerd Stuff:

Oh so you want an even deeper scoop? 

Then how about this function right here! 
![image](https://github.com/user-attachments/assets/af20da49-4487-40a6-9839-f89e3f56d6f3)
This is my own function for powering up the ball after it is hit by the bat, if you invert it over the X axis, X represents the distance from the place the bat collided with the ball from the "sweetspot"/"barrel" of the bat, such that the greater the distance from this "sweetspot", the weaker the contact with the ball will be.

This combined with the in-game gravity modifier system for the balls (where the gravity of balls changes from the moment they are thrown by the pitcher/hit by the bat/touch the ground), makes it so that the weight and trajectory of the ball feel both more rewarding and realistic, given that our 2D data from the bat's position can't really communicate anything special about our batting object's speed.

Another math-heavy process is the pitching, which based on a set of parameters the user specifies when they step into the box, is in charge of calculating the final position of the ball on the strike zone (or slightly outside). This system has to come up with the set of forces and directions to be applied to the ball at different stages as it exits the pitcher's hands in order to get to its target location, while obeying the speed parameter as input by the user.

# Playing the Game:

**Note**: for using BatTM, you will need one of the versions below that support the BatTM usage along with a BatTM-resembling object (with two distinguishable red blobs), and a camera. I personally used my phone and IvCam (setup video not made by me can be found [here](https://youtu.be/3-_pIos5n8s?si=Rn0dvIi_Lp_yCKb7)) throughout the development of this project, so that might work even better than a fancier setup!

### Full Version: 
A full MVP version of the game can be found here, this contains the executable file which you can make via Unity builds, which is very comfortable to play, works well overall apart from a couple of Unity's peculiarities (if found, just quit the game and restart!), and supports both M&K as well as BatTM inputs for the game.

Download the zip with the executable for the [Full version here!](https://drive.google.com/file/d/184KPslJSrhbWwMs26vzR9KBaft7xnK5c/view?usp=sharing)

### Mouse & Keyboard (no BatTM support):
A solely M&K MVP version of the game can be found here, this contains the executable file which you can make via Unity builds, which is very comfortable to play and works really well, but it only supports mouse and keyboard inputs (no bat object or visual computing done in this version) as Unity, as the calling of Python scripts is a hard thing to set up correctly with Unity builds.

This leaves you with a fun M&K game, but that's not all that we are here for!

Download the zip with the executable for the [M&K version here!](https://drive.google.com/file/d/18tkzC7uIQbzmg5BNAsbxrCLv1-sWnnmf/view?usp=sharing)
- Selecting the "BatTM" option here results in nothing, you'll be locked onto a batting box without the Python scripting going on, so you won't have control over the bat. Simply press the locking button ("L") again to unlock yourself from the batter's box.

### In-Editor Version:
A full MVP version of the game can be found here, which includes the BatTM experience.
This comes in the form of a Unity Asset Package with all the scripts, scenes, and setup you need. 

The extra setup you would need to go through then should be:
- Create a new Unity 3D project
- Have a Python interpreter installed (any version of Python 3.8+ should work!)
- Have a Python environment with OpenCV installed (I did mine via Pip).
- Import the asset package via: Assets > Import Package > Custom Package > Select the package installed via the link below!
- Import the Input System unity package via: Window > Package Manager > Go to Unity Registry > Search for and install the "Input System" package > If a warning pops up telling you to "Enable the backends" and "restart the editor", click "Yes".
- Open the "Menu" scene by double-clicking and adding it to the build settings scenes, then do the same with the "Main" scene. Then you can run the game from the "Menu" scene.
After all of that, you should be able to play the full version of the game, with the added bonus of being able to add extra behaviors you may want into it!
Just remember to credit me as the original creator for the base assets and scripting, as all of the files found under this project are copyrighted under copyright (c) 2024 Michele Massa.

Download the asset package to import for the [Unity Editor version here!](https://drive.google.com/file/d/1i1_l2raRQTTNSkAD3JfpcWnaTGZbrDbR/view?usp=sharing)

Note: I use Unity 2022.3.10f1 for the development of this project, so that would be the most stable version you could get for developing on top of my work!

# Controls:

The main controls for the game revolve around using your mouse and keyboard and the BatTM (or a combination of these) to move around, pick where to look at while batting, and perform the batting. Additionally, there is controller support for movement-related actions, but you won't be able to bat this way for the moment being (I had to limit scope somewhere, but I might fully i implement controller support at some point).

You can follow the following diagrams for further reference: 
![P20000WR-Controllers](https://github.com/user-attachments/assets/7fefe4df-7a02-4239-9da1-764d12cb89b4)
![P20000WR-Controllers (1)](https://github.com/user-attachments/assets/a55ff048-7ed1-4ad3-bc82-ba39ba9a2528)
![P20000WR-Controllers (2)](https://github.com/user-attachments/assets/388ed4e4-d455-4f9f-b66b-9f263e4a46ae)
![P20000WR-Controllers (3)](https://github.com/user-attachments/assets/3430ced1-4624-4410-90da-6a9453b7d7de)
![P20000WR-Controllers (4)](https://github.com/user-attachments/assets/b42b5d7c-38e0-4da3-b90e-4aa56795976e)

# Exitlude

Well, this has been Michele, hope this was an enjoyable repo and project to look over!
If you have any questions or would like to contact me for any reason, please reach out [here](https://www.linkedin.com/in/michele-massa--woohoo-this-is-my-profile/) via LinkedIn!
