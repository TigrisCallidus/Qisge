# the all important import of the game engine
import qisge

# before loading qiskit, we put up a loading message
loading = qisge.Text('Loading Qiskit',16,2)
loading.x = 6
loading.y = 8
loading.set_font_color( (0,0,255) )
qisge.update()

# then import it
# (actually we'll be using microqiskit here)
from microqiskit import QuantumCircuit, simulate

# then hide the message
loading.width = 0
qisge.update()

# we need to keep track of time, to regulate the FPS
import time

# other stuff
import random
import math


# target frame rate
FPS = 30 

# load images
images = qisge.ImageList([
    'terrain-water.png',
    'terrain-red-flower.png',
    'terrain-grass.png',
    'terrain-path.png',
    'terrain-grass.png',
    'terrain-purple-flower.png',
    'terrain-tree.png',
    ])
terrain_types = len(images)

# set initial position
pos_x = 0
pos_y = 0


# define function that gets image ids for each tile
def get_image_id(x,y):

    d = math.sqrt(x**2+y**2)

    qc = QuantumCircuit(1)

    f = [ s*math.cos(s*d*math.pi/100) for s in seed]

    tx = (f[0]*x + f[1]*y)*math.pi/7
    ty = (f[2]*x - f[3]*y)*math.pi/7
    tz = (f[4]*(x+y) + f[5]*(x-y))*math.pi/7

    qc.rx(tx,0)
    qc.rz(tz,0)
    qc.ry(ty,0)

    if x==y:
        qc.h(0)

    ket = simulate(qc,get='statevector')
    
    p = ket[0][0]**2 + ket[0][1]**2
    

    image_id = int(round(p*(terrain_types-1)))

    return image_id


# generate random seed
seed = [0.5*random.random() for _ in range(6)]


# set up screen

# loading message
loading.text = 'Creating Park'
loading.width = 16
qisge.update()

# the actual setuo
sprite = {} # sprites for each tile
# loop over all tiles (remembering screen is 28x16)
for dx in range(28):
    for dy in range(16):
        sprite[dx,dy] = qisge.Sprite(get_image_id(pos_x+dx,pos_y+dy),x=dx,y=dy,z=0)

# remove loading message
loading.width = 0

# get initial input
input = qisge.update()
# set the variable that determines whether the game is still running
running = True
# check the time when we begin
t0 = time.time()
# initialize the frame counter
frame = 0


while running:

    pressed = False
    if 0 in input['key_presses']:
        pos_y += 1
        pressed = True
    if 1 in input['key_presses']:
        pos_x += 1
        pressed = True
    if 2 in input['key_presses']:
        pos_y -= 1
        pressed = True
    if 3 in input['key_presses']:
        pos_x -= 1
        pressed = True
        
    if pressed:
        #qisge.print((frame,pressed))
        
        for dx in range(28):
            for dy in range(16):
                image_id = 0
                sprite[dx,dy].image_id = get_image_id(pos_x+dx,pos_y+dy)

    if 7 in input['key_presses']:
        # end game
        running = False

    # update screen
    input = qisge.update()
    
    # update the number of frames that have been rendered
    frame += 1
    # and check the number that should have been rendered by now
    expected_frame = (time.time()-t0)*FPS
    # add a pause if we are ahead of time
    if frame > expected_frame:
        time.sleep(1/FPS)
    
    # uncomment the following line to print these frame numbers to screen
    #qisge.print((frame,expected_frame))
