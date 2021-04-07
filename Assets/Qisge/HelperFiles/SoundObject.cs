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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundObject : MonoBehaviour {

    public AudioClip Clip;
    public AudioSource Source;

    public void ApplySettings(SoundUpdate sound) {

        Source.volume = sound.volume;
        Source.pitch = sound.pitch;
        if (sound.note != MusicalNote.None && sound.note != MusicalNote.C) {
            float increase = (int)sound.note;
            increase = increase - 1;
            float pitch = Mathf.Pow(2, increase / 12);
            Source.pitch = pitch;
        }

        if (sound.playmode == 0) {
            Source.Stop();
        } else if (sound.playmode > 1) {
            Source.PlayOneShot(Clip);
            Debug.Log("One shot");
            //Source.clip = Clip;
        } else if (sound.playmode == 1) {
            //default set to looping.
            Source.clip = Clip;
            Source.Play();
            Source.loop = true;
            if (sound.looping > 0) {
                Source.loop = true;
            } else if (sound.looping == 0) {
                Source.loop = false;
            }
        }

    }

}
