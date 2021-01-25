using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private MainGameController gameController;
    public int shipSize;

    public void Spawn(MainGameController gameController, Rotation shipRotation, int tileX, int tileY)
    {
        this.gameController = gameController;
        gameController.ShipSpawned(shipSize, shipRotation, tileX, tileY);
    }
}
