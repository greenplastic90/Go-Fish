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

        // Instanciate Cards using the CardData List
        float z = 0;
        for (int i = 0; i < cardData.Count; i++)
        {
            GameObject card = Instantiate(cardPrefab, transform.position, Quaternion.identity, transform);
            card.GetComponent<Card>().frontSprite = cardData[i].frontSprite;
            card.GetComponent<Card>().value = cardData[i].value;

            //? give the card a random position within a certain range and a random z rotation
            card.transform.localPosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), z);
            z -= 0.01f;
            card.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}

