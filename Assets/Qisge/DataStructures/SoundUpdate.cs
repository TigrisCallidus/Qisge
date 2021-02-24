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

    public int sound_id;
    public string filename;
}

[System.Serializable]

public class SoundUpdate {

    public const int MinValue = -1;

    //Value between 0-999
    public int sound_id = MinValue;
    public int channel = MinValue;
    public int playmode = MinValue;
    //0 = not playing, 1=oneshot, >1 = playing
    public float volume = MinValue - 0.01f;
    public float pitch = MinValue - 0.01f;
    public int looping = MinValue;
    public MusicalNote note = MusicalNote.None;

    public static SoundUpdate Default() {
        SoundUpdate returnValue = new SoundUpdate();
        returnValue.volume = 0.5f;
        returnValue.pitch = 1;
        returnValue.playmode = 0;
        returnValue.channel = 0;
        returnValue.looping = 0;
        returnValue.note = MusicalNote.C;
        return returnValue;
    }


}

public enum MusicalNote {
    None,
    C,
    Csharp,
    D,
    Dsharp,
    E,
    F,
    Fsharp,
    G,
    Gsharp,
    A,
    Asharp,
    H
}

//TODO FOR BETTER MUSIC

//  Pitch as from notes Assume base value c (or make base value a variable)
//  Scheduling
//  