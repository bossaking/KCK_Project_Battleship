using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scope : MonoBehaviour
{

    

    private float posX = 0;
    private float maxX = 0;

    private float posY = 0;
    private float maxY = 0;



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveX(float value)
    {
        if (posX + value > maxX || posX + value < 0)
            return;

        posX += value;
        gameObject.transform.localPosition = new Vector2(posX, posY);
    }

    public void MoveY(float value)
    {
        if (posY + value < -maxY || posY + value > 0)
            return;

        posY += value;
        gameObject.transform.localPosition = new Vector2(posX, posY);
    }

    public void SetScope(float maxX, float maxY)
    {
        this.maxX = maxX;
        this.maxY = maxY;
    }
}
