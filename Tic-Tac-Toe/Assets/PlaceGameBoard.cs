using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaceGameBoard : MonoBehaviour
{
    public GameObject gameBoard;
    
    public GameObject moveBoardButton;
    public GameObject playButton;
    
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    
    private bool isBoardPlaced = false;
    private bool isMoveBoardButtonClick = true;

    // Start is called before the first frame update.
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    // Update is called once per frame.
    void Update()
    {
        if (!isBoardPlaced && !isMoveBoardButtonClick)
        {
            if (Input.touchCount > 0)
            {
                Vector2 touchPosition = Input.GetTouch(0).position;

                // Raycast will return a list of all planes intersected by the ray as well as the intersection point.
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    // The list is sorted by distance so to get the location
                    // of the closest intersection we simply reference hits[0].
                    var hitPose = hits[0].pose;
                    // Now we will activate our game board and place it at the
                    // chosen location.
                    gameBoard.SetActive(true);
                    gameBoard.transform.position = hitPose.position;
                    moveBoardButton.SetActive(true);
                    playButton.SetActive(true);
                    isBoardPlaced = true;
                    // After we have placed the game board we will disable the
                    // planes in the scene as we no longer need them.
                    SetTrackingSurfacesActive(false);
                }
            }
        }
        else
        {
            SetTrackingSurfacesActive(isMoveBoardButtonClick);
            isMoveBoardButtonClick = false;
        }
    }

    // If the user places the game board at an undesirable location we 
    // would like to allow the user to move the game board to a new location.
    public void AllowMoveGameBoard()
    {
        gameBoard.SetActive(false);
        moveBoardButton.SetActive(false);
        playButton.SetActive(false);
        isBoardPlaced = false;
        isMoveBoardButtonClick = true;
    }
    
    private void SetTrackingSurfacesActive(bool activeStatus)
    {
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(activeStatus);
        }
    }

    // Lastly we will later need to allow other components to check whether the
    // game board has been places so we will add an accessor to this.
    public bool Placed()
    {
        return isBoardPlaced;
    }
}