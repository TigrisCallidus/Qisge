[System.Serializable]

public class SoundFile {

    public int sound_id;
    public string filename;
}

[System.Serializable]

public class SoundUpdate {

    //Value between 0-999
    public int sound_id;
    public int channel;
    public int playmode;
    public float volume;
    public float pitch;
}

//TODO FOR BETTER MUSIC

//  Pitch as from notes Assume base value c (or make base value a variable)
//  Scheduling
//  