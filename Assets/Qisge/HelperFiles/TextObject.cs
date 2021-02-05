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

    public RectTransform PositionHolder;
    public RectTransform SizeHolder;

    public Image Border;
    public Image Background;

    public Text Text;

    public void AdaptSize() {
        PositionHolder.localPosition = new Vector3(PosX * TextManager.SquareSize, PosY * TextManager.SquareSize, PosZ);
        SizeHolder.sizeDelta = new Vector2(Width* TextManager.SquareSize, Height* TextManager.SquareSize);

    }

    // Start is called before the first frame update
    void Start()
    {
        AdaptSize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
