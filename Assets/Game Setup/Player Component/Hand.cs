using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<Card> cardsInHand;
    // Start is called before the first frame update
    void Start()
    {
        cardsInHand = new List<Card>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(Card card)
    {
        cardsInHand.Add(card);

    }
}
