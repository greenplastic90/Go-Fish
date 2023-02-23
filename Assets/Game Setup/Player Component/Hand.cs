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
    public float cardsOffset = 0.5f;
    private bool playerDetailsUpdated = false;
    private int playerNumber;
    public bool isMainPlayer;
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
            return true;
        }
        else
        {
            StartCoroutine(DrawCardFromDrawPileCoroutine());
            return false;
        }
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


        yield return StartCoroutine(Movement.AdjustGameObjectsPositions(cardsInHand, gameObject, cardsOffset, adjustCardsPositionSpeed));

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
            StartCoroutine(AdjustCardPositionsInHandCoroutine());
            StartCoroutine(opposingPlayerHand.AdjustCardPositionsInHandCoroutine());

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
        }

    }
    void MoveCardsToBooksWon(int value)
    {
        StartCoroutine(MoveCardsToBooksWonCoroutine(value));
    }

    IEnumerator MoveCardsToBooksWonCoroutine(int value)
    {
        // Find all the cards in the hand that match the specified value
        List<GameObject> cardsToMove = cardsInHand.Where(cardInHand => cardInHand.GetComponent<Card>().value == value).ToList();
        cardsInHand.RemoveAll(card => cardsToMove.Contains(card));

        // Set the initial position and offset for the cards in Books Won
        BooksWon booksWon = BooksWonGameObject.GetComponent<BooksWon>();
        Vector3 booksWonPosition = BooksWonGameObject.transform.position;

        // Instantiate a new Book
        GameObject newBook = Instantiate(BookPrefab, booksWonPosition, Quaternion.identity);
        newBook.name = "Book" + value;
        newBook.transform.SetParent(BooksWonGameObject.transform);

        // Save cards list in newBook
        newBook.GetComponent<Book>().book = cardsToMove;

        // Set the position of the new book to a fixed value
        newBook.transform.position = booksWonPosition;

        // Move each card to the appropriate position in newBook and shrink their size
        float shrinkScale = 0.5f;
        foreach (GameObject card in cardsToMove)
        {
            int index = cardsToMove.IndexOf(card);
            yield return StartCoroutine(Movement.MoveToDesiredPosition(card, card.transform.position, newBook.transform.position, 0.5f));
            yield return StartCoroutine(ChangeCardScale(card, shrinkScale, 0.5f));
            card.transform.SetParent(newBook.transform);
        }

        // Update the positions of the cards in the hand
        yield return StartCoroutine(AdjustCardPositionsInHandCoroutine());

        // Update the positions of the books in the Books Won game object
        booksWon.booksWon.Add(newBook);
        yield return StartCoroutine(booksWon.AdjustBookPositionsInBooksWon());
    }




    [ContextMenu("Draw Card")]
    public void DrawCardFromDrawPile()
    {

        StartCoroutine(DrawCardFromDrawPileCoroutine());
    }

    public IEnumerator DrawCardFromDrawPileCoroutine()
    {
        //todo Check that draw pile isn't empty
        List<GameObject> drawPile = DrawPile.GetComponent<DrawPile>().drawPile;
        if (drawPile.Count > 0)
        {
            //todo locate card thats top of the draw pile (Last Index)
            int lastIndex = drawPile.Count - 1;
            GameObject CardGameObject = drawPile[lastIndex];
            //todo add Card to cardsInHAnd
            cardsInHand.Add(CardGameObject);
            //todo remove Card from drawPile in DrawPile
            drawPile.Remove(CardGameObject);

            if (isMainPlayer)
            {
                CardGameObject.GetComponent<Card>().ToggleIsFaceUp(true);
            }
            //todo adjust cards postion in hand
            StartCoroutine(AdjustCardPositionsInHandCoroutine());

            //todo move card
            float cardZ = CardGameObject.transform.position.z;
            yield return StartCoroutine(RotateGameObject(CardGameObject));
            //todo change Card parent to this Hand
            CardGameObject.transform.SetParent(gameObject.transform);

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
        StartCoroutine(AdjustCardPositionsInHandCoroutine());
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
