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
using UnityEngine.UI;

public class TextObject : MonoBehaviour
{

    public float PosX;
    public float PosY;
    public float PosZ;

    public float Width;
    public float Height;

    public Color TextColor = Color.black;
    public Color BackgroundColor = Color.white;
    public Color BorderColor = Color.black;

    public RectTransform PositionHolder;
    public RectTransform SizeHolder;

    public Image Border;
    public Image Background;

    public Text Text;

    public void SetText(string text) {
        Text.text = text;
    }

    public void AdaptSize() {



        //Debug.Log("Adapt size");
        PositionHolder.localPosition = new Vector3(PosX * TextManager.SquareSize, PosY * TextManager.SquareSize, PosZ);
        SizeHolder.sizeDelta = new Vector2(Width* TextManager.SquareSize, Height* TextManager.SquareSize);

        if (Width<=0 || Height <=0 ) {
            this.gameObject.SetActive(false);
        } else {
            this.gameObject.SetActive(true);
        }
    }

    public void AdaptColors() {
        Text.color = TextColor;
        Border.color = BorderColor;
        Background.color = BackgroundColor;
    }

    //Only for testing
    /*
    void Start()
    {
        AdaptSize();
    }
    */


}
