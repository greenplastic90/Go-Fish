using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData
{
    public Sprite frontSprite;
    public int value;

}

public class GameSetup : MonoBehaviour
{
    //! Number of players has to be between no less than 2 and no greater than 5
    public int numberOfPlayers;
    public GameObject drawPilePrefab;
    public GameObject playerComponentPrefab;
    public List<GameObject> playerComponents;

    public Vector3 drawPileInitialPosition = new Vector3(-8, -4, 0);

    // Start is called before the first frame update
    void Start()
    {
        playerComponents = new List<GameObject>();
        InstantiatePlayers(numberOfPlayers);
        InstanciateDrawPile();
    }

    // Instanciating players
    void InstantiatePlayers(int numberOfPlayers)
    {
        if (numberOfPlayers < 2 || numberOfPlayers > 5)
        {
            Debug.LogError("Number of players can not be less than 2 or more than 5");
            return;
        }
        // Instantiate the player components based on the number of players
        switch (numberOfPlayers)
        {
            case 2:
                InstantiatePlayer(1, new Vector3(0, -4, 0));          // Bottom center
                InstantiatePlayer(2, new Vector3(0, 4, 0));           // Top center
                break;
            case 3:
                InstantiatePlayer(1, new Vector3(0, -4, 0));          // Bottom center
                InstantiatePlayer(2, new Vector3(-4, 4, 0));          // Top left
                InstantiatePlayer(3, new Vector3(4, 4, 0));           // Top right
                break;
            case 4:
                InstantiatePlayer(1, new Vector3(0, -4, 0));          // Bottom center
                InstantiatePlayer(2, new Vector3(-7, 0, 0));          // Left center
                InstantiatePlayer(3, new Vector3(0, 4, 0));           // Top center
                InstantiatePlayer(4, new Vector3(7, 0, 0));           // Right center
                break;
            case 5:
                InstantiatePlayer(1, new Vector3(0, -4, 0));          // Bottom center
                InstantiatePlayer(2, new Vector3(-7, 0, 0));          // Left center
                InstantiatePlayer(3, new Vector3(-4, 4, 0));          // Top left
                InstantiatePlayer(4, new Vector3(4, 4, 0));           // Top right
                InstantiatePlayer(5, new Vector3(7, 0, 0));           // Right center
                break;
            default:
                Debug.LogError("Unsupported number of players: " + numberOfPlayers);
                break;
        }
    }

    void InstantiatePlayer(int playerNumber, Vector3 position)
    {
        GameObject playerComponent = Instantiate(playerComponentPrefab, position, Quaternion.identity);
        playerComponent.name = "Player " + playerNumber; // uniqe object name
        playerComponent.GetComponent<PlayerDetails>().playerNumber = playerNumber;
        playerComponents.Add(playerComponent);
        // playerComponent.transform.Find("Hand").GetComponent<Hand>().drawCardSpeed = 2f;
        Debug.Log(playerComponent.transform.Find("Hand").GetComponent<Hand>().drawCardSpeed);

    }

    void InstanciateDrawPile()
    {
        GameObject drawPile = Instantiate(drawPilePrefab, drawPileInitialPosition, Quaternion.identity);
        drawPile.name = "Draw Pile";

    }
}
