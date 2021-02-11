using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public SoundObject ChanelPrefab;

    SoundObject[] channelsInScene = new SoundObject[20];

    Dictionary<int, AudioClip> usedClips = new Dictionary<int, AudioClip>();
    Dictionary<int, SoundUpdate> channels = new Dictionary<int, SoundUpdate>();


    public void Init() {

    }

    public void UpdateSounds(UpdateFile update) {
        UpdateClips(update.soundlist);
        UpdateChannels(update.sounds);
    }

    public void UpdateClips(SoundFile[] soundfiles) {
        for (int i = 0; i < soundfiles.Length; i++) {
            if (usedClips.ContainsKey(soundfiles[i].sound_id)) {
                //Update sprite
                UpdateAudioClip(soundfiles[i].sound_id, soundfiles[i].filename);
            } else {
                //add new sprite
                GenerateAudioClip(soundfiles[i].sound_id, soundfiles[i].filename);
                //UsedSprites.Add(sprites[i].image_id, GenerateSprite(sprites[i].filename));
            }
        }
    }

    public void UpdateChannels(SoundUpdate[] sounds) {
        for (int i = 0; i < sounds.Length; i++) {
            if (channels.ContainsKey(sounds[i].channel)) {
                UpdateChannelInScene(sounds[i]);
            } else {
                channels.Add(sounds[i].sound_id, sounds[i]);
                if (channelsInScene[sounds[i].channel] == null) {
                    channelsInScene[sounds[i].channel] = GenerateChannel(sounds[i]);
                } else {
                    UpdateChannelInScene(sounds[i]);
                }
            }
        }
    }

    public void GenerateAudioClip(int sound_id, string filename) {
        AudioClip clip = loadClipFromMP3(filename);
        usedClips.Add(sound_id, clip);
    }

    public void UpdateAudioClip(int sound_id, string filename) {
        //Generate new sprite
        AudioClip clip = loadClipFromMP3(filename);

        usedClips[sound_id] = clip;

    }

    //TODO IMPLEMENT!
    AudioClip loadClipFromMP3(string fileName) {

        Texture2D texture = null;
        byte[] data;

        fileName = Path.Combine(GameManager.DataFolder, fileName);

        //making sure to read existing png file
        if (File.Exists(fileName) && fileName.EndsWith(".png")) {

            //Load texture from file
            data = File.ReadAllBytes(fileName);
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be set correctly here
            texture.LoadImage(data);
        } else if (File.Exists(fileName + ".png")) {
            //Load texture from file
            data = File.ReadAllBytes(fileName + ".png");
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be set correctly here
            texture.LoadImage(data);
        } else {
            Debug.LogError("File does not exist" + fileName);
        }

        int max = Mathf.Max(texture.width, texture.height);

        //Texture2D tex = new Texture2D(4,4);
        //all our sprite use 50 pixel
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), max);

        //TODO load audioclip
        return null;
    }


    public SoundObject GenerateChannel(SoundUpdate sound) {

        //Debug.Log("Generate sprite" + position.ToString());

        SoundObject channel = Instantiate<SoundObject>(ChanelPrefab, this.transform);

        CheckDefaultValues(sound);

        if (usedClips.ContainsKey(sound.sound_id)) {
            channel.Clip = usedClips[sound.sound_id];
        }

        channel.name = "Channel: " + sound.channel;

        channel.ApplySettings();

        return channel;
    }



    public void UpdateChannelInScene(SoundUpdate sound) {

        SoundObject channel = channelsInScene[sound.channel];

        CheckDefaultValues(sound);

        if (usedClips.ContainsKey(sound.sound_id)) {
            channel.Clip = usedClips[sound.sound_id];
        }

        channel.ApplySettings();

    }

    public void CheckDefaultValues(SoundUpdate sound) {

    }

}
