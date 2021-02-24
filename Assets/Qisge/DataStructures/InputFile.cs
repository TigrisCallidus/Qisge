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
[System.Serializable]
public class InputFile {
    public KeyPress[] key_presses;
    public ClickInput[] clicks;
    public int count = 0;
}

public enum KeyPress {
    up,
    right,
    down,
    left,
    action1, //top action button
    action2, //right action button
    action3, //bottom action button
    action4, //left action button
}

[System.Serializable]
public class ClickInput {
    public int X;
    public int Y;
}