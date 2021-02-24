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





    //Grid now 28x16 was initially 25x14 (squares)
    //Sprite import maximum of width and height pixel per unit (to get a scale 1)
    public const int maxWidth = 28;
    public const int maxHeight = 16;


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
            if (SpritesInScene[spriteref] != null) {
                SpritesInScene[spriteref].sprite = UsedSprites[imageID];
            }

        }

    }

    Sprite loadSpriteFromPNG(string fileName) {

        Texture2D texture = null;
        byte[] data;

        fileName = Path.Combine(GameManager.DataFolder, fileName);

        //making sure to read existing png file
        if (File.Exists(fileName) && fileName.EndsWith(".png")) {

            //Load texture from file
            data = File.ReadAllBytes(fileName);
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be set correctly here
            texture.LoadImage(data);
        } else if (File.Exists(fileName + ".png")) {
            //Load texture from file
            data = File.ReadAllBytes(fileName + ".png");
            //Small values to initialize texture
            texture = new Texture2D(2, 2);
            texture.name = fileName;
            //The correct size will be set correctly here
            texture.LoadImage(data);
        } else {
            Debug.LogError("File does not exist" + fileName);
        }

        int max = Mathf.Max(texture.width, texture.height);

        //Texture2D tex = new Texture2D(4,4);
        //all our sprite use 50 pixel
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), max);

        return sprite;
    }


    //TODO animations

    public SpriteRenderer GenerateSpriteInScene(SpritePosition position) {

        //Debug.Log("Generate sprite" + position.ToString());

        CheckDefaultValues(position);

        Vector3 pos = new Vector3(position.x, position.y, -position.z);
        Quaternion rot = Quaternion.Euler(0, 0, position.angle);
        float size = position.size;
        Vector3 sizeVec = new Vector3(size, size, size);

        SpriteRenderer renderer = Instantiate<SpriteRenderer>(RendererPrefab, pos, rot, this.transform);

        renderer.transform.localScale = sizeVec;


        if (UsedSprites.ContainsKey(position.image_id)) {
            renderer.sprite = UsedSprites[position.image_id];
        }

        renderer.name = position.sprite_id.ToString();

        return renderer;
    }



    public void UpdateSpriteInScene(SpritePosition position) {

        CheckDefaultValues(position);

        Vector3 pos = new Vector3(position.x, position.y, -position.z);
        Quaternion rot = Quaternion.Euler(0, 0, position.angle);
        float size =position.size;
        Vector3 sizeVec = new Vector3(size, size, size);

        SpriteRenderer renderer = SpritesInScene[position.sprite_id];
        renderer.transform.position = pos;
        renderer.transform.rotation = rot;
        renderer.transform.localScale = sizeVec;

        Positions[position.sprite_id] = position;

        if (UsedSprites.ContainsKey(position.image_id)) {
            renderer.sprite = UsedSprites[position.image_id];
        }



        //renderer.name = position.sprite_id.ToString();
    }


    public void CheckDefaultValues(SpritePosition position) {


        if (Positions.ContainsKey(position.sprite_id)) {
            SpritePosition orig = Positions[position.sprite_id];
            if (position.image_id < 0) {
                position.image_id = orig.image_id;
            }
            if (position.x < GameManager.MinPosition) {
                position.x = orig.x;
            }
            if (position.y < GameManager.MinPosition) {
                position.y = orig.y;
            }
            if (position.z < GameManager.MinPosition) {
                position.z = orig.z;
            }
            if (position.size < 0) {
                position.size = orig.size;
            }
            if (position.angle < 0) {
                position.angle = orig.angle;
            }
            if (position.movement_duration < 0) {
                position.movement_duration = orig.movement_duration;
            }
            if (position.alpha < 0) {
                position.alpha = orig.alpha;
            }
            if (position.type == TileType.notSet) {
                position.type = orig.type;
            }
        } else {
            if (position.size < 0) {
                position.size = 1;
            }
            if (position.angle < 0) {
                position.angle = 0;
            }
            if (position.movement_duration < 0) {
                position.movement_duration = 0;
            }
            if (position.alpha < 0) {
                position.alpha = 1;
            }
            if (position.type == TileType.notSet) {
                position.type = TileType.empty;
            }
        }


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
}


/*
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
*/
