using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public int numberOfPlayers = 3;

    public GameObject drawPilePrefab;
    public GameObject playerComponentPrefab;

    public List<GameObject> PlayerComponents;
    // Start is called before the first frame update

    void Start()
    {
        PlayerComponents = new List<GameObject>();
        InstantiatePlayers(numberOfPlayers);
        InstantiateDrawPile();
        Debug.Log("List " + PlayerComponents.Count);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void InstantiateDrawPile()
    {
        Instantiate(drawPilePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    void InstantiatePlayer(int playerNumber, Vector3 position)
    {
        GameObject playerComponent = Instantiate(playerComponentPrefab, position, Quaternion.identity);
        playerComponent.GetComponent<PlayerDetails>().playerNumber = playerNumber;
        PlayerComponents.Add(playerComponent);
    }

    void InstantiatePlayers(int numberOfPlayers)
    {
        // Instantiate the player components based on the number of players
        switch (numberOfPlayers)
        {
            case 2:
                InstantiatePlayer(1, new Vector3(0, 0, 0)); // Bottom center
                InstantiatePlayer(2, new Vector3(0, 8, 0));  // Top center
                break;
            case 3:
                InstantiatePlayer(1, new Vector3(0, 0, 0));           // Bottom center
                InstantiatePlayer(2, new Vector3(-4, 8, 0));          // Top left
                InstantiatePlayer(3, new Vector3(4, 8, 0));           // Top right
                break;
            case 4:
                InstantiatePlayer(1, new Vector3(0, 0, 0));           // Bottom center
                InstantiatePlayer(2, new Vector3(0, 8, 0));           // Top center
                InstantiatePlayer(3, new Vector3(-7, 4, 0));          // Left center
                InstantiatePlayer(4, new Vector3(7, 4, 0));           // Right center
                break;
            case 5:
                InstantiatePlayer(1, new Vector3(0, 0, 0));           // Bottom center
                InstantiatePlayer(2, new Vector3(-4, 8, 0));          // Top left
                InstantiatePlayer(3, new Vector3(4, 8, 0));           // Top right
                InstantiatePlayer(4, new Vector3(-7, 4, 0));          // Left center
                InstantiatePlayer(5, new Vector3(7, 4, 0));           // Right center
                break;
            default:
                Debug.LogError("Unsupported number of players: " + numberOfPlayers);
                break;
        }
    }
}
