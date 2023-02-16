using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public List<GameObject> cardsInHand;
    // Start is called before the first frame update
    void Start()
    {
        cardsInHand = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddCard(GameObject card)
    {
        cardsInHand.Add(card);

    }
}
