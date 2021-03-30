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
using TMPro;
using UnityEngine;


public class WritingManager : MonoBehaviour {

    public TMP_InputField InputField;


    public string[] Commands;



    public Color CommentColor;
    public Color CommandColor;
    public Color NumberColor;
    public Color StringColor;

    string commentColorString;
    string commandColorString;
    string numberColorString;
    string stringColorString;

    string colorEnd = "</color> ";

    string lastText;
    string currentText;

    string[] lastLines;

    string[] coloredCommands;


    // Start is called before the first frame update
    void Start() {
        //Debug.Log(ColorUtility.ToHtmlStringRGB(CommentColor));

        commentColorString = GenerateColorString(CommentColor);
        commandColorString = GenerateColorString(CommandColor);
        numberColorString = GenerateColorString(NumberColor);
        stringColorString = GenerateColorString(StringColor);

        coloredCommands = new string[Commands.Length];
        for (int i = 0; i < Commands.Length; i++) {
            Commands[i] = Commands[i].Trim();
            Commands[i] = " " + Commands[i] + " ";
            coloredCommands[i] = ColorText(Commands[i], commandColorString);
        }
    }

    bool textChange = false;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            //Debug.Log("Potential New line");
            if (textChange == true) {
                CheckColoring();
                textChange = false;
            }
        }
    }

    public void TextUpdated(string newText) {

        textChange = true;
        currentText = newText;
    }


    public void CheckColoring() {


        Debug.Log(InputField.caretPosition);

        Debug.Log("CheckColoring");

        string[] seperators = {
            "\r\n", "\r", "\n"
        };

        string newText = "";

        string[] currentLines = currentText.Split(seperators, System.StringSplitOptions.None);

        Debug.Log(currentLines.Length);

        for (int i = 0; i < currentLines.Length; i++) {
            if (lastLines == null || lastLines.Length <= i || lastLines[i] != currentLines[i]) {
                currentLines[i] = prepareLine(currentLines[i]);
            } else {
                Debug.Log("Lines the same");
            }
            if (i == 0) {
                newText = currentLines[0];
            } else {
                newText += "\n"+ currentLines[i];
            }
        }

        //making text to lines
        //Checking lines against old ones, see if there was a change
        //if there was a change on the line check the coloring for that line
        //if line was added (more lines) check if not equal to last also if equal to one before (if 1 line was added)

        lastText = currentText;
        lastLines = currentLines;
        InputField.text = newText;
    }

    public string prepareLine(string line) {
        Debug.Log("PrepareLine");
        line = RemoveColor(line);
        string lineCopy = line.Trim();

        //Comments
        if (lineCopy.StartsWith("#")){
            line = ColorText(line, commentColorString);
        }

        //Comments
        for (int i = 0; i < Commands.Length; i++) {
            if (line.Contains(Commands[i])) {
                line = line.Replace(Commands[i], coloredCommands[i]);
            }
        }

        return line;
    }

    public string ColorText(string text, string colorString) {


        return colorString + text + colorEnd;
    }

    public string GenerateColorString(Color color) {
        string returnValue = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">";

        return returnValue;
    }

    public string RemoveColor(string line) {
        Debug.Log("Remove color");
        if (line.Contains("color")) {
            Debug.Log("color found " + line);

            line= line.Replace(commentColorString, string.Empty);
            line=line.Replace(commandColorString, string.Empty);
            line= line.Replace(numberColorString, string.Empty);
            line= line.Replace(stringColorString, string.Empty);
            line= line.Replace(colorEnd, string.Empty);
        } else {
            Debug.Log("no color found " + line);
        }


        return line;
    }

}
