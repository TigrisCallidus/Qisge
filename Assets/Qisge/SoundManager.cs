﻿// -*- coding: utf-8 -*-

// This code is part of Qiskit.
//
// (C) Copyright IBM 2020.
//
// This code is licensed under the Apache License, Version 2.0. You may
// obtain a copy of this license in the LICENSE.txt file in the root directory
// of this source tree or at http://www.apache.org/licenses/LICENSE-2.0.
//
// Any modifications or derivative works of this code must retain this
// copyright notice, and modified files need to carry a notice indicating
// that they have been altered from the originals.
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SoundManager : MonoBehaviour {
    public SoundObject ChanelPrefab;

    SoundObject[] channelsInScene = new SoundObject[20];

    public Dictionary<int, AudioClip> usedClips = new Dictionary<int, AudioClip>();
    public Dictionary<int, SoundUpdate> channels = new Dictionary<int, SoundUpdate>();

    public AudioSource Source;
    public string FileName;

    private void Start() {
        //Source.clip = loadClipFromMP3(FileName);
        //Source.Play();
    }


    public void Init() {

    }

    public void UpdateSounds(UpdateFile update) {
        UpdateClips(update.sound_changes);
        UpdateChannels(update.channel_changes);
    }

    public void UpdateClips(SoundFile[] soundfiles) {
        if (soundfiles==null) {
            return;
        }
        for (int i = 0; i < soundfiles.Length; i++) {
            if (usedClips.ContainsKey(soundfiles[i].sound_id)) {
                //Update audioclip
                UpdateAudioClip(soundfiles[i].sound_id, soundfiles[i].filename);
            } else {
                //add new audioclip
                GenerateAudioClip(soundfiles[i].sound_id, soundfiles[i].filename);
            }
        }
    }

    public void UpdateChannels(SoundUpdate[] sounds) {
        if (sounds==null) {
            return;
        }
        for (int i = 0; i < sounds.Length; i++) {
            //Debug.Log(sounds[i].channel);
            if (channels.ContainsKey(sounds[i].channel)) {
                UpdateChannelInScene(sounds[i]);
            } else {
                CheckDefaultValues(sounds[i]);
                channels.Add(sounds[i].channel, sounds[i]);
                if (channelsInScene[sounds[i].channel] == null) {
                    channelsInScene[sounds[i].channel] = GenerateChannel(sounds[i]);
                } else {
                    UpdateChannelInScene(sounds[i]);
                }
            }
        }
    }

    public void GenerateAudioClip(int sound_id, string filename) {
        AudioClip clip = loadClip(filename);
        usedClips.Add(sound_id, clip);
    }

    public void UpdateAudioClip(int sound_id, string filename) {
        //Generate new Audio clip
        AudioClip clip = loadClip(filename);

        usedClips[sound_id] = clip;

    }


    AudioClip loadClip(string fileName) {
        if (fileName.EndsWith(".mp3")) {
            return loadClipFromMP3(fileName);
        } else if (fileName.ToLower().EndsWith(".wav")) {
            return loadClipFromWav(fileName);
        } else if (fileName.EndsWith(".ogg")) {
            return loadClipFromOGG(fileName);
        } else {
            Debug.Log("No allowed file ending");
            return null;
        }
    }

    //TODO MAKE WITHOUT WRITING OF FILE!
    AudioClip loadClipFromMP3(string fileName) {

        AudioClip clip = null;
        //byte[] data;

        fileName = Path.Combine(GameManager.DataFolder, fileName);

        if (File.Exists(fileName) && fileName.EndsWith(".mp3")) {

            //data = File.ReadAllBytes(fileName);
            //clip = NAudioPlayer.FromMp3Data(data);
            string newFileName = NAudioPlayer.CreateWaveFromMp3(fileName);
            clip = loadClipFromWav(newFileName);
            File.Delete(newFileName);
        } else {
            Debug.LogError("File does not exist" + fileName);
        }

        return clip;
    }

    AudioClip loadClipFromWav(string fileName) {

        AudioClip clip = null;

        fileName = Path.Combine(GameManager.DataFolder, fileName);

        if (File.Exists(fileName) && fileName.EndsWith(".wav")) {

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileName, AudioType.WAV)) {
                www.SendWebRequest();

                while (!www.isDone) {
                    
                }

                if (www.isNetworkError) {
                    Debug.Log(www.error);
                } else {
                    clip = DownloadHandlerAudioClip.GetContent(www);
                }
            }
        } else {
            Debug.LogError("File does not exist" + fileName);
        }

        return clip;
    }

    AudioClip loadClipFromOGG(string fileName) {

        AudioClip clip = null;

        fileName = Path.Combine(GameManager.DataFolder, fileName);

        if (File.Exists(fileName) && fileName.EndsWith(".ogg")) {

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileName, AudioType.OGGVORBIS)) {
                www.SendWebRequest();

                while (!www.isDone) {

                }

                if (www.isNetworkError) {
                    Debug.Log(www.error);
                } else {
                    clip = DownloadHandlerAudioClip.GetContent(www);
                }
            }
        } else {
            Debug.LogError("File does not exist" + fileName);
        }

        return clip;
    }


    /*
    IEnumerator loadAudio(string fileName) {

        fileName = Path.Combine(GameManager.DataFolder, fileName);

        //making sure to read existing png file
        if (File.Exists(fileName) && fileName.EndsWith(".mp3")) {

            AudioClip clip = NAudioPlayer.FromMp3Data(c);
        }

        yield return null;

    }/*/


    public SoundObject GenerateChannel(SoundUpdate sound) {

        SoundObject channel = Instantiate<SoundObject>(ChanelPrefab, this.transform);

        if (usedClips.ContainsKey(sound.sound_id)) {
            channel.Clip = usedClips[sound.sound_id];
        }

        channel.name = "Channel: " + sound.channel;

        channel.ApplySettings(sound);

        return channel;
    }



    public void UpdateChannelInScene(SoundUpdate sound) {

        SoundObject channel = channelsInScene[sound.channel];

        CheckDefaultValues(sound);
        channels[sound.channel] = sound;

        if (usedClips.ContainsKey(sound.sound_id)) {
            channel.Clip = usedClips[sound.sound_id];
        }

        channel.ApplySettings(sound);

    }

    public void CheckDefaultValues(SoundUpdate sound) {
        SoundUpdate original = SoundUpdate.Default();

        if (channels.ContainsKey(sound.channel)) {
            original = channels[sound.channel];
        }

        if (sound.channel<=SoundUpdate.MinValue) {
            sound.channel = original.channel;
        }

        if (sound.volume <= SoundUpdate.MinValue) {
            sound.volume = original.volume;
        }

        if (sound.pitch <= SoundUpdate.MinValue) {
            sound.pitch = original.pitch;
        }

        if (sound.playmode <= SoundUpdate.MinValue) {
            sound.playmode = original.playmode;
        }
        if (sound.looping <= SoundUpdate.MinValue) {
            sound.looping = original.looping;
        }
    }

}