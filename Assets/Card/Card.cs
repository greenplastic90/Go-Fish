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
        UpdateSprite();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleIsFaceUp(bool boolean)
    {
        isFaceUp = boolean;
        UpdateSprite();

    }

    void UpdateSprite()
    {
        cardSpriteRenderer.sprite = isFaceUp ? frontSprite : backSprite;
    }

}

