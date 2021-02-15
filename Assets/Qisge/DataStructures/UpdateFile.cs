
[System.Serializable]
public class UpdateFile {

    public SpriteChange[] image_changes;
    public SpritePosition[] sprite_changes;
    public TextUpdate[] text_changes;
    public SoundFile[] soundlist;
    public SoundUpdate[] sounds;

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
