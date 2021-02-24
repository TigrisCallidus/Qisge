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