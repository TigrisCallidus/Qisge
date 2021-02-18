import sys

sys.path.append(sys.path[0]+'/Data/game')

try:
    import game
except Exception as e:
    import qisge 
    qisge.print(e)
    qisge.update()

