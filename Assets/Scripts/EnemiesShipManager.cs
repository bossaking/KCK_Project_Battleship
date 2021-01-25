using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EnemiesShipManager : MonoBehaviour
{
    private MainGameController gameController;
    public int shipsCount = 10;
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemyShips()
    {
        gameController = gameObject.GetComponent<MainGameController>();
        List<TextAsset> levels = new List<TextAsset>();
        levels.AddRange(Resources.LoadAll<TextAsset>("Levels"));
        string level = levels[UnityEngine.Random.Range(0, levels.Count)].text;

        string[] levelRows = level.Split('\n');

        for(int i = 0; i < levelRows.Length; i++)
        {
            for(int j = 0; j < 10; j++)
            {
                gameController.enemyGameField[i, j] = (int)char.GetNumericValue(levelRows[i][j]);
            }
        }

    }
}
