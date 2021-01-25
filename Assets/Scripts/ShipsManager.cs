using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShipsManager : MonoBehaviour
{
    public int shipsCount;

    private string shipTag;

    GameObject shipRef, shipObj;

    public GridManager gridManager;

    private Text shipsCountText;

    private float angle = 0;

    private Rotation rotation;

    private MainGameController gameController;

    private int tileX, tileY;

    void Start()
    {
        shipTag = gameObject.tag;
        shipRef = Resources.Load<GameObject>($"Prefabs/Ships/{shipTag}");
        shipsCountText = GameObject.FindGameObjectWithTag($"{tag}sCountText").GetComponent<Text>();
        shipsCountText.text = $"X {shipsCount}";

        gameController = GameObject.FindGameObjectWithTag("MainGameController").GetComponent<MainGameController>();
        gameController.allShipsCount += shipsCount;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (shipObj == null)
                return;
            angle -= 90;
            angle %= 360;
            shipObj.transform.localRotation = Quaternion.identity;
            shipObj.transform.localRotation = Quaternion.Euler(0, 0, angle);
            rotation++;
            rotation = (Rotation)((int)rotation % Enum.GetNames(typeof(Rotation)).Length);
            Debug.Log(rotation);
        }
    }

    private void OnMouseDown()
    {
        if (shipsCount > 0)
        {
            shipObj = Instantiate(shipRef, Input.mousePosition, Quaternion.identity);
            angle = 0;
            rotation = Rotation.Down;
        }
    }

    private void OnMouseDrag()
    {
        if (shipObj == null)
            return;
        shipObj.transform.localPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        shipObj.transform.localPosition = new Vector3(shipObj.transform.localPosition.x, shipObj.transform.localPosition.y, 0.0f);
    }

    private void OnMouseUp()
    {
        if (shipObj == null)
            return;

        

        if (!CheckBorders() || !CheckField())
        {
            Destroy(shipObj);
        }
        else
        {
            SpawnShip();
        }
    }

    private bool CheckBorders()
    {
        float shipX = shipObj.transform.localPosition.x;
        float shipY = shipObj.transform.localPosition.y;

        if (shipX < gridManager.minX || shipY > gridManager.minY || shipX > gridManager.maxX || shipY < gridManager.maxY)
        {
            return false;
        }

        return true;
    }

    private bool CheckField()
    {
        float shipX = shipObj.transform.localPosition.x - gridManager.gameObject.transform.localPosition.x;
        float shipY = shipObj.transform.localPosition.y - gridManager.gameObject.transform.localPosition.y;

        tileX = Mathf.RoundToInt(Mathf.Abs(shipX / gridManager.tileSize));
        tileY = Mathf.RoundToInt(Mathf.Abs(shipY / gridManager.tileSize));

        int shipSize = shipObj.GetComponent<Ship>().shipSize;

        switch (rotation)
        {
            case Rotation.Down:
                for(int i = 0; i < shipSize; i++)
                {
                    try
                    {
                        if (gameController.playerGameField[tileY + i, tileX] != 0)
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            case Rotation.Up:
                for (int i = 0; i < shipSize; i++)
                {
                    try
                    {
                        if (gameController.playerGameField[tileY - i, tileX] != 0)
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            case Rotation.Left:
                for (int i = 0; i < shipSize; i++)
                {
                    try
                    {
                        if (gameController.playerGameField[tileY, tileX - i] != 0)
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            case Rotation.Right:
                for (int i = 0; i < shipSize; i++)
                {
                    try
                    {
                        if (gameController.playerGameField[tileY, tileX + i] != 0)
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return true;
            default:
                return false;
        }
    }

    private void SpawnShip()
    {
        shipObj.transform.parent = gridManager.transform.GetChild(tileY * 10 + tileX);
        shipObj.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        shipObj.GetComponent<Ship>().Spawn(gameController, rotation, tileX, tileY);

        shipObj = null;

        SetShipsCount();
    }

    private void SetShipsCount()
    {
        shipsCount--;
        shipsCountText.text = $"x {shipsCount}";
    }
}
