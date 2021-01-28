
[System.Serializable]
public class InputFile {
    public KeyPress[] key_presses;
    public ClickInput[] clicks;

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