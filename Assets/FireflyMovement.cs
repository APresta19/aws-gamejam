using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflyMovement : MonoBehaviour
{
    private Camera cam;
    public float verticalMove = 2f;
    public float speed = 1f;

    // New customizable destruction margin and vertical oscillation controls
    public float destructionMargin = 2f;
    public float verticalAmplitude = 2f;
    public float verticalFrequency = 1f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Horizontal movement
        transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);

        // Vertical oscillation (sinusoidal movement)
        float newY = Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Camera bounds for destruction
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        if (transform.position.x < (cam.transform.position.x - camWidth / 2) - destructionMargin)
        {
            Destroy(this.gameObject);  // Destroy if out of bounds
        }
    }
}
