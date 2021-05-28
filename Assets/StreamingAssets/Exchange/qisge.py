import json
from renderer import _get_input, _update_screen

FPS = 30

def _val_change(key,value,dictionary):
    '''Returns whether the given dictionary has the given value at the given key.'''
    if key in dictionary:
        val_change = dictionary[key]!=value
    else:
        val_change = True
    return val_change

def update(wait=True):
    '''Update screen and get input.'''
    _update_screen(_engine.get_changes(), wait=wait)
    return _get_input()


class _Engine():
    
    def __init__(self):
        self.image_changes = []
        self.sprite_changes = {}
        self.camera_changes = {}
        self.text_changes = {}
        self.sound_changes = []
        self.channel_changes = {}
                
    def get_changes(self):
        # make the changes dictionary
        changes = {}
        for attr in self.__dict__:
            if attr in ['sprite_changes','text_changes','channel_changes']:
                changes[attr] = list(self.__dict__[attr].values())
            else:
                changes[attr] = self.__dict__[attr]
        # empty the record of changes
        self.image_changes = []
        self.sound_changes = []
        for attr in ['sprite_changes','text_changes','channel_changes','camera_changes']:
            self.__dict__[attr] = {}
        # output the string of changes
        return json.dumps(changes)


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


class SoundList(list):
    
    def __init__(self,filenames):
        for filename in filenames:
            self.append(filename)
            
    def _record(self,sound_id,filename):
        _engine.sound_changes.append({'sound_id':sound_id,'filename':filename})
        
    def __setitem__(self,sound_id,filename):
        super().__setitem__(sound_id,filename)
        self._record(sound_id,filename)
        
    def append(self,filename):
        sound_id = len(self)
        super().append(filename)
        self._record(sound_id,filename)


class Camera():

    def __init__(self,x=0,y=0,z=0,size=8,angle=0):
        self.x = x
        self.y = y
        self.size = size
        self.angle = angle

    def __setattr__(self,name,val):
        # only do something if the value actually changes
        if _val_change(name,val,self.__dict__):
            # record the updated value for the thing that's changed
            _engine.camera_changes[name] = val
        self.__dict__[name] = val


class Sprite():
    def __init__(self,image_id,x=0,y=0,z=0,size=1,angle=0,flip_h=0,flip_v=0):
        
        self.sprite_id = len(_engine.sprite_changes)
        _engine.sprite_changes[self.sprite_id] = {}
        
        self.image_id = image_id
        self.x = x
        self.y = y
        self.z = z
        self.flip_h = flip_h
        self.flip_v = flip_v
        self.size = size
        self.angle = angle
        
    def __setattr__(self,name,val):
        # only do something if the value actually changes
        if _val_change(name,val,self.__dict__):
            if name!='sprite_id':
                # record the updated value for the thing that's changed
                if self.sprite_id not in _engine.sprite_changes:
                    _engine.sprite_changes[self.sprite_id] = {}
                _engine.sprite_changes[self.sprite_id]['sprite_id'] = self.sprite_id
                _engine.sprite_changes[self.sprite_id][name] = val
            self.__dict__[name] = val


class Sound():

    def __init__(self,sound_id,playmode=0,volume=1,pitch=1,note=0):
        self.channel_id = len(_engine.channel_changes)
        _engine.channel_changes[self.channel_id] = {}

        self.sound_id = sound_id
        self.playmode = playmode
        self.volume = volume
        self.pitch = pitch
        self.note = note

    def __setattr__(self,name,val):
        # only do something if the value actually changes
        if _val_change(name,val,self.__dict__):
            if name!='channel_id':
                # record the updated value for the thing that's changed
                if self.channel_id not in _engine.channel_changes:
                    _engine.channel_changes[self.channel_id] = {}
                _engine.channel_changes[self.channel_id]['channel_id'] = self.channel_id
                _engine.channel_changes[self.channel_id][name] = val
            self.__dict__[name] = val


class Text():
    def __init__(self,text,width,height,x=0,y=0,font_size=0,font=0,angle=0,font_color={"r":0,"g":0,"b":0,"a":255},background_color={"r":255,"g":255,"b":255,"a":255},border_color={"r":0,"g":0,"b":0,"a":255}):

        self.text_id = len(_engine.text_changes)
        _engine.text_changes[self.text_id] = {}
        
        self.text = text
        self.x = x
        self.y = y
        #self.z = z will be added one day, but not today
        self.font_size = font_size
        self.font = font
        self.width = width
        self.height = height
        self.angle = angle
        self._font_color = font_color
        self._background_color = background_color
        self._border_color = border_color

    def __setattr__(self,name,val):
        # only do something if the value actually changes
        if _val_change(name,val,self.__dict__):
            # change the value
            self.__dict__[name] = val
            # record the change
            if name!='text_id':
                if name[0]=='_':
                    name = name [1::]
                # record the updated value for the thing that's changed
                if self.text_id not in _engine.text_changes:
                    _engine.text_changes[self.text_id] = {}
                _engine.text_changes[self.text_id]['text_id'] = self.text_id
                _engine.text_changes[self.text_id][name] = val

    def _rgb2col(self,rgb):
        col= {'r':rgb[0], 'g':rgb[1], 'b':rgb[2]}
        try:
            col['a'] = rgb[3]
        except:
            col['a'] = 255
        return col

    def set_background_color(self,rgb):
        self._background_color = self._rgb2col(rgb)

    def set_font_color(self,rgb):
        self._font_color = self._rgb2col(rgb)

    def set_border_color(self,rgb):
        self._border_color = self._rgb2col(rgb)


def print(text):
    if type(text)!=str:
        text = str(text)
    fulltext = _print_buffer.text + '\n' + text
    fulltext = '\n'.join(fulltext.split('\n')[-32::])
    _print_buffer.text = fulltext
    show_print()

def show_print():
    _print_buffer.set_font_color((0,0,0,128))
    _print_buffer.set_background_color((255,255,255,128))
    _print_buffer.set_border_color((255,255,255,0))

def hide_print():
    _print_buffer.set_font_color((0,0,0,0))
    _print_buffer.set_background_color((255,255,255,0))
    _print_buffer.set_border_color((255,255,255,0))


_engine = _Engine()
camera = Camera()

_print_buffer = Text('',28,16,y=15)
hide_print()
