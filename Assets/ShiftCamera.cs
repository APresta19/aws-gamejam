using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ShiftCamera : MonoBehaviour
{
    public Camera cam;
    public float camSpeed = 1f;
    public float camSize = 8f;
    private GameObject background;

    private bool shouldIncreaseSize;
    public float playerBlockDist = 1f;
    private bool shouldMove;
    private PlayerMovement pMove;
    private Transform jumpPoint; //vertical camera shift drags player up - need an "animation" so they go to a certain point smoother
    private float jumpSpeed;
    private float jumpProgress;
    private bool isJumping;
    public Vector3 offset;

    private Vector3 desiredPos;
    public enum Direction { Left, Right, Up, Down }
    public Direction direction;
    public bool canGoBack = false;
    private bool isGoingBack;
    private Vector3 lastPos;

    void Start()
    {
        background = GameObject.Find("Sky");
        pMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if (pMove != null)
        {
            pMove.cameraBlockPlayerDist = playerBlockDist;
        }
        if (transform.childCount > 0)
        {
            jumpPoint = transform.GetChild(0);
        }
    }
    void Update()
    {
        //make player jump to a jump point
        //creates an "animation"
        if (jumpPoint != null)
        {
            /*if (isJumping && jumpProgress < 1)
            {
                jumpProgress += Time.deltaTime;
                jumpSpeed = pMove.GetComponent<Rigidbody2D>().velocity.y;
                Vector2 midpoint = (pMove.transform.position + jumpPoint.position) / 2;
                midpoint = Vector2.up * 2f;
                pMove.transform.position = Vector3.Slerp(pMove.transform.position, midpoint, (jumpSpeed/2) * Time.deltaTime);
            }
            else
                isJumping = false;*/
        }
    }
    void LateUpdate()
    {
        if (shouldMove)
        {
            pMove.canMove = false;

            float camHeight = 2f * cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            //determine desired camera position based on direction
            switch (direction)
            {
                case Direction.Left:
                    desiredPos = new Vector3(transform.position.x - camWidth / 2, cam.transform.position.y, cam.transform.position.z);
                    break;
                case Direction.Right:
                    desiredPos = new Vector3(transform.position.x + camWidth / 2, cam.transform.position.y, cam.transform.position.z);
                    break;
                case Direction.Up:
                    desiredPos = new Vector3(cam.transform.position.x, transform.position.y + camHeight / 2, cam.transform.position.z);
                    //isJumping = true;
                    break;
                case Direction.Down:
                    desiredPos = new Vector3(cam.transform.position.x, transform.position.y - camHeight / 2, cam.transform.position.z);
                    break;
            }
            //move camera smoothly
            cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPos + offset, camSpeed * Time.deltaTime);

            //increase camera size smoothly
            if (shouldIncreaseSize)
            {
                background.transform.localScale = Vector3.Lerp(background.transform.localScale, new Vector3(background.transform.localScale.x * (camSize / 8), background.transform.localScale.y * (camSize / 8), 1), camSpeed * Time.deltaTime);
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, camSize, camSpeed * Time.deltaTime);
                if (Mathf.Abs(cam.orthographicSize - camSize) < 0.1f)
                {
                    shouldIncreaseSize = false;
                }
            }

            //check if the camera has reached the desired position
            if (Vector3.Distance(cam.transform.position, desiredPos) < 0.5f)
            {
                shouldMove = false;
                pMove.canMove = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !shouldMove)
        {
            Debug.Log("Should move");
            if (!isGoingBack)
            {
                lastPos = cam.transform.position;
            }

            shouldMove = true;
            Vector3 playerPos = other.transform.position;
            //determine the direction for moving back or forward
            if (isGoingBack)
            {
                desiredPos = lastPos; 
                isGoingBack = false;
            }
            else
            {
                // Set the desired position for forward movement based on direction
                float camHeight = 2f * cam.orthographicSize;
                float camWidth = camHeight * cam.aspect;
                shouldIncreaseSize = true;

                switch (direction)
                {
                    case Direction.Left:
                        desiredPos = new Vector3(transform.position.x - camWidth / 2, cam.transform.position.y, cam.transform.position.z);
                        break;
                    case Direction.Right:
                        desiredPos = new Vector3(transform.position.x + camWidth / 2, cam.transform.position.y, cam.transform.position.z);
                        break;
                    case Direction.Up:
                        desiredPos = new Vector3(cam.transform.position.x, transform.position.y + camHeight / 2, cam.transform.position.z);
                        break;
                    case Direction.Down:
                        desiredPos = new Vector3(cam.transform.position.x, transform.position.y - camHeight / 2, cam.transform.position.z);
                        break;
                }
                //check if the player is backtracking through the trigger
                if (Vector3.Distance(playerPos, lastPos) < Vector3.Distance(playerPos, desiredPos))
                {
                    isGoingBack = true;
                    desiredPos = lastPos;
                }
            }
        }
    }
    public void TriggerGoBack()
    {
        isGoingBack = true; // Set the flag to indicate a return action
        shouldMove = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        Gizmos.color = Color.blue;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        Vector3 debugPos = Vector3.zero;
        switch (direction)
        {
            case Direction.Left:
                desiredPos = new Vector3(transform.position.x - camWidth, transform.position.y, cam.transform.position.z);
                break;
            case Direction.Right:
                desiredPos = new Vector3(transform.position.x + camWidth, transform.position.y, cam.transform.position.z);
                break;
            case Direction.Up:
                desiredPos = new Vector3(transform.position.x, transform.position.y + camHeight, cam.transform.position.z);
                break;
            case Direction.Down:
                desiredPos = new Vector3(transform.position.x, transform.position.y - camHeight, cam.transform.position.z);
                break;
        }

        Gizmos.DrawWireSphere(desiredPos, 0.5f);
    }
}