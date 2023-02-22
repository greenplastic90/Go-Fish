using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooksWon : MonoBehaviour
{
    private Movement Movement;
    public List<GameObject> booksWon = new List<GameObject>();
    private float booksOffset = 0.5f;
    private float booksAdjustmentSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        Movement = GameObject.Find("Game Logic").GetComponent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    [ContextMenu("Adjust Books")]
    public IEnumerator AdjustBookPositionsInBooksWon()
    {

        yield return StartCoroutine(Movement.AdjustGameObjectsPositions(booksWon, gameObject, booksOffset, booksAdjustmentSpeed));

    }
}
