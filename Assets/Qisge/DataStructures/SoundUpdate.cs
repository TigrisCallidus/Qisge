// -*- coding: utf-8 -*-

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
[System.Serializable]

public class SoundFile {

    public int sound_id=-1;
    public string filename="No_Filename_defined!";
}

[System.Serializable]

public class SoundUpdate {

    public const int MinValue = -10;

    //Value between 0-999
    public int sound_id = MinValue;
    public int channel_id = MinValue;
    //0 = not playing, 1=oneshot, >1 = playing
    public int playmode = MinValue;
    //between 0 and 1. 1 means max volume, 0 means no sound
    public float volume = MinValue - 0.01f;
    public float pitch = MinValue - 0.01f;
    //stand in for bool: 0=false, 1=true
    public int looping = MinValue;
    public MusicalNote note = MusicalNote.None;

    public static SoundUpdate Default() {
        SoundUpdate returnValue = new SoundUpdate();
        returnValue.volume = 0.5f;
        returnValue.pitch = 1;
        returnValue.playmode = 0;
        returnValue.channel_id = 0;
        //returnValue.looping = 0;
        returnValue.note = MusicalNote.C;
        return returnValue;
    }


}

public enum MusicalNote {
    None, //=0
    C, //=1
    Csharp, //=2
    D, //==3
    Dsharp, // =4 
    E, //=5
    F, //=6
    Fsharp, //=7
    G, //=8
    Gsharp, //=9
    A, //=10
    Asharp, //=11
    H //=12
}

//TODO FOR BETTER MUSIC

//  Pitch as from notes Assume base value c (or make base value a variable)
//  Scheduling
//  