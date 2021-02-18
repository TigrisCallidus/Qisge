import qisge

import time
import random


Lx,Ly = 28,16 # screen size in tiles
Mx,My = 10,10 # mountain size in screens
FPS = 30 # target frame rate
P = 0.9 # cutoff between background and foreground rocks

# randomly choose a value for all tiles
map = [ [ random.random() for x in range(Mx*Lx)] for y in range(My*Ly)  ]
# then enforce hard boundaries
for x in range(Mx*Lx):
    for y in [0,My*Ly-1]:
        map[y][x] = 1
for y in range(My*Ly):
    for x in [0,Mx*Lx-1]:
        map[y][x] = 1     

# gets the map value at the given position
def mget(x,y):
    return map[int(y)][int(x)]

# determines whether there is a collision with the map when the player is at a given position
def collide(x,y):
    not_collision = True
    eps = 0.2
    for xx,yy in [(eps,eps),(eps,1-eps),(1-eps,eps),(1-eps,1-eps)]:
        not_collision = not_collision and mget(x+xx,y+yy)<P
    return not not_collision


class Player():
    def __init__(self,img,sx=0.15,sy=0.075,g=1,j=4,j_frames=12):
        self.sprite = qisge.Sprite(img) 
        self.x = 0
        self.y = 0
        self.z = 0
        self.sx = sx
        self.sy = sy
        self.g = g
        self.j = j
        self.j_frames = j_frames
        self.impulse = 0
        self.d = 1
    def jump(self):
        if self.impulse==0:
            self.impulse = self.j_frames
    def update(self,direction=[0,0]):
        
        on_ground = collide(self.x,self.y-self.sy)
        diving = direction[1]<0

        # if not pushed in x direction...
        if direction[0]==0:
            # ...or sitting on the ground or diving...
            if not on_ground and not diving:
                # ...add a glide
                direction[0] += self.d/2
        else:
            # when pushed, change direction faced
            self.d = 1-2*(direction[0]<0)
            # if sitting on the ground, don't move
            if on_ground:
                direction[0] = 0
        # add upwards direction for jump
        if self.impulse>0:
            direction[1] += self.j
            self.impulse -= 1
        # and downwards for gravity
        direction[1] -= self.g
        # now do the move
        new_x = self.x+self.sx*direction[0]
        if not collide(new_x,self.y):
            self.x = new_x
        new_y = self.y+self.sy*direction[1]
        if not collide(self.x,new_y):
            self.y = new_y
        
        self.sprite.x = self.x%Lx
        self.sprite.y = self.y%Ly
        self.sprite.z = self.z
        if not diving:
            if self.impulse>0:
                img = 6
            elif on_ground:
                img = 4
            else:
                img = 2
            img += int(self.d==-1)
        else:
            img = 8
        self.sprite.image_id = img    


# load images
images = qisge.ImageList([
    'game/backrock.png',
    'game/frontrock.png',
    'game/fly_r.png',
    'game/fly_l.png',
    'game/walk_r.png',
    'game/walk_l.png',
    'game/flap_r.png',
    'game/flap_l.png',
    'game/dive.png'
    ])

# set up screen
sx,sy = 0,0 # screen coords
sprite = {} # sprites for each tile
for x in range(Lx):
    for y in range(Ly):
        m = mget(x,y)
        sprite[x,y] = qisge.Sprite(int(m>P),x=x,y=y,z=0)

# initialize player
player = Player(1)
player.x = 7
player.y = 7
player.z = 1
player.update()


running = True
paused = False
input = qisge.update()
t0 = time.time()
while running:

    if 7 in input['key_presses']:
        paused = not paused

    if not paused:

        # act on input
        direction = [0,0]
        if 2 in input['key_presses']:
            direction[1] = -1
        if 1 in input['key_presses'] and 0 not in input['key_presses']:
            direction[0] = 1
        if 3 in input['key_presses']:
            direction[0] = -1
        if 4 in input['key_presses']:
            player.jump() 

        # update player
        player.update(direction)

    # determine new screen coords
    nx = int(player.x/Lx)
    ny = int(player.y/Ly)


    # if it is a new screen, redraw the tiles
    if sx!=nx or sy!=ny:
        sx,sy = nx,ny

        for x in range(Lx):
            for y in range(Ly):
                m = mget(sx*Lx+x,sy*Ly+y)
                sprite[x,y].image_id = int(m>P)

    # update screen
    input = qisge.update()

    # pause to get a frame rate of FPS
    t = time.time()
    time.sleep(max(0,1/FPS - (t-t0) ))
    t0 = t

