import sys
import traceback
import time

sys.path.append(sys.path[0]+'/Data/game')

try:

    from game import qisge, next_frame

    input = qisge.update()
    frame = 0
    t0 = time.time()
    while True:

        next_frame(input)

        # update screen
        input = qisge.update()
        
        # update the number of frames that have been rendered
        frame += 1
        # and check the number that should have been rendered by now
        expected_frame = (time.time()-t0)*qisge.FPS
        # add a pause if we are ahead of time
        if frame > expected_frame:
            time.sleep(1/qisge.FPS)
            
except:
    import qisge 
    qisge.print(traceback.format_exc())
    qisge.update()

