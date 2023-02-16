using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public Sprite backSprite;
    public Sprite frontSprite;
    public int value;
    public SpriteRenderer cardSpriteRenderer;

    public bool isFaceUp;
    // Start is called before the first frame update
    void Start()
    {
        cardSpriteRenderer.sprite = isFaceUp ? frontSprite : backSprite;
    }

    // Update is called once per frame
    void Update()
    {
        cardSpriteRenderer.sprite = isFaceUp ? frontSprite : backSprite;
    }

    public void ToggleFaceUp(bool boolean)
    {
        isFaceUp = boolean;
    }

}

