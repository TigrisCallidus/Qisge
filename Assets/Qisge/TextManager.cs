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
            if (UsedTexts.ContainsKey(texts[i].text_id)) {
                UpdateText(texts[i]);

            } else {
                CreateText(texts[i]);
            }
        }

    }



    public void CreateText(TextUpdate text) {
        TextsInScene[text.text_id] = Instantiate<TextObject>(TextPrefab, TextParent);
        CheckDefaultValues(text);
        UsedTexts.Add(text.text_id, text);
        UpdateTextPosition(text);
    }

    public void UpdateText(TextUpdate text) {
        CheckDefaultValues(text);
        UpdateTextPosition(text);
    }



    public void CheckDefaultValues(TextUpdate text) {
        if (UsedTexts.ContainsKey(text.text_id)) {
            TextUpdate orig = UsedTexts[text.text_id];

            if (text.text.Length== NoneString.Length && text.text== NoneString) {
                text.text = orig.text;
            } else {
                text.UpdateText = true;
            }

            bool updatePos = false;

            if (text.x < GameManager.MinPosition) {
                text.x = orig.x;
            } else {
                updatePos = true;
            }
            if (text.y < GameManager.MinPosition) {
                text.y = orig.y;
            } else {
                updatePos = true;
            }
            if (text.z < GameManager.MinPosition) {
                text.z = orig.z;
            } else {
                updatePos = true;
            }
            if (text.width < 0) {
                text.width = orig.width;
            } else {
                updatePos = true;
            }
            if (text.height < 0) {
                text.height = orig.height;
            } else {
                updatePos = true;
            }
            if (text.angle < 0) {
                text.angle = orig.angle;
            } else {
                updatePos = true;
            }
            if (text.font_size < 0) {
                text.font_size = orig.font_size;
            } else {
                updatePos = true;
            }
            if (text.font == FontType.None) {
                text.font = orig.font;
            } else {
                updatePos = true;
            }

            text.UpdatePosition= updatePos;

            bool updateColors = false;


            if (IsDefaultColor(text.background_color)) {
                text.background_color = orig.background_color;
            } else {
                updateColors = true;
            }
            if (IsDefaultColor(text.border_color)) {
                text.border_color = orig.border_color;
            } else {
                updateColors = true;
            }
            if (IsDefaultColor(text.font_color)) {
                text.font_color = orig.font_color;
            } else {
                updateColors = true;
            }

            text.UpdateColor = updateColors;


        } else {

            if (text.text.Length == NoneString.Length && text.text == NoneString) {
                text.text = "";
            } else {
                text.UpdateText = true;
            }

            bool updatePos = false;


            if (text.x < GameManager.MinPosition) {
                text.x = 0;
            } else {
                updatePos = true;
            }
            if (text.y < GameManager.MinPosition) {
                text.y = 0;
            } else {
                updatePos = true;
            }
            if (text.z < GameManager.MinPosition) {
                text.z = 0;
            } else {
                updatePos = true;
            }
            if (text.width < 0) {
                text.width = 2;
            } else {
                updatePos = true;
            }
            if (text.height < 0) {
                text.height = 1;
            } else {
                updatePos = true;
            }
            if (text.angle < 0) {
                text.angle = 0;
            } else {
                updatePos = true;
            }
            if (text.font_size < 0) {
                text.font_size = 1;
            } else {
                updatePos = true;
            }
            if (text.font == FontType.None) {
                text.font = FontType.Arial;
            } else {
                updatePos = true;
            }

            text.UpdatePosition = updatePos;

            bool updateColors = false;

            if (IsDefaultColor(text.background_color)) {
                text.background_color = new Color32(255,255,255,255);
            } else {
                updateColors = true;
            }
            if (IsDefaultColor(text.border_color)) {
                text.border_color = new Color32(0, 0, 0, 255);
            } else {
                updateColors = true;
            }
            if (IsDefaultColor(text.font_color)) {
                text.font_color = new Color32(0, 0, 0, 255);
            } else {
                updateColors = true;
            }

            text.UpdateColor = updateColors;


        }
    }

    public bool IsDefaultColor(Color32 col) {
        return col.r == NoneColor.r && col.g == NoneColor.g && col.b == NoneColor.b && col.a == NoneColor.a;
    }

    public void UpdateTextPosition(TextUpdate text) {

        UsedTexts[text.text_id] = text;
        TextObject target = TextsInScene[text.text_id];

        if (text.UpdateText) {
            target.SetText(text.text);
        }

        if (text.UpdatePosition) {

            target.PosX = text.x;
            target.PosY = text.y;
            target.PosZ = text.z;
            target.Width = text.width;
            target.Height = text.height;

            target.AdaptSize();
        }

        if (text.UpdateColor) {

            target.BorderColor = text.border_color;
            target.BackgroundColor = text.background_color;
            target.TextColor = text.font_color;

            target.AdaptColors();
        }

    }

    public void Clear() {
        StopAllCoroutines();
    }

}
