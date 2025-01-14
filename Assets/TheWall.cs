using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheWall : MonoBehaviour
{
    public Animator winPanel;
    public GameObject[] wallEnemies;
    public int killCount = 0;
    public void CheckForWin(GameObject enemy)
    {
        bool isValidEnemy = false;
        for (int i = 0; i < wallEnemies.Length; i++)
        {
            if (enemy == wallEnemies[i])
            {
                isValidEnemy = true;
            }
        }
        if (isValidEnemy)
        {
            killCount++;
        }
        if (killCount >= wallEnemies.Length)
        {
            winPanel.SetTrigger("Won");
        }
    }
}
