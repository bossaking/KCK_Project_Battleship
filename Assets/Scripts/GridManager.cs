using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows = 10;
    public int columns = 10;
    public float tileSize = 0.55f;

    public float minX, minY, maxX, maxY;

    //public delegate void GridLoadHandler();
    //public event GridLoadHandler gridLoaded;

    void Awake()
    {
        GenerateGrid();        
    }

    private void GenerateGrid()
    {

        GameObject refTile = Instantiate(Resources.Load<GameObject>("Tiles/water_tile"));

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject tile = Instantiate(refTile, gameObject.transform);

                float posX = j * tileSize;
                float posY = i * -tileSize;

                tile.transform.localPosition = new Vector3(posX, posY, 0);
            }
        }

        Destroy(refTile);
        
        //float gridW = columns * tileSize;
        //float gridH = rows * tileSize;
        //transform.position = new Vector3(-gridW / 2 + tileSize, gridH / 2 - tileSize / 2, 0);
        CalculateMinMaxField();

    }

    private void CalculateMinMaxField()
    {
        minX = gameObject.transform.localPosition.x - tileSize / 2;
        minY = gameObject.transform.localPosition.y + tileSize / 2;

        maxX = minX + tileSize * columns;
        maxY = minY - tileSize * rows;
    }

}
