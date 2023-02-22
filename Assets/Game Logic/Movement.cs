using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{


    public IEnumerator AdjustGameObjectsPositions(List<GameObject> objectsList, GameObject parentGameObject, float cardsOffset, float speed)
    {




        // Store all desired positions in a list
        List<Vector3> desiredPositions = new List<Vector3>();
        for (int i = 0; i < objectsList.Count; i++)
        {
            Vector3 newPosition = generateGameObjectPosition(parentGameObject, i, objectsList.Count, cardsOffset);
            desiredPositions.Add(newPosition);
        }


        // Start coroutine for each card
        for (int i = 0; i < objectsList.Count; i++)
        {
            Vector3 startingPosition = objectsList[i].transform.position; // Starting position of the card
            StartCoroutine(MoveToDesiredPosition(objectsList[i], startingPosition, desiredPositions[i], speed));
        }
        yield return new WaitForSeconds(0);
    }


    private Vector3 generateGameObjectPosition(GameObject parentGameObject, int i, int numberOfGameObjects, float gameObjectOffset)
    {

        float yPos = parentGameObject.transform.position.y;
        float xPos = (i - (numberOfGameObjects - 1) / 2f) * gameObjectOffset + parentGameObject.transform.position.x;
        float zPos = 0;
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
}
