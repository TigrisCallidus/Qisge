using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextUpdate
{
    public string text = TextManager.NoneString;
    public int text_id;
    public float x= GameManager.MinPosition - 1;
    public float y= GameManager.MinPosition - 1;
    public float z= GameManager.MinPosition - 1;

    public FontType font= FontType.None;
    public int font_size=-1;

    public float width=-1;
    public float height=-1;
    public float angle = -1;

    public Color32 font_color = TextManager.NoneColor;
    public Color32 background_color= TextManager.NoneColor;
    public Color32 border_color= TextManager.NoneColor;

    public bool UpdateText {
        get {
            return updateText;
        }
        set {
            updateText = value;
        }
    }

    bool updateText = false;

    public bool UpdatePosition {
        get {
            return updatePosition;
        }
        set {
            updatePosition = value;
        }
    }
    bool updatePosition = false;
    public bool UpdateColor {
        get {
            return updateColor;
        }
        set {
            updateColor = value;
        }
    }
    bool updateColor = false;

}

public enum FontType {
    None,
    Arial
}