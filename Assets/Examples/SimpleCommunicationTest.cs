using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimpleCommunicationTest : MonoBehaviour {


    public float movementSpeed = 1.0f;

    public Vector3 Position = new Vector3(0, 0, 0);


    public bool DoesUpdate = true;

    float lastUpdate = 0;

    public UpdateFile InitialState;



    void Start() {
        CreateInitialState();
    }




    // Update is called once per frame
    void Update() {
        if (DoesUpdate) {
            CheckForUpdate();
        }
    }


    void CreateInitialState() {
        File.WriteAllText(GameManager.SpriteFilePath, JsonUtility.ToJson(InitialState));
        float lastUpdate = Time.time;
    }

    public void CheckForUpdate() {
        string input = File.ReadAllText(GameManager.InputFilePath);
        string sprite = File.ReadAllText(GameManager.SpriteFilePath);

        if (input.Length > 0) {
            InputFile file = JsonUtility.FromJson<InputFile>(input);
            //Clear input
            HandleInput(file);
            File.WriteAllText(GameManager.InputFilePath, string.Empty);
        }

        if (sprite.Length == 0) {
            UpdateFileSmall update = WritePosition();
            string json = JsonUtility.ToJson(update);
            File.WriteAllText(GameManager.SpriteFilePath, json);
        }
    }

    void HandleInput(InputFile file) {

        float movement = (Time.time - lastUpdate) * movementSpeed;

        movement = Time.deltaTime * movementSpeed;

        for (int i = 0; i < file.key_presses.Length; i++) {
            if (file.key_presses[i] == KeyPress.up) {
                //Debug.Log("up read");
                Position += new Vector3(0, movement, 0);
            }
            if (file.key_presses[i] == KeyPress.down) {
                //Debug.Log("down read");
                Position += new Vector3(0, -movement, 0);
            }
            if (file.key_presses[i] == KeyPress.right) {
                //Debug.Log("right read");
                Position += new Vector3(movement, 0, 0);
            }
            if (file.key_presses[i] == KeyPress.left) {
                //Debug.Log("left read");
                Position += new Vector3(-movement, 0, 0);
            }
        }

        lastUpdate = Time.time;

        Debug.Log("received message number: " + file.count + " at time " + Time.time);
    }

    UpdateFileSmall WritePosition() {
        UpdateFileSmall update = new UpdateFileSmall();

        update.sprite_changes = new SpritePositionSmall[1];
        update.sprite_changes[0] = new SpritePositionSmall();
        update.sprite_changes[0].sprite_id = 0;
        //update.sprite_changes[0].image_id = 0;
        update.sprite_changes[0].x = Position.x;
        update.sprite_changes[0].y = Position.y;
        update.sprite_changes[0].z = Position.z;
        //update.sprite_changes[0].alpha = 1;

        return update;
    }


    //Used for testing/less writing
    public class UpdateFileSmall {

        public SpriteChange[] image_changes;
        public SpritePositionSmall[] sprite_changes;
    }
    /*
    [System.Serializable]
    public class SpriteChange {

        public int image_id;
        public string filename;
    }*/

    [System.Serializable]

    public class SpritePositionSmall {

        //Value between 0-999
        public int sprite_id;

        //public int image_id = -1;
        public float x = -1;
        public float y = -1;
        public float z = -1;

        //public float size = -1;

        //public float angle = -1;

        //public float movement_duration = -1;
        //public float alpha = -1;

        //For level editor and logic.
        //public TileType type;

        //Todo add coroutine for animations?

    }
    /*
    public enum TileType {
        notSet,
        empty,
        background,
        floor,
        player,
        pickup,
        enemy
    }
    */

}


[System.Serializable]
public class Grid {
    public GridRow[] Columns;

    public TileType this[int x, int y] {
        get {
            return Columns[x].Elements[y];
        }

        set {
            Columns[x].Elements[y] = value;
        }
    }
}

[System.Serializable]
public class GridRow {

    public TileType[] Elements;
}



