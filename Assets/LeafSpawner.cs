using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafSpawner : MonoBehaviour
{
    public Gradient gradient;
    public Camera cam;
    public GameObject leafPrefab;
    public int leafLimit = 10;
    public float verticalEndPoint = 2f;
    public float spawnTime = 2f;
    private int amountInScene;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        StartCoroutine(SpawnLeaves());
    }
    IEnumerator SpawnLeaves()
    {
        while (true)
        {
            amountInScene = GameObject.FindGameObjectsWithTag("Leaf").Length;
            if (amountInScene >= leafLimit)
            {
                yield return null;
                continue;
            }
            yield return new WaitForSeconds(spawnTime);
            float camHeight = 2f * cam.orthographicSize; //orthographic size is half
            float camWidth = camHeight * cam.aspect; //aspect is width / height so results in width

            // Calculate random position within bounds
            float randX = Random.Range(cam.transform.position.x - camWidth / 2, cam.transform.position.x + camWidth / 2);
            float randY = Random.Range(cam.transform.position.y + camHeight / 2, (cam.transform.position.y + camHeight / 2) + verticalEndPoint);
            Vector2 randomPosition = new Vector2(randX, randY);
            GameObject leaf = Instantiate(leafPrefab, randomPosition, Quaternion.identity, transform);
            Debug.Log("Spawned leaf" + amountInScene);

            //make random color
            SpriteRenderer sr = leaf.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color randomColor = gradient.Evaluate(Random.Range(0f, 1f));
                sr.color = randomColor;
            }
        }
    }
}
