using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VisualManager : MonoBehaviour {



    public SpriteRenderer RendererPrefab;


    //Limit of 1000 Sprites, this is about 3 per grid element, which is a lot. Normally 350 (number of grid elements) should be enough
    SpriteRenderer[] SpritesInScene = new SpriteRenderer[1000];

    Dictionary<int, SpritePosition> Positions = new Dictionary<int, SpritePosition>();
    Dictionary<int, Sprite> UsedSprites = new Dictionary<int, Sprite>();

    //To store which sprite is used by which renderer (for update)
    Dictionary<int, List<int>> spriteReferences = new Dictionary<int, List<int>>();





    //Grid 25x14 (squares)
    //Sprite import 50 pixel per unit
    public const int maxWidth = 25;
    public const int maxHeight = 14;


    public void UpdateAll(UpdateFile update) {
        UpdateSprites(update.image_changes);
        UpdatePositions(update.sprite_changes);
    }

    public void UpdateSprites(SpriteChange[] sprites) {
        for (int i = 0; i < sprites.Length; i++) {
            if (UsedSprites.ContainsKey(sprites[i].image_id)) {
                //Update sprite
                UpdateSpriteTexture(sprites[i].image_id, sprites[i].filename);
            } else {
                //add new sprite
                GenerateSprite(sprites[i].filename, sprites[i].image_id);
                //UsedSprites.Add(sprites[i].image_id, GenerateSprite(sprites[i].filename));
            }
        }
    }

    public void UpdatePositions(SpritePosition[] positions) {
        for (int i = 0; i < positions.Length; i++) {
            if (Positions.ContainsKey(positions[i].sprite_id)) {
                UpdateSpriteInScene(positions[i]);
            } else {
                Positions.Add(positions[i].sprite_id, positions[i]);
                if (SpritesInScene[positions[i].sprite_id] == null) {
                    SpritesInScene[positions[i].sprite_id] = GenerateSpriteInScene(positions[i]);
                } else {
                    UpdateSpriteInScene(positions[i]);
                }
            }
        }
    }

    //TODO make stuff in parallel

    public void GenerateSprite(string fileName, int imageID) {

        Sprite sprite = loadSpriteFromPNG(fileName);

        UsedSprites.Add(imageID, sprite);

        if (spriteReferences.ContainsKey(imageID)) {
            Debug.LogError("iamge ID already used:" + imageID);
        } else {
            spriteReferences.Add(imageID, new List<int>());
        }

    }




    public void UpdateSpriteTexture(int imageID, string fileName) {
        //Generate new sprite

        Sprite sprite = loadSpriteFromPNG(fileName);

        UsedSprites[imageID] = sprite;

        //update all SpriteInScenes, which use the given sprite

        for (int i = 0; i < spriteReferences[imageID].Count; i++) {
            int spriteref = spriteReferences[imageID][i];
            if (SpritesInScene[spriteref]!=null) {
                SpritesInScene[spriteref].sprite = UsedSprites[imageID];
            }
                
        }

    }

    Sprite loadSpriteFromPNG(string fileName) {

        Texture2D texture = null;
        byte[] data;

        fileName = Path.Combine(GameManager.DataFolder,fileName);

        //making sure to read existing png file
        if (File.Exists(fileName) && fileName.EndsWith(".png") ) {

            //Load texture from file
            data = File.ReadAllBytes(fileName);
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be set correctly here
            texture.LoadImage(data);
        } else if (File.Exists(fileName +".png")) {
            //Load texture from file
            data = File.ReadAllBytes(fileName + ".png");
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be set correctly here
            texture.LoadImage(data);
        } else {
            Debug.LogError("File does not exist"  + fileName);
        }

        //Texture2D tex = new Texture2D(4,4);
        //all our sprite use 50 pixel
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 50.0f);

        return sprite;
    }


    //TODO animations

    public SpriteRenderer GenerateSpriteInScene(SpritePosition position) {

        //Debug.Log("Generate sprite" + position.ToString());

        Vector3 pos = new Vector3(position.x, position.y, position.z);
        Quaternion rot = Quaternion.Euler(0, position.angle, 0);

        SpriteRenderer renderer = Instantiate<SpriteRenderer>(RendererPrefab, pos, rot,  this.transform);

        if (UsedSprites.ContainsKey(position.image_id)) {
            renderer.sprite = UsedSprites[position.image_id];
        }

        renderer.name = position.sprite_id.ToString();

        return renderer;
    }

    //Recycle a sprite which might already be there, but no longer used. Not sure if needed.
    public void RecycleSpriteInScene(SpritePosition position) {

        Debug.Log("RecycleSprite");

        Vector3 pos = new Vector3(position.x, position.y, position.z);
        Quaternion rot = Quaternion.Euler(0, position.angle, 0);

        SpriteRenderer renderer = SpritesInScene[position.sprite_id];
        renderer.transform.position = pos;
        renderer.transform.rotation = rot;

        if (UsedSprites.ContainsKey(position.image_id)) {
            renderer.sprite = UsedSprites[position.image_id];
        }

        //renderer.name = position.sprite_id.ToString();
    }

    public void UpdateSpriteInScene(SpritePosition position) {

        Vector3 pos = new Vector3(position.x, position.y, position.z);
        Quaternion rot = Quaternion.Euler(0, position.angle, 0);

        SpriteRenderer renderer = SpritesInScene[position.sprite_id];
        renderer.transform.position = pos;
        renderer.transform.rotation = rot;

        if (UsedSprites.ContainsKey(position.image_id)) {
            renderer.sprite = UsedSprites[position.image_id];
        }

        //renderer.name = position.sprite_id.ToString();
    }


    public void Clear() {
        StopAllCoroutines();
        //Clear all dictionaries
        //Make all SpritesInScene invisible
    }

    /*
        //FOR TESTING: TODO REMOVE LATER
        public UpdateFile UsedFile;
        public string JsonString;
        void Start()
        {
            // FOR TESTING TODO REMOVE
            //string json = JsonUtility.ToJson(UsedFile);
            //JsonString = json;

            UsedFile = JsonUtility.FromJson<UpdateFile>(JsonString);
        }
    */

    // Update is called once per frame
    void Update() {

    }
}


