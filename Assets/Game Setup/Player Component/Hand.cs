using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hand : MonoBehaviour
{
    private Movement Movement;
    public GameObject PlayerComponent;
    public GameObject BooksWonGameObject;
    public GameObject BookPrefab;
    public DrawPile DrawPile;
    public List<GameObject> cardsInHand;
    public PlayerDetails ThisPlayerDetails;
    bool isPlayerTurn = true;
    public float cardsOffset = 0.5f;
    private bool playerDetailsUpdated = false;

    private int playerNumber;
    public bool isMainPlayer = false;
    [SerializeField]
    public float drawCardSpeed;
    [SerializeField]
    private float rotateCardSpeed;
    [SerializeField]
    private float adjustCardsPositionSpeed;


    // Start is called before the first frame update
    void Start()
    {
        Movement = GameObject.Find("Game Logic").GetComponent<Movement>();
        DrawPile = GameObject.Find("Draw Pile").GetComponent<DrawPile>();
        cardsInHand = new List<GameObject>();


    }

    void Update()
    {
        // Updates cardOffset once thisPlayerDetails has been assigned
        if (ThisPlayerDetails != null && !playerDetailsUpdated)
        {
            playerNumber = ThisPlayerDetails.playerNumber;

            playerDetailsUpdated = true;

            if (playerNumber == 1)
            {
                isMainPlayer = true;
                cardsOffset = 1f;
            }


        }


    }

    [ContextMenu("Take Turn")]
    public bool TakeTurn()
    {
        //todo ask for card
        bool opposingPlayerHasCard = AskForCardFromAnotherPlayer();



        if (opposingPlayerHasCard)
        {

            Debug.Log("Player " + playerNumber + " Take Another Turn");
            return true;
        }
        else
        {
            DrawCardFromDrawPile();
            Debug.Log("Player " + playerNumber + " Turn Ends");
            return false;
        }



        //todo

    }
    public void AdjustCardPositionsInHand()
    {
        StartCoroutine(AdjustCardPositionsInHandCoroutine());
    }

    private IEnumerator AdjustCardPositionsInHandCoroutine()
    {
        if (isMainPlayer)
        {
            cardsInHand.Sort(new CardValueComparer());
        }


        Movement.AdjustGameObjectsPositions(cardsInHand, PlayerComponent, cardsOffset, adjustCardsPositionSpeed);
        yield return null;
    }

    [SerializeField]
    private int opposingPlayerNumber = 2;

    [SerializeField]
    private int cardValue = 2;

    [ContextMenu("Ask For Card")]
    bool AskForCardFromAnotherPlayer()
    {
        // Find the hand of the player you want to access using opposingPlayerNumber
        GameObject opposingPlayerComponent = GameObject.Find("Player " + opposingPlayerNumber);
        Hand opposingPlayerHand = opposingPlayerComponent.GetComponentInChildren<Hand>();



        // Find all the cards in the opposing player's hand that match the specified value
        List<GameObject> matchingCards = opposingPlayerHand.cardsInHand
            .Where(card => card.GetComponent<Card>().value == cardValue)
            .ToList();

        if (matchingCards.Count > 0)
        {
            // Remove the matching cards from the opposing player's hand and add them to this hand's cardsInHand
            opposingPlayerHand.cardsInHand.RemoveAll(card => matchingCards.Contains(card));
            cardsInHand.AddRange(matchingCards);

            // Update the positions of the cards in hands
            AdjustCardPositionsInHand();
            opposingPlayerHand.AdjustCardPositionsInHand();

            // Set the parent of the matching cards to this hand
            matchingCards.ForEach(card =>
            {
                card.transform.SetParent(transform);
                if (isMainPlayer)
                {
                    card.GetComponent<Card>().ToggleIsFaceUp(true);
                }
                else
                {
                    card.GetComponent<Card>().ToggleIsFaceUp(false);
                }
            });
            CheckForFourOfAKind();
            return true;
        }
        return false;
    }


    [ContextMenu("Check For 4 Of A Kind")]
    public void CheckForFourOfAKind()
    {
        bool foundFourOfAKind = false;
        Dictionary<int, int> valueCounts = new Dictionary<int, int>();

        foreach (GameObject card in cardsInHand)
        {
            int value = card.GetComponent<Card>().value;

            if (valueCounts.ContainsKey(value))
            {
                valueCounts[value]++;
            }
            else
            {
                valueCounts.Add(value, 1);
            }
        }

        foreach (int value in valueCounts.Keys)
        {
            if (valueCounts[value] >= 4)
            {
                foundFourOfAKind = true;

                MoveCardsToBooksWon(value);
            }
        }

        if (!foundFourOfAKind)
        {

            Debug.Log("No Four Of A Kinds Found");
        }

    }


    void MoveCardsToBooksWon(int value)
    {
        // Find all the cards in the hand that match the specified value
        List<GameObject> cardsToMove = cardsInHand.Where(cardInHand => cardInHand.GetComponent<Card>().value == value).ToList();
        cardsInHand.RemoveAll(card => cardsToMove.Contains(card));

        // Set the initial position and offset for the cards in Books Won
        Vector3 booksWonPosition = BooksWonGameObject.transform.position;
        // Instanciate a new Book
        GameObject newBook = Instantiate(BookPrefab, booksWonPosition, Quaternion.identity);
        newBook.name = "Book" + value;


        // Make it child of Books Won
        newBook.transform.SetParent(BooksWonGameObject.transform);

        // Move each card to the appropriate position in newBook and shrink their size
        float shrinkScale = 0.5f;
        foreach (GameObject card in cardsToMove.ToList())
        {
            int index = cardsToMove.IndexOf(card);
            StartCoroutine(Movement.MoveToDesiredPosition(card, card.transform.position, booksWonPosition, 0.5f));
            StartCoroutine(ChangeCardScale(card, shrinkScale, 0.5f));
            AdjustCardPositionsInHand();
            card.transform.SetParent(newBook.transform, false);
            // cardsInHand.Remove(card);
        }

        // Save cards list in newBook
        newBook.GetComponent<Book>().book = cardsToMove;

        // Add new book to booksWon list in in Books Won game object
        BooksWon booksWon = BooksWonGameObject.GetComponent<BooksWon>();
        booksWon.booksWon.Add(newBook);
        booksWon.AdjustBookPositionsInBooksWon();

    }



    [ContextMenu("Draw Card")]
    public void DrawCardFromDrawPile()
    {

        StartCoroutine(DrawCardFromDrawPileCoroutine());
    }
    private IEnumerator DrawCardFromDrawPileCoroutine()
    {
        //todo Check that draw pile isn't empty
        List<GameObject> drawPile = DrawPile.GetComponent<DrawPile>().drawPile;
        if (drawPile.Count > 0)
        {
            //todo locate card thats top of the draw pile (Last Index)
            int lastIndex = drawPile.Count - 1;
            GameObject CardGameObject = drawPile[lastIndex];
            //todo change Card parent to this Hand
            CardGameObject.transform.SetParent(gameObject.transform);
            //todo add Card to cardsInHAnd
            cardsInHand.Add(CardGameObject);
            //todo remove Card from drawPile in DrawPile
            drawPile.Remove(CardGameObject);
            //todo move card
            float cardZ = CardGameObject.transform.position.z;


            StartCoroutine(Movement.MoveToDesiredPosition(CardGameObject, CardGameObject.transform.position + new Vector3(0, 0, cardZ), transform.position, drawCardSpeed));
            // Debug.Log("MoveToDesiredPosition");

            yield return StartCoroutine(RotateGameObject(CardGameObject));
            // Debug.Log("RotateGameObject");
            if (isMainPlayer)
            {
                CardGameObject.GetComponent<Card>().ToggleIsFaceUp(true);
            }

            // Debug.Log("AdjustCardPositionsInHandCoroutine");
            //todo adjust cards postion in hand
            yield return StartCoroutine(AdjustCardPositionsInHandCoroutine());
            CheckForFourOfAKind();
        }
    }


    IEnumerator RotateGameObject(GameObject GameObject)
    {
        Quaternion startRot = GameObject.transform.rotation;
        Quaternion endRot = Quaternion.Euler(transform.position);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * rotateCardSpeed;
            GameObject.transform.rotation = Quaternion.Lerp(startRot, endRot, t);
            yield return null;
        }
    }


    IEnumerator ChangeCardScale(GameObject card, float scale, float duration)
    {
        float time = 0f;
        Vector3 startingScale = card.transform.localScale;
        Vector3 desiredScale = new Vector3(scale, scale, 1f);
        while (time < duration)
        {
            Vector3 newScale = new Vector3(
                Mathf.Lerp(startingScale.x, desiredScale.x, time / duration),
                Mathf.Lerp(startingScale.y, desiredScale.y, time / duration),
                1f
            );
            card.transform.localScale = newScale;
            time += Time.deltaTime;
            yield return null;
        }
        card.transform.localScale = desiredScale;
        AdjustCardPositionsInHand();
    }

    // Custom comparer to compare the value of cards
    private class CardValueComparer : IComparer<GameObject>
    {
        public int Compare(GameObject a, GameObject b)
        {
            int valueA = a.GetComponent<Card>().value;
            int valueB = b.GetComponent<Card>().value;
            return valueB.CompareTo(valueA);
        }
    }


}
