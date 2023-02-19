using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawPile : MonoBehaviour
{
    //? Public
    public float timeBetweenInstanciatingCards;
    public List<GameObject> drawPile;
    public GameObject cardPrefab;
    public List<GameObject> playerComponents;

    public GameSetup gameSetup;

    //? Private
    private List<CardData> cardData;
    private int numberOfCardsToDealAtGameStart;


    // Start is called before the first frame update
    void Start()
    {

        gameSetup = GameObject.Find("Game Setup").GetComponent<GameSetup>();
        cardData = new List<CardData>();
        drawPile = new List<GameObject>();
        playerComponents = gameSetup.playerComponents;
        numberOfCardsToDealAtGameStart = playerComponents.Count < 4 ? 25 : 5;

        CreateSuffledDrawPile();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateSuffledDrawPile()
    {
        //! Creating a List of CardData with all the sprites and their values


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

    IEnumerator InstantiateCards()
    {

        float z = 0;
        foreach (CardData card in cardData)
        {
            // Instanciate the card
            GameObject newCard = Instantiate(cardPrefab, transform.position, transform.rotation);
            newCard.transform.SetParent(gameObject.transform); // attach Card to the GameObject this script is on
            newCard.name = "Card " + card.frontSprite.name; // uniqe object name in Unity
            newCard.GetComponent<Card>().frontSprite = card.frontSprite;
            newCard.GetComponent<Card>().value = card.value;
            //? give the card a random position within a certain range and a random z rotation
            newCard.transform.localPosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), z);
            z -= 0.01f;
            newCard.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            drawPile.Add(newCard);



            // Add a delay before instantiating the next card
            yield return new WaitForSeconds(timeBetweenInstanciatingCards);
        }

        DealCards();


    }
    void DealCards()
    {


        if (drawPile.Count < numberOfCardsToDealAtGameStart * playerComponents.Count)
        {
            Debug.LogError("Not enough cards in the draw pile to deal.");
            return;
        }

        StartCoroutine(DealCardsCoroutine());
    }
    IEnumerator DealCardsCoroutine()
    {
        float timeBetweenInstanciatingCards = 0.05f;
        float s = 4;
        float coroutineDuration = 2.0f;

        for (int i = 0; i < numberOfCardsToDealAtGameStart; i++)
        {
            foreach (GameObject player in playerComponents)
            {
                if (drawPile.Count > 0)
                {
                    int lastIndex = drawPile.Count - 1;
                    int playerNumber = player.GetComponent<PlayerDetails>().playerNumber;
                    GameObject card = drawPile[lastIndex];
                    Transform hand = player.transform.Find("Hand");
                    Vector3 startPos = card.transform.position;
                    Vector3 endPos = hand.position;
                    Quaternion startRot = card.transform.rotation;
                    Quaternion endRot = Quaternion.Euler(hand.position);

                    float t = 0;
                    while (t < 1)
                    {
                        t += Time.deltaTime * s;
                        card.transform.position = Vector3.Lerp(startPos, endPos, t);
                        card.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
                        yield return null;
                    }

                    // Only flip card for player 1
                    if (playerNumber == 1) { card.GetComponent<Card>().ToggleIsFaceUp(true); }
                    card.transform.SetParent(hand, true);
                    hand.GetComponent<Hand>().cardsInHand.Add(card);

                    drawPile.RemoveAt(lastIndex);

                    yield return new WaitForSeconds(timeBetweenInstanciatingCards);
                    hand.GetComponent<Hand>().AdjustCardPositions(0.1f);
                }
            }

            // Reduce the duration of the coroutine
            yield return new WaitForSeconds(coroutineDuration / numberOfCardsToDealAtGameStart);
        }
    }

}
