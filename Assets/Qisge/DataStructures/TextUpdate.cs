using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TextUpdate
{
    public string text = TextManager.NoneString;
    public int _text_id;
    public float x=-1;
    public float y=-1;
    public float z=-1;

    public FontType font= FontType.None;
    public int font_size=-1;

    public float width=-1;
    public float height=-1;
    public float angle = -1;

    public Color32 font_color = TextManager.NoneColor;
    public Color32 background_color= TextManager.NoneColor;
    public Color32 border_color= TextManager.NoneColor;

}

public enum FontType {
    None,
    Arial
}