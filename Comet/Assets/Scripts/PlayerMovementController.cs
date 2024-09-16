using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// from https://www.youtube.com/@StringCodeStudios
public class PlayerMovementController : MonoBehaviour
{
    // [SerializeField] float movementSpeed = 1f;

    bool playerIsMoving = false;

    List<Tile> TravelRange = new List<Tile>();
    Vector2Int[] searchOrder = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
    GridManager gridManager;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            bool hasHit = Physics.Raycast(ray, out hit);

            if (hasHit)
            {
                Debug.Log("Raycast hit");
                if(hit.transform.tag == "Tile")
                {
                    Debug.Log("Tagged Tile");
                    if (playerIsMoving)
                    {
                        Debug.Log("Player is moving");
                        Vector2Int targetCords = hit.transform.GetComponent<Labeller>().cords;
                        Vector2 startCords = new Vector2Int((int)transform.position.x,
                            (int)transform.position.y) / gridManager.UnityGridSize;

                        transform.position = new Vector3(targetCords.x, transform.position.y, targetCords.y);
                    }
                }

                if (hit.transform.tag == "Player")
                {
                    Debug.Log("Tagged Player");
                    playerIsMoving = true;
                }
            }
        }
    }


    void HighlightMovementRange(int range)
    {
        // take the passed range value to build the characters movement range
        // start with the players current position and check for available spots
        // in the cardinal directions. For any available spaces found, add them to
        // the list of the spaces in the players current movement range.
        // then check the cardinal directions of the newly added spaces and add tiles
        // next to those ones. Repeat this process for however many times the passed range
        // value indicates.
    }
}
