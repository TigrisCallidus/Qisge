using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SimpleCommunicationTest : MonoBehaviour {


    public float movementSpeed = 1.0f;

    public Vector3 Position = new Vector3(0, 0, 0);

    float lastUpdate = 0;

    public UpdateFile InitialState;


    void Start() {
        CreateInitialState();
    }




    // Update is called once per frame
    void Update() {
        CheckForUpdate();
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
            UpdateFile update = WritePosition();
            string json = JsonUtility.ToJson(update);
            File.WriteAllText(GameManager.SpriteFilePath, json);
        }
    }

    void HandleInput(InputFile file) {

        float movement = (Time.time-lastUpdate) * movementSpeed;

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
    }

    UpdateFile WritePosition() {
        UpdateFile update = new UpdateFile();

        update.sprite_changes = new SpritePosition[1];
        update.sprite_changes[0] = new SpritePosition();
        update.sprite_changes[0].sprite_id = 0;
        update.sprite_changes[0].image_id = 0;
        update.sprite_changes[0].x = Position.x;
        update.sprite_changes[0].y = Position.y;
        update.sprite_changes[0].z = Position.z;
        update.sprite_changes[0].alpha = 0;

        return update;
    }
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