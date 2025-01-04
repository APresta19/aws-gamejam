using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyMovement : MonoBehaviour
{
    private Camera cam;
    public float verticalMove = 2f;
    public float speed = 1f;
    private float desiredY;
    private bool goingUp = true;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        desiredY = transform.position.y + verticalMove;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);

        if (goingUp)
        {
            //transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, desiredY), speed * Time.deltaTime);
        }
        else
        {
            //transform.position = Vector2.Lerp(transform.position, new Vector2(transform.position.x, desiredY - verticalMove), speed * Time.deltaTime);
        }
        
        
        if(goingUp && transform.position.y >= desiredY - .1f)
        {
            //goingUp = false;
        }else if(!goingUp && transform.position.y <= desiredY - verticalMove - .1f)
        {
            //goingUp = true;
        }
        

        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        if (transform.position.x < (cam.transform.position.x - camWidth / 2) - 2)
        {
            Destroy(this.gameObject);
        }
    }
}
