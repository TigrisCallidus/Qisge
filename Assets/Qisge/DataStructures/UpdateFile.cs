
[System.Serializable]
public class UpdateFile {

    public SpriteChange[] image_changes;
    public SpritePosition[] sprite_changes;
}

[System.Serializable]
public class SpriteChange {

    public int image_id;
    public string filename;
}

[System.Serializable]

public class SpritePosition {

    //Value between 0-999
    public int image_id;
    public int sprite_id;
    public float x = 0;
    public float y = 0;
    public float z = 0;

    public float size = 1;

    public float angle = 0;

    public float movement_duration = 0;
    public float alpha = 1;

    //For level editor and logic.
    public TileType type;

    //Todo add coroutine for animations?

}

public enum TileType {
    empty,
    background,
    floor,
    player,
    pickup,
    enemy
}
