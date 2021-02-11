using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour {


    public Transform TextParent;

    public TextObject TextPrefab;


    //Sample resolution: 3840 x 2160
    //left and right border: 30
    //Square size: 135

    public const float SquareSize = 135;

    public const string NoneString = "NoNe";
    public static readonly Color32 NoneColor = new Color32(0, 1, 2, 3);

    TextObject[] TextsInScene = new TextObject[100];

    Dictionary<int, TextUpdate> UsedTexts = new Dictionary<int, TextUpdate>();


    public void UpdateTexts(TextUpdate[] texts) {
        if (texts==null) {
            return;
        }
        for (int i = 0; i < texts.Length; i++) {
            if (UsedTexts.ContainsKey(texts[i]._text_id)) {
                UpdateText(texts[i]);

            } else {
                CreateText(texts[i]);
            }
        }

    }



    public void CreateText(TextUpdate text) {
        TextsInScene[text._text_id] = Instantiate<TextObject>(TextPrefab, TextParent);
        CheckDefaultValues(text);
        UsedTexts.Add(text._text_id, text);
        UpdateTextPosition(text);
    }

    public void UpdateText(TextUpdate text) {
        CheckDefaultValues(text);
        UpdateTextPosition(text);
    }



    public void CheckDefaultValues(TextUpdate text) {
        if (UsedTexts.ContainsKey(text._text_id)) {
            TextUpdate orig = UsedTexts[text._text_id];

            if (text.text.Length== NoneString.Length && text.text== NoneString) {
                text.text = orig.text;
            }

            if (text.x < 0) {
                text.x = orig.x;
            }
            if (text.y < 0) {
                text.y = orig.y;
            }
            if (text.z < 0) {
                text.z = orig.z;
            }

            if (text.font == FontType.None) {
                text.font = orig.font;
            }

            if (text.font_size < 0) {
                text.font_size = orig.font_size;
            }

            if (text.width < 0) {
                text.width = orig.width;
            }
            if (text.height < 0) {
                text.height = orig.height;
            }
            if (text.angle < 0) {
                text.angle = orig.angle;
            }

            if (IsDefaultColor(text.background_color)) {
                text.background_color = orig.background_color;
            }
            if (IsDefaultColor(text.border_color)) {
                text.border_color = orig.border_color;
            }
            if (IsDefaultColor(text.font_color)) {
                text.font_color = orig.font_color;
            }

        } else {

            if (text.text.Length == NoneString.Length && text.text == NoneString) {
                text.text = "";
            }
            
            if (text.x < 0) {
                text.x = 0;
            }
            if (text.y < 0) {
                text.y = 0;
            }
            if (text.z < 0) {
                text.z = 0;
            }

            if (text.font == FontType.None) {
                text.font = FontType.Arial;
            }

            if (text.font_size < 0) {
                text.font_size = 1;
            }

            if (text.width < 0) {
                text.width = 2;
            }
            if (text.height < 0) {
                text.height = 1;
            }
            if (text.angle < 0) {
                text.angle = 0;
            }

            if (IsDefaultColor(text.background_color)) {
                text.background_color = new Color32(255,255,255,255);
            }
            if (IsDefaultColor(text.border_color)) {
                text.border_color = new Color32(0, 0, 0, 255);
            }
            if (IsDefaultColor(text.font_color)) {
                text.font_color = new Color32(0, 0, 0, 255);
            }

        }
    }

    public bool IsDefaultColor(Color32 col) {
        return col.r == NoneColor.r && col.g == NoneColor.g && col.b == NoneColor.b && col.a == NoneColor.a;
    }

    public void UpdateTextPosition(TextUpdate text) {
        TextObject target = TextsInScene[text._text_id];
        target.PosX = text.x;
        target.PosY = text.y;
        target.PosZ = text.z;
        target.Width = text.width;
        target.Height = text.height;

        target.AdaptSize();

        target.BorderColor = text.border_color;
        target.BackgroundColor = text.background_color;
        target.TextColor = text.font_color;

        target.AdaptColors();

        target.SetText(text.text);
    }

}
