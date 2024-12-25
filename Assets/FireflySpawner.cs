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
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        StartCoroutine(SpawnFireflies());
    }
    IEnumerator SpawnFireflies()
    {
        while (true)
        {
            amountInScene = GameObject.FindGameObjectsWithTag("Firefly").Length;
            if (amountInScene >= fireflyLimit)
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(spawnTime);
            float camHeight = 2f * cam.orthographicSize; //orthographic size is half
            float camWidth = camHeight * cam.aspect; //aspect is width / height so results in width

            // Calculate random position within bounds
            float randX = Random.Range(cam.transform.position.x + camWidth / 2, cam.transform.position.x + camWidth / 2 + horizontalEndPoint);
            float randY = Random.Range(cam.transform.position.y - camHeight / 2, cam.transform.position.y + camHeight / 2);
            Vector2 randomPosition = new Vector2(randX, randY);
            GameObject firefly = Instantiate(fireflyPrefab, randomPosition, Quaternion.identity, transform);

            //make random color
            SpriteRenderer sr = firefly.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color randomColor = gradient.Evaluate(Random.Range(0f, 1f));
                sr.color = randomColor;
            }
        }
    }
}
