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


    void CheckForFourOfAKinds()
    {
        //todo loop over cardsInHand

        //todo get all values

        //todo IF there are 4 of the same value
        //todo move cards and shrink their size as they go to Books Won GameObject
        //todo we remove all Cards from cardsInHand with said value
        //todo increment books won by 1

        //todo ELSE contiune to next action

    }
    [ContextMenu("Check For 4 Of A Kind")]
    public void CheckForFourOfAKind()
    {
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
            Debug.Log("Value => " + value);
            if (valueCounts[value] >= 4)
            {
                Debug.Log("Four of a Kind Found ==> " + value);
            }
        }

        Debug.Log("No Four Of A Kinds Found");

    }


}
