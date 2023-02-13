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
    public float timeBetweenCards;

    // Start is called before the first frame update
    void Start()
    {
        //! Creating a List of CardData with all the sprites and their values
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

        //! Shuffle the cardData list
        Shuffle(cardData);

        //! Instanciate Cards using the CardData List one by one with a delay in between each card
        StartCoroutine(InstantiateCards());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator InstantiateCards()
    {
        float z = 0;
        foreach (CardData card in cardData)
        {
            //! Instanciate the card
            GameObject newCard = Instantiate(cardPrefab, transform.position, transform.rotation);
            newCard.GetComponent<Card>().frontSprite = card.frontSprite;
            newCard.GetComponent<Card>().value = card.value;
            //? give the card a random position within a certain range and a random z rotation
            newCard.transform.localPosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), z);
            z -= 0.01f;
            newCard.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            //! Add a delay before instantiating the next card
            yield return new WaitForSeconds(timeBetweenCards);
        }
    }

    void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            CardData temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

