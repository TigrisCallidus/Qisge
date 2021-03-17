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
public class UpdateFile {

    public SpriteChange[] image_changes;
    public SpritePosition[] sprite_changes;
    public TextUpdate[] text_changes;
    public SoundFile[] sound_changes;
    public SoundUpdate[] channel_changes;
    public CameraChanges camera_changes;
}

[System.Serializable]
public class SpriteChange {

    public int image_id;
    public string filename;
}

[System.Serializable]

public class SpritePosition {

    //Value between 0-999
    public int sprite_id;

    public int image_id = -1;
    public float x = GameManager.MinPosition-1;
    public float y = GameManager.MinPosition-1;
    public float z = GameManager.MinPosition-1;

    public float size = -1;

    public float angle = -1;

    public bool flip_v = false;
    public bool flip_h = false;

    public float movement_duration = -1;
    public float alpha = -1;

    //For level editor and logic.
    public TileType type= TileType.notSet;

    //Todo add coroutine for animations?

}

public enum TileType {
    notSet,
    empty,
    background,
    floor,
    player,
    pickup,
    enemy
}
