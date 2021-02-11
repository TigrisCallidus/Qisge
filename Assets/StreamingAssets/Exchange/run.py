import qisge

import time

x0,y0 = 5,0
d = 0.2
fps = 30

# load images
images = qisge.ImageList(['SquareRed.png','SquareBlue.png'])




sprite = {}
for x in range(16):
    for y in [0,15]:
        sprite[x,y] = qisge.Sprite(1,x=x0+x,y=y0+y,z=0)

player = qisge.Sprite(0)
player.x = x0+7
player.y = y0+7
player.z = 1

input = qisge.update()

title = qisge.Text('test',1,1)


running = True
while running:

    if 0 in input['key_presses']:   
        player.y += d
    if 2 in input['key_presses']:
        player.y -= d
    if 1 in input['key_presses']:
        player.x += d
    if 3 in input['key_presses']:
        player.x -= d
    if 4 in input['key_presses']:
        pass

    input = qisge.update()
    time.sleep(1/fps)

