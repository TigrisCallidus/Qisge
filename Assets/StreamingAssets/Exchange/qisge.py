import json
from os.path import dirname, abspath, join

def _read(filename):
    filename=join( dirname(abspath(__file__)), filename)
    with open(filename,'r') as file:
        return file.read()
        
def _write(filename,message):
    filename=join( dirname(abspath(__file__)), filename)
    with open(filename,'w') as file:
        file.write(message)

def _scrub():     
    _write('sprite.txt','')
    _write('input.txt','')

def _get_input():
    raw_input = _read('input.txt')
    _write('input.txt','')
    if raw_input:
        input = eval(raw_input)
    else:
        input = {'key_presses': [], 'clicks': []}
    return input

def _update_screen():
    changes = _engine.get_changes()
    if changes:
        _write('sprite.txt',changes)

def update():
    _update_screen()
    return _get_input()

class _Engine():
    
    def __init__(self):
        
        self.image_changes = []
        self.sprite_changes = []
                
    def get_changes(self):
                
        changes = {'image_changes':self.image_changes,'sprite_changes':[]}
        for sprite_id, change in enumerate(self.sprite_changes):
            if change:
                change['sprite_id'] = sprite_id
                changes['sprite_changes'].append(change)
    
        self.sprite_changes = [{} for _ in range(len(self.sprite_changes))]
        self.image_changes = []

        if changes!={"image_changes": [], "sprite_changes": []}:
            return json.dumps(changes)
        else:
            return ''

class ImageList(list):
    
    def __init__(self,filenames):
        for filename in filenames:
            self.append(filename)
            
    def _record(self,image_id,filename):
        _engine.image_changes.append({'image_id':image_id,'filename':filename})
        
    def __setitem__(self,image_id,filename):
        super().__setitem__(image_id,filename)
        self._record(image_id,filename)
        
    def append(self,filename):
        image_id = len(self)
        super().append(filename)
        self._record(image_id,filename)
        
        
class Sprite():
    def __init__(self,image_id):
        
        self._sprite_id = len(_engine.sprite_changes)
        _engine.sprite_changes.append({})
        
        self.image_id = image_id
        self.x = 0
        self.y = 0
        self.z = 0
        self.size = 1
        self.angle = 0
        
    def __setattr__(self,name,val):
        # only do something if the value actually changes
        if name in self.__dict__:
            val_change = self.__dict__[name]!=val
        else:
            val_change = True
        if val_change:
            if name!='_sprite_id':
                # record all values whenever something changes (remove once issue is fixed on Unity side)
                for attr in self.__dict__:
                    _engine.sprite_changes[self._sprite_id][attr] = self.__dict__[attr]
                # record the updated value for the thing that's changed
                _engine.sprite_changes[self._sprite_id][name] = val
            self.__dict__[name] = val
    

_engine = _Engine()
_scrub()
