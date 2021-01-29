#ADD YOUR CODE HERE (overwrite the existing code)
#Below is a simple test example (for testing the speed of communication)

print('runs')



from os.path import dirname, abspath, join  


def read(filename):
    filename=join( dirname(abspath(__file__)), filename)
    with open(filename,'r') as file:
        return file.read()
        
def write(filename,message):
    filename=join( dirname(abspath(__file__)), filename)
    with open(filename,'w') as file:
        file.write(message)      
        


pingFile='input.txt'
pongFile='sprite.txt'

ping='ping'
pong='pong'
empty=''

count=0
maxcount=100
print('start')


while True:
    if read(pingFile)==ping:

        write(pingFile, empty)
        write(pongFile, pong)
        count=count+1
        print('ping found')
        #if count==maxcount:
        #    break

    
    
