using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Hand : MonoBehaviour
{
    public List<GameObject> cardsInHand;
    private int lastCardCount;

    private float cardsOffset = 0.5f;
    private bool cardsOffsetUpdated = false;

    public PlayerDetails thisPlayerDetails;
    public GameObject booksWonGameObject;

    // Start is called before the first frame update
    void Start()
    {
        cardsInHand = new List<GameObject>();
        lastCardCount = cardsInHand.Count;
    }

    void Update()
    {
        // Updates cardOffset once thisPlayerDetails has been assigned
        if (thisPlayerDetails != null && !cardsOffsetUpdated)
        {
            cardsOffset = thisPlayerDetails.playerNumber == 1 ? 1f : 0.5f;
            cardsOffsetUpdated = true;
        }


    }



    public void AdjustCardPositions(float speed)
    {
        if (thisPlayerDetails.playerNumber == 1)
        {
            // Sort the cards by value
            cardsInHand.Sort(new CardValueComparer());
        }



        // Store all desired positions in a list
        List<Vector3> desiredPositions = new List<Vector3>();
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector3 newPosition = generateCardPosition(i, cardsInHand.Count);
            desiredPositions.Add(newPosition);
        }


        // Start coroutine for each card
        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector3 startingPosition = cardsInHand[i].transform.position; // Starting position of the card
            StartCoroutine(MoveToDesiredPosition(cardsInHand[i], startingPosition, desiredPositions[i], speed));
        }
    }


    private Vector3 generateCardPosition(int i, int numberOfCards)
    {

        float yPos = transform.position.y;
        float xPos = (i - (numberOfCards - 1) / 2f) * cardsOffset + transform.position.x;
        float zPos = i + 1;
        return new Vector3(xPos, yPos, zPos);
    }

    public IEnumerator MoveToDesiredPosition(GameObject card, Vector3 startingPosition, Vector3 desiredPosition, float duration)
    {

        float time = 0f; // Time elapsed since the start of the movement



        // Loop until the movement is complete
        while (time < duration)
        {
            // Calculate the new position of the card based on the time elapsed and the desired position
            Vector3 newPosition = Vector3.Lerp(startingPosition, desiredPosition, time / duration);

            // Set the position of the card to the new position
            card.transform.position = newPosition;

            // Increment the time elapsed
            time += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Set the position of the card to the desired position (in case of rounding errors)
        card.transform.position = desiredPosition;

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


    [SerializeField]
    private int opposingPlayerNumber = 2;

    [SerializeField]
    private int cardValue = 2;

    [ContextMenu("Ask For Card")]
    void AskForCardFromAnotherPlayer()
    {
        // Find the hand of the player you want to access using opposingPlayerNumber
        Hand opposingPlayerHand = GameObject.Find("Player " + opposingPlayerNumber).GetComponentInChildren<Hand>();

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
            AdjustCardPositions(0.75f);
            opposingPlayerHand.AdjustCardPositions(0.75f);

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

        // Set the initial position and offset for the cards in Books Won
        Vector3 booksWonPosition = booksWonGameObject.transform.position;
        float xOffset = 0.2f;

        // Move each card to the appropriate position in Books Won and shrink their size
        float shrinkScale = 0.5f;
        foreach (GameObject card in cardsToMove)
        {
            int index = cardsToMove.IndexOf(card);
            Vector3 position = booksWonPosition + new Vector3(index * xOffset, 0, -index);
            StartCoroutine(MoveToDesiredPosition(card, card.transform.position, booksWonPosition, 0.5f));
            StartCoroutine(ChangeCardScale(card, shrinkScale, 0.5f));
            card.transform.SetParent(booksWonGameObject.transform, false);
        }

        // Remove the matching cards from the hand
        cardsInHand.RemoveAll(card => cardsToMove.Contains(card));
        booksWonGameObject.GetComponent<BooksWon>().booksWon.Add(cardsToMove);
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
        AdjustCardPositions(0.75f);
    }


}
