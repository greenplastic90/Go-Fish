using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    private Movement Movement;
    public List<GameObject> book;
    public float cardsOffset = 0f;
    public float cardAdjustmentSpeed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        Movement = GameObject.Find("Game Logic").GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator AdjustCardsPositionsInBook()
    {

        yield return StartCoroutine(Movement.AdjustGameObjectsPositions(book, gameObject, cardsOffset, cardAdjustmentSpeed));

    }


}
