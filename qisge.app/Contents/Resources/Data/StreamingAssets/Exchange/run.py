import sys
import traceback

sys.path.append(sys.path[0]+'/Data/game')

try:
    import game
except:
    import qisge 
    qisge.print(traceback.format_exc())
    qisge.update()

