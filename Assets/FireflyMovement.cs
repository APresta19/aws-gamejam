using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyMovement : MonoBehaviour
{
    private Camera cam;
    public float verticalMove = 2f;
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float desiredY = desiredY = transform.position.y + verticalMove;
        transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        if (transform.position.x < (cam.transform.position.x - camWidth / 2) - 2)
        {
            Destroy(this.gameObject);
        }
    }
}
