using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooksWon : MonoBehaviour
{
    private GameLogic gameLogic;
    public List<GameObject> booksWon = new List<GameObject>();
    private float booksOffset = 0.5f;
    private float booksAdjustmentSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        gameLogic = GameObject.Find("Game Logic").GetComponent<GameLogic>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void AdjustBooksPositions()
    {
        gameLogic.AdjustGameObjectsPositions(booksWon, gameObject, booksOffset, booksAdjustmentSpeed);
    }
}
