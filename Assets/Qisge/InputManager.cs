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

public class InputManager : MonoBehaviour
{

    Dictionary<ClickInput, int> clicks = new Dictionary<ClickInput, int>();
    List<ClickInput> uniqueClicks = new List<ClickInput>();

    Dictionary<KeyPress, int> presses = new Dictionary<KeyPress, int>();
    List<KeyPress> uniquePresses = new List<KeyPress>();


    public void ControlledUpdate()
    {


        if (Input.GetMouseButtonDown(0)) {
            //Debug.Log(Input.mousePosition);
            Clickhappened(Input.mousePosition);
        }

        if (Input.GetAxis("Horizontal")>0) {
            //Debug.Log("Right received");
            PressHappened(KeyPress.right);
        } else if (Input.GetAxis("Horizontal") < 0) {
            //Debug.Log("left received");
            PressHappened(KeyPress.left);
        }

        if (Input.GetAxis("Vertical") > 0) {
            //Debug.Log("Up received");
            PressHappened(KeyPress.up);
        } else if (Input.GetAxis("Vertical") < 0) {
            //Debug.Log("Down received");
            PressHappened(KeyPress.down);
        }

        if (Input.GetButtonDown("Fire1")) {
            PressHappened(KeyPress.action3);
        }


        if (Input.GetButtonDown("Fire2")) {
            PressHappened(KeyPress.action2);
        }


        if (Input.GetButtonDown("Fire3")) {
            PressHappened(KeyPress.action4);
        }

        if (Input.GetButtonDown("Jump")) {
            PressHappened(KeyPress.action1);
        }

    }

    public void PressHappened(KeyPress press) {

        if (!presses.ContainsKey(press)) {
            uniquePresses.Add(press);
            presses.Add(press,1);
        }else {
            presses[press] = presses[press] + 1;
        }
    }


    public void Clickhappened(Vector3 position) {


        ClickInput click = GenerateClick(position);

        if (click==null) {
            return;
        }

        if (!clicks.ContainsKey(click)) {
            uniqueClicks.Add(click);
            clicks.Add(click, 1);
        } else {
            clicks[click] = clicks[click] + 1;
        }
    }

    public ClickInput GenerateClick(Vector3 position) {

        float width = 1.0f/Screen.width;
        float height = 1.0f/Screen.height;

        width = width * position.x;
        height = height * position.y;

        int x = Mathf.FloorToInt(width * VisualManager.maxWidth);
        int y = Mathf.FloorToInt(height * VisualManager.maxHeight);

        ClickInput click = new ClickInput();
        
        return null;
    }

    int count = 0;


    public bool HasInput() {
        return uniqueClicks.Count > 0 || uniquePresses.Count > 0;
    }

    public InputFile Collect() {

        InputFile inputFile = new InputFile();
        inputFile.clicks = uniqueClicks.ToArray();
        inputFile.key_presses = uniquePresses.ToArray();

        clicks.Clear();
        uniqueClicks.Clear();

        presses.Clear();
        uniquePresses.Clear();

        inputFile.count = count;

        //Debug.Log("sending message number: " + count + " at time: " + Time.time);

        count++;

        return inputFile;
    }
}
