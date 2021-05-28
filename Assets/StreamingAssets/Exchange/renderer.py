import json
from os.path import dirname, abspath, join
import time


def _read(filename):
    '''Returns the contents of the given file.'''
    filename=join( dirname(abspath(__file__)), filename)
    with open(filename,'r') as file:
        return file.read()
      
def _write(filename,message):
    '''Writes the given message to the given file.'''
    filename=join( dirname(abspath(__file__)), filename)
    with open(filename,'w') as file:
        file.write(message)

def _scrub():
    '''Empties the files 'sprite.txt' and 'input.txt'.'''
    _write('sprite.txt','')
    _write('input.txt','')

def _get_input():
    '''Returns the dictionary of inputs from 'input.txt' and empties the file.'''
    raw_input = _read('input.txt')
    _write('input.txt','')
    if raw_input:
        input = json.loads(raw_input)
    else:
        input = {'key_presses': [], 'clicks': []}
    return input

def _update_screen(changes,wait=True):
    '''Write the changes from the engine to 'sprite.txt'.'''
    if changes:
        # if there are already changes in the queue, wait
        queue = _read('sprite.txt')
        while queue!='' and wait:
            time.sleep(1/100)
            queue = _read('sprite.txt')
        # write new changes
        _write('sprite.txt',changes)

_scrub()