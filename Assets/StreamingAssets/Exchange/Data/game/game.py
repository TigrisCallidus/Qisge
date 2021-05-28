import qisge

from microqiskit import QuantumCircuit, simulate

import math, random, time

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

# define function that gets image ids for each tile
def get_image_id(x,y):

    # set up a single qubit circuit
    qc = QuantumCircuit(1)

    # define some (x,y) dependent angles
    d = math.sqrt(x**2+y**2)
    f = [ s*math.cos(s*d*math.pi/100) for s in seed]
    tx = (f[0]*x + f[1]*y)*math.pi/7
    ty = (f[2]*x - f[3]*y)*math.pi/7
    tz = (f[4]*(x+y) + f[5]*(x-y))*math.pi/7

    # add rotations by these angles to the circuit
    qc.rx(tx,0)
    qc.rz(tz,0)
    qc.ry(ty,0)

    if x==y:
        qc.h(0)

    # simulate it to get the probability of a 0 outcome
    ket = simulate(qc,get='statevector')
    p = ket[0][0]**2 + ket[0][1]**2
    
    # scale and round the result to get an image id
    image_id = int(round(p*(terrain_types-1)))

    return image_id


# generate random seed
seed = [0.5*random.random() for _ in range(6)]

pos_x = 0
pos_y = 0

sprite = {} # sprites for each tile
# loop over all tiles (remembering screen is 28x16)
for dx in range(28):
    for dy in range(16):
        sprite[dx,dy] = qisge.Sprite(get_image_id(pos_x+dx,pos_x+dy),x=dx,y=dy,z=0)

        
def next_frame(input):
        
    global pos_x,pos_y
        
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
        for dx in range(28):
            for dy in range(16):
                sprite[dx,dy].image_id = get_image_id(pos_x+dx,pos_y+dy)