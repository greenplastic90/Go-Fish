using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public Sprite frontSprite;
    public int value;

}

public class GameSetup : MonoBehaviour
{
    private List<CardData> cardData;
    public GameObject cardPrefab;

    public List<GameObject> drawPile;
    private int numberOfCardsEachPlayerDraws;
    public float timeBetweenCards;

    //! Number of players has to be between no less than 2 and no greater than 5
    public int numberOfPlayers;

    public GameObject drawPilePrefab;
    public GameObject playerComponentPrefab;

    public List<GameObject> playerComponents;
    // Start is called before the first frame update

    void Start()
    {
        playerComponents = new List<GameObject>();
        cardData = new List<CardData>();
        drawPile = new List<GameObject>();

        numberOfCardsEachPlayerDraws = numberOfPlayers < 3 ? 7 : 5;
        // numberOfCardsEachPlayerDraws = 1;

        InstantiatePlayers(numberOfPlayers);
        CreateSuffledDrawPile();

    }





    //! Instanciating players
    void InstantiatePlayers(int numberOfPlayers)
    {
        // Instantiate the player components based on the number of players
        switch (numberOfPlayers)
        {
            case 2:
                InstantiatePlayer(1, new Vector3(0, -4, 0)); // Bottom center
                InstantiatePlayer(2, new Vector3(0, 4, 0));  // Top center
                break;
            case 3:
                InstantiatePlayer(1, new Vector3(0, -4, 0));           // Bottom center
                InstantiatePlayer(2, new Vector3(-4, 4, 0));          // Top left
                InstantiatePlayer(3, new Vector3(4, 4, 0));           // Top right
                break;
            case 4:
                InstantiatePlayer(1, new Vector3(0, -4, 0));           // Bottom center
                InstantiatePlayer(2, new Vector3(0, 4, 0));           // Top center
                InstantiatePlayer(3, new Vector3(-7, 0, 0));          // Left center
                InstantiatePlayer(4, new Vector3(7, 0, 0));           // Right center
                break;
            case 5:
                InstantiatePlayer(1, new Vector3(0, -4, 0));           // Bottom center
                InstantiatePlayer(2, new Vector3(-4, 4, 0));          // Top left
                InstantiatePlayer(3, new Vector3(4, 4, 0));           // Top right
                InstantiatePlayer(4, new Vector3(-7, 0, 0));          // Left center
                InstantiatePlayer(5, new Vector3(7, 0, 0));           // Right center
                break;
            default:
                Debug.LogError("Unsupported number of players: " + numberOfPlayers);
                break;
        }
    }

    void InstantiatePlayer(int playerNumber, Vector3 position)
    {
        GameObject playerComponent = Instantiate(playerComponentPrefab, position, Quaternion.identity);
        playerComponent.name = "Player " + playerNumber; // uniqe object name
        playerComponent.GetComponent<PlayerDetails>().playerNumber = playerNumber;
        playerComponents.Add(playerComponent);
    }







    //! Darw pile 
    void DealCards()
    {
        if (drawPile.Count < numberOfCardsEachPlayerDraws * playerComponents.Count)
        {
            Debug.LogError("Not enough cards in the draw pile to deal.");
            return;
        }

        StartCoroutine(DealCardsCoroutine());
    }

    IEnumerator DealCardsCoroutine()
    {
        for (int i = 0; i < numberOfCardsEachPlayerDraws; i++)
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
                        t += Time.deltaTime * 3;
                        card.transform.position = Vector3.Lerp(startPos, endPos, t);
                        card.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
                        yield return null;


                    }

                    //? Only flip card for player 1
                    if (playerNumber == 1) { card.GetComponent<Card>().ToggleFaceUp(true); }
                    card.transform.SetParent(hand, true);
                    hand.GetComponent<Hand>().AddCard(card);
                    drawPile.RemoveAt(lastIndex);
                    yield return new WaitForSeconds(timeBetweenCards);
                }
            }
        }
    }






    void CreateSuffledDrawPile()
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

    IEnumerator InstantiateCards()
    {
        float z = 0;
        foreach (CardData card in cardData)
        {
            //! Instanciate the card
            GameObject newCard = Instantiate(cardPrefab, transform.position, transform.rotation);
            newCard.name = "Card " + card.frontSprite.name; // uniqe object name in Unity
            newCard.GetComponent<Card>().frontSprite = card.frontSprite;
            newCard.GetComponent<Card>().value = card.value;
            //? give the card a random position within a certain range and a random z rotation
            newCard.transform.localPosition = new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), z);
            z -= 0.01f;
            newCard.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

            drawPile.Add(newCard);



            //! Add a delay before instantiating the next card
            yield return new WaitForSeconds(timeBetweenCards);
        }

        DealCards();


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
