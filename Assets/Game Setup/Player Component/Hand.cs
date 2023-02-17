using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<GameObject> cardsInHand;

    private int lastCardCount;

    private float cardsOffset = 0.5f;

    public PlayerDetails playerDetails;


    // Start is called before the first frame update
    void Start()
    {
        cardsInHand = new List<GameObject>();
        lastCardCount = cardsInHand.Count;

        cardsOffset = playerDetails.playerNumber == 1 ? 1f : 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (cardsInHand.Count != lastCardCount)
        {
            lastCardCount = cardsInHand.Count;
            AdjustCardPositions();
        }

        // cardsInHand.ForEach(card => Debug.Log(card.name + "  " + card.transform.position));

    }

    public void AddCard(GameObject card)
    {
        cardsInHand.Add(card);

    }

    private void AdjustCardPositions()
    {
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
            StartCoroutine(MoveToDesiredPosition(cardsInHand[i], desiredPositions[i]));
        }
    }

    private Vector3 generateCardPosition(int i, int numberOfCards)
    {

        float yPos = transform.position.y;
        float xPos = (i - (numberOfCards - 1) / 2f) * cardsOffset + transform.position.x;
        float zPos = i + 1;
        return new Vector3(xPos, yPos, zPos);
    }



    public IEnumerator MoveToDesiredPosition(GameObject card, Vector3 desiredPosition)
    {
        float duration = 0.5f; // Duration of the movement
        float time = 0f; // Time elapsed since the start of the movement

        Vector3 startingPosition = card.transform.position; // Starting position of the card

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

}
