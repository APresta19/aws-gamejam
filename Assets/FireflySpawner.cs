using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireflySpawner : MonoBehaviour
{
    public Gradient gradient;
    public Camera cam;
    public GameObject fireflyPrefab;
    public int fireflyLimit = 10;
    public float horizontalEndPoint = 2f;
    public float spawnTime = 2f;
    private int amountInScene;

    // New variables for camera bounds
    private float camHeight;
    private float camWidth;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;

        // Ensure Camera.main is assigned correctly
        if (cam == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // Calculate camera bounds once
        camHeight = 2f * cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        StartCoroutine(SpawnFireflies());
    }

    IEnumerator SpawnFireflies()
    {
        while (true)
        {
            // Get the current number of fireflies in the scene
            amountInScene = GameObject.FindGameObjectsWithTag("Firefly").Length;
            if (amountInScene >= fireflyLimit)
            {
                yield return null;
                continue;
            }

            // Wait for the specified spawn time
            yield return new WaitForSeconds(spawnTime);

            // Calculate random position within camera bounds
            float randX = Random.Range(cam.transform.position.x + camWidth / 2, cam.transform.position.x + camWidth / 2 + horizontalEndPoint);
            float randY = Random.Range(cam.transform.position.y - camHeight / 2, cam.transform.position.y + camHeight / 2);
            Vector2 randomPosition = new Vector2(randX, randY);
            GameObject firefly = Instantiate(fireflyPrefab, randomPosition, Quaternion.identity, transform);

            // Make random color
            SpriteRenderer sr = firefly.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color randomColor = gradient.Evaluate(Random.Range(0f, 1f));
                sr.color = randomColor;
            }
        }
    }
}
