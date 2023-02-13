using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CardData
{
    public Sprite frontSprite;
    public int value;

}
public class DrawPile : MonoBehaviour
{
    public GameObject cardPrefab;
    public List<CardData> cardData;


    // Start is called before the first frame update
    void Start()
    {
        // Creating a List of CardData with all the sprites and their values
        cardData = new List<CardData>();

        //? creating an array of all the sprites in our sprite sheet
        Sprite[] sprites = Resources.LoadAll<Sprite>("Card_Sprites");

        for (int i = 0; i < sprites.Length; i++)
        {
            CardData card = new CardData();
            card.frontSprite = sprites[i];

            //? sprites have names like "D2" and "S13" to mean Diamond 2 and Spade King respectivly, we want the value from the name.
            string stringValue = sprites[i].name.Substring(1);
            card.value = int.Parse(stringValue);

            cardData.Add(card);
        }

        // Shuffle the cardData list
        System.Random random = new System.Random();
        for (int i = 0; i < cardData.Count; i++)
        {
            int randomIndex = random.Next(0, cardData.Count);
            CardData temp = cardData[i];
            cardData[i] = cardData[randomIndex];
            cardData[randomIndex] = temp;
        }

        // Instantiate cards using the shuffled cardData list
        foreach (CardData cardData in cardData)
        {
            GameObject card = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
            card.GetComponent<Card>().frontSprite = cardData.frontSprite;
            card.GetComponent<Card>().value = cardData.value;
        }



    }

    // Update is called once per frame
    void Update()
    {

    }
}

