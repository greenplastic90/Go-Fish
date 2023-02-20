using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hand : MonoBehaviour
{
    private GameLogic gameLogic;
    public List<GameObject> cardsInHand;
    private int lastCardCount;
    public float cardsOffset = 0.5f;
    private bool playerDetailsUpdated = false;
    public PlayerDetails thisPlayerDetails;
    private int playerNumber;
    public GameObject PlayerComponent;
    public GameObject booksWonGameObject;
    public GameObject bookPrefab;

    public bool isMainPlayer = false;


    // Start is called before the first frame update
    void Start()
    {
        gameLogic = GameObject.Find("Game Logic").GetComponent<GameLogic>();
        cardsInHand = new List<GameObject>();
        lastCardCount = cardsInHand.Count;

    }

    void Update()
    {
        // Updates cardOffset once thisPlayerDetails has been assigned
        if (thisPlayerDetails != null && !playerDetailsUpdated)
        {
            playerNumber = thisPlayerDetails.playerNumber;

            playerDetailsUpdated = true;

            if (playerNumber == 1)
            {
                isMainPlayer = true;
                cardsOffset = 1f;
            }


        }


    }

    public void AdjustCardPositionsInHand()
    {
        if (isMainPlayer)
        {
            cardsInHand.Sort(new CardValueComparer());
        }

        gameLogic.AdjustGameObjectsPositions(cardsInHand, PlayerComponent, cardsOffset, 0.75f);
    }

    [SerializeField]
    private int opposingPlayerNumber = 2;

    [SerializeField]
    private int cardValue = 2;

    [ContextMenu("Ask For Card")]
    void AskForCardFromAnotherPlayer()
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
                if (thisPlayerDetails.playerNumber == 1)
                {
                    card.GetComponent<Card>().ToggleIsFaceUp(true);
                }
                else
                {
                    card.GetComponent<Card>().ToggleIsFaceUp(false);
                }
            });
        }
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
        Vector3 booksWonPosition = booksWonGameObject.transform.position;
        // Instanciate a new Book 
        GameObject newBook = Instantiate(bookPrefab, booksWonPosition, Quaternion.identity);
        newBook.name = "Book" + value;
        float cardsOffsetInbook = newBook.GetComponent<Book>().cardsOffset;

        // Make it child of Books Won
        newBook.transform.SetParent(booksWonGameObject.transform);

        // Move each card to the appropriate position in newBook and shrink their size
        float shrinkScale = 0.5f;
        foreach (GameObject card in cardsToMove.ToList())
        {
            int index = cardsToMove.IndexOf(card);
            StartCoroutine(gameLogic.MoveToDesiredPosition(card, card.transform.position, booksWonPosition, 0.5f));
            StartCoroutine(ChangeCardScale(card, shrinkScale, 0.5f));
            AdjustCardPositionsInHand();
            card.transform.SetParent(newBook.transform, false);
            // cardsInHand.Remove(card);
        }

        // Save cards list in newBook
        newBook.GetComponent<Book>().book = cardsToMove;

        // Add new book to booksWon list in in Books Won game object
        BooksWon booksWon = booksWonGameObject.GetComponent<BooksWon>();
        booksWon.booksWon.Add(newBook);
        booksWon.AdjustBookPositionsInBooksWon();

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
