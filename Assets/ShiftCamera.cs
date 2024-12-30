using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftCamera : MonoBehaviour
{
    public Camera cam;
    public float camSpeed = 1f;
    public float playerBlockDist = 1;
    private bool shouldMove;
    private PlayerMovement pMove;

    private Vector3 desiredPos;
    public enum Direction { Horizontal, Vertical }
    public Direction direction;
    // Start is called before the first frame update
    void Start()
    {
        pMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        if(pMove != null)
        {
            pMove.cameraBlockPlayerDist = playerBlockDist;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (shouldMove)
        {
            pMove.canMove = false;
            float camHeight = 2f * cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;

            if (direction == Direction.Horizontal) 
            {
                desiredPos = new Vector3(transform.position.x + camWidth / 2, cam.transform.position.y, cam.transform.position.z);
            }
            else
            {
                desiredPos = new Vector3(cam.transform.position.x, transform.position.y + camHeight / 2, cam.transform.position.z);
            }

            cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPos, camSpeed * Time.deltaTime);
            if(Vector3.Distance(cam.transform.position, desiredPos) < .5f)
            {
                shouldMove = false;
                pMove.canMove = true;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            shouldMove = true;
        }
    }
    private void OnDrawGizmos()
    {
        if(direction == Direction.Horizontal)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
            Gizmos.color = Color.blue;
            float camHeight = 2f * cam.orthographicSize;
            float camWidth = camHeight * cam.aspect;
            Vector3 desiredPos = new Vector3(transform.position.x + camWidth, transform.position.y, cam.transform.position.z);
            Gizmos.DrawWireSphere(desiredPos, 0.5f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);

            Gizmos.color = Color.blue;
            float camHeight = 2f * cam.orthographicSize;
            Vector3 desiredPos = new Vector3(transform.position.x, transform.position.y + camHeight, cam.transform.position.z);
            Gizmos.DrawWireSphere(desiredPos, 0.5f);
        }
        
    }
}
