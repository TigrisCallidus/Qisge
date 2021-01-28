import qisge

#ADD YOUR CODE HERE (overwrite the existing code)
import time

dx,dy = 0.1,0.1

images = qisge.ImageList(['spiral.png'])
sprite = qisge.Sprite(0)

input = qisge.update()

running = True
while running:

    if 0 in input['key_presses']:
        sprite.x = sprite.x
        sprite.y += dy
        print(sprite.x,sprite.y)
    if 2 in input['key_presses']:
        sprite.x = sprite.x
        sprite.y -= dy
        print(sprite.x,sprite.y)
    if 1 in input['key_presses'] and 0 not in input['key_presses']:
        sprite.y = sprite.y
        sprite.x += dx
        print(sprite.x,sprite.y)
    if 3 in input['key_presses']:
        sprite.y = sprite.y
        sprite.x -= dx
        print(sprite.x,sprite.y)
    if 6 in input['key_presses']:
        running = False

    
    
    input = qisge.update()
    time.sleep(1/40)

