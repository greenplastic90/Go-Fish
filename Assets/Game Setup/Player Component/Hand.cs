using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<GameObject> cardsInHand;

    private int lastCardCount;

    private float cardsOffset = 0.5f;
    public float returnSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        cardsInHand = new List<GameObject>();
        lastCardCount = cardsInHand.Count;
    }

    // Update is called once per frame
    void Update()
    {
        // if (cardsInHand.Count != lastCardCount)
        // {
        //     lastCardCount = cardsInHand.Count;
        //     AdjustCardPositions();
        // }

        // cardsInHand.ForEach(card => Debug.Log(card.name + "  " + card.transform.position));

    }

    public void AddCard(GameObject card)
    {
        cardsInHand.Add(card);
        AdjustCardPositions();
    }

    private void AdjustCardPositions()
    {

        int playerNumber = transform.parent.GetComponent<PlayerDetails>().playerNumber;

        for (int i = 0; i < cardsInHand.Count; i++)
        {
            Vector3 newPosition = generateCardPosition(i, cardsInHand.Count);
            cardsInHand[i].transform.position = newPosition;
        }


    }
    private Vector3 generateCardPosition(int i, int numberOfCards)
    {
        float yPos = transform.position.y;
        float xPos = (i - (numberOfCards - 1) / 2f) * cardsOffset;
        float zPos = i + 1;
        return new Vector3(xPos, yPos, zPos);
    }



    // public IEnumerator MoveToDesiredPostion(Vector3 desiredPosition)
    // {
    //     while (Vector3.Distance(transform.position, desiredPosition) > 0.1f)
    //     {
    //         transform.position = Vector3.Lerp(transform.position, desiredPosition, returnSpeed * Time.deltaTime);
    //         yield return null;
    //     }
    //     transform.position = desiredPosition;
    // }
}
