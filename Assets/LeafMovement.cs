using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafMovement : MonoBehaviour
{
    private Camera cam;
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, speed * Time.deltaTime, 0);

        float camHeight = 2f * cam.orthographicSize;
        if (transform.position.y < (cam.transform.position.y - camHeight / 2) - 2)
        {
            Destroy(this.gameObject);
        }
    }
}
