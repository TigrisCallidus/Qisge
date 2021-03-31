# the all important import of the game engine
import qisge

# we need to keep track of time, to regulate the FPS
import time

# before loading qiskit, we put up a loading message
loading = qisge.Text('Loading Qiskit',16,2)
loading.x = 6
loading.y = 8
loading.set_font_color( (0,0,255) )
qisge.update()

# then import it (though we don't actually use it in this example game)
#from qiskit import QuantumCircuit, execute, Aer

# then hide the message
loading.width = 0
qisge.update()

# target frame rate
FPS = 30 

# load images
images = qisge.ImageList([
    'game/fly_r.png',
    'game/flap_r.png',
    'game/flap_l.png',
    'game/dive.png',
    'game/sky.png',
    ])

# load audio files
sounds = qisge.SoundList([
    'game/piano_middle_C.mp3'
])

#middle_c = qisge.Sound(0)

# set up screen
sprite = {} # sprites for each tile
# loop over all tiles (remembering screen is 28x16)
for x in range(28):
    for y in range(16):
        sprite[x,y] = qisge.Sprite(4,x=x,y=y,z=0)
        
# initialize player
player = qisge.Sprite(0,x=x,y=y,z=0)
player.x = 7
player.y = 7
player.z = 1

# get initial input
input = qisge.update()
# set the variable that determines whether the game is still running
running = True
# check the time when we begin
t0 = time.time()
# initialize the frame counter
frames = 0
while running:

    # act on input
    direction = [0,0]
    if 0 in input['key_presses']:
        # move up
        player.y += 0.1
        player.image_id = 0
    if 2 in input['key_presses']:
        # move down
        player.y -= 0.3
        player.image_id = 3
    if 1 in input['key_presses']:
        # move right
        player.x += 0.2
        player.image_id = 1
    if 3 in input['key_presses']:
        # move left
        player.x -= 0.2
        player.image_id = 2

    #if  4 in input['key_presses']:
    #    # play a note
    #    middle_c.playmode = 1

    if 7 in input['key_presses']:
        # end game
        running = False

    # update screen
    input = qisge.update()
    
    # update the number of frames that have been rendered
    frames += 1
    # and check the number that should have been rendered by now
    expected_frames = (time.time()-t0)*FPS
    # add a pause if we are ahead of time
    if frames > expected_frames:
        time.sleep(1/FPS)
    
    # uncomment the following line to print these frame numbers to screen
    #qisge.print((frames,expected_frames))

