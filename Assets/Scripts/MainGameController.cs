using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainGameController : MonoBehaviour
{
    //GRID
    public GridManager gridManager;
    public EnemyGridManager enemyGridManager;

    private EnemiesShipManager enemiesShipManager;

    //SCOPE
    private GameObject scopeRef;
    private Scope scope;

    private Scope enemyScope;

    public int allShipsCount = 0;
    private int allShipsPieces = 0;

    public int[,] playerGameField, enemyGameField;

    public Button playButton;

    public GameObject textsPanel, shipsButtonsPanel;

    private GameObject waterSplashEffect, scopeObj, enemyScopeObj, redDot, hitEffect, redCross;

    private GameObject[] ships;

    private bool playerMove = false;
    private bool pause = false;

    public Text turnText;

    public GameObject gameInfoPanel, pausePanel;

    void Awake()
    {
        scopeRef = Resources.Load<GameObject>("Prefabs/Scope");
        waterSplashEffect = Resources.Load<GameObject>("Prefabs/Effects/WaterSplashEffect");
        hitEffect = Resources.Load<GameObject>("Prefabs/Effects/HitEffect");
        redDot = Resources.Load<GameObject>("Prefabs/RedDot");
        redCross = Resources.Load<GameObject>("Prefabs/RedCross");

        ships = Resources.LoadAll<GameObject>("Prefabs/Ships");

        playerGameField = new int[gridManager.rows, gridManager.columns];
        enemyGameField = new int[gridManager.rows, gridManager.columns];

        enemiesShipManager = GetComponent<EnemiesShipManager>();

        //SpawnEnemyShips();

        playButton.interactable = false;
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
            SetCursorVisibility();
            pause = true;
        }

        if (!playerMove || pause)
            return;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            scope.MoveY(gridManager.tileSize);
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            scope.MoveY(-gridManager.tileSize);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            scope.MoveX(-gridManager.tileSize);
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            scope.MoveX(gridManager.tileSize);
        }
        if (Input.GetMouseButtonUp(0))
        {
            float scopeX = scopeObj.transform.localPosition.x;
            float scopeY = scopeObj.transform.localPosition.y;

            int tileX = Mathf.RoundToInt(Mathf.Abs(scopeX / enemyGridManager.tileSize));
            int tileY = Mathf.RoundToInt(Mathf.Abs(scopeY / enemyGridManager.tileSize));

            StartCoroutine(PlayerShoot(tileX, tileY));
        }
    }

    private IEnumerator PlayerShoot(int tileX, int tileY)
    {
        if (enemyGameField[tileY, tileX] >= 0)
        scope.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        int childPosition = tileY * 10 + tileX;

        if (enemyGameField[tileY, tileX] == 0)
        {
            GameObject waterSplashEffectObj = Instantiate(waterSplashEffect, enemyGridManager.transform.GetChild(childPosition));
            waterSplashEffectObj.transform.localPosition = new Vector3(0, 0, 0);


            yield return new WaitForSeconds(1f);
            GameObject redDotObj = Instantiate(redDot, enemyGridManager.transform.GetChild(childPosition));
            redDotObj.transform.localPosition = new Vector3(0, 0, 0);

            enemyGameField[tileY, tileX] = -1;

            EnemyMove();
        }

        if (enemyGameField[tileY, tileX] > 0)
        {
            GameObject hitEffectObj = Instantiate(hitEffect, enemyGridManager.transform.GetChild(childPosition));
            hitEffectObj.transform.localPosition = new Vector3(0, 0, 0);

            yield return new WaitForSeconds(1f);
            GameObject redCrossObj = Instantiate(redCross, enemyGridManager.transform.GetChild(childPosition));
            redCrossObj.transform.localPosition = new Vector3(0, 0, 0);

            int shipSize = enemyGameField[tileY, tileX];

            enemyGameField[tileY, tileX] = -2;

            CheckShip(shipSize, tileX, tileY);

            PlayerMove();
        }
    }

    private IEnumerator EnemyShoot(int tileX, int tileY)
    {
        int childPosition = tileY * 10 + tileX;

        ShowEnemyScope(childPosition);

        yield return new WaitForSeconds(1f);

        

        if (playerGameField[tileY, tileX] == 0 || playerGameField[tileY, tileX] == -1)
        {
            GameObject waterSplashEffectObj = Instantiate(waterSplashEffect, gridManager.transform.GetChild(childPosition));
            waterSplashEffectObj.transform.localPosition = new Vector3(0, 0, 0);


            yield return new WaitForSeconds(1f);
            GameObject redDotObj = Instantiate(redDot, gridManager.transform.GetChild(childPosition));
            redDotObj.transform.localPosition = new Vector3(0, 0, 0);

            playerGameField[tileY, tileX] = -2;

            //EnemyMove();
            PlayerMove();
        }

        if(playerGameField[tileY, tileX] > 0)
        {
            GameObject hitEffectObj = Instantiate(hitEffect, gridManager.transform.GetChild(childPosition));
            hitEffectObj.transform.localPosition = new Vector3(0, 0, 0);

            yield return new WaitForSeconds(1f);
            GameObject redCrossObj = Instantiate(redCross, gridManager.transform.GetChild(childPosition));
            redCrossObj.transform.localPosition = new Vector3(0, 0, 0);

            playerGameField[tileY, tileX] = -2;
            allShipsPieces--;
            if (allShipsPieces == 0)
            {
                GameOver(true);
                yield break;
            }

            EnemyMove();
        }
    }

    private void EnemyMove()
    {
        playerMove = false;
        turnText.text = "COMPUTER TURN";
        while (true)
        {
            int x = UnityEngine.Random.Range(0, gridManager.rows);
            int y = UnityEngine.Random.Range(0, gridManager.columns);

            if(playerGameField[y, x] != -2)
            {
                StartCoroutine(EnemyShoot(x, y));
                break;
            }
        }

        
    }

    private void CheckShip(int size, int tileX, int tileY)
    {
        int count = 0;
        for (int i = 0; i < size; i++)
        {
            if (tileY - i >= 0 && enemyGameField[tileY - i, tileX] < -1)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = 1; i < size; i++)
        {
            if (tileY + i < gridManager.rows && enemyGameField[tileY + i, tileX] < -1)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = 1; i < size; i++)
        {
            if (tileX - i >= 0 && enemyGameField[tileY, tileX - i] < -1)
            {
                count++;
            }
            else
            {
                break;
            }
        }
        for (int i = 1; i < size; i++)
        {
            if (tileX + i < gridManager.columns && enemyGameField[tileY, tileX + i] < -1)
            {
                count++;
            }
            else
            {
                break;
            }
        }

        if (count == size)
        {
            enemiesShipManager.shipsCount--;
            if (enemiesShipManager.shipsCount == 0)
                GameOver(false);

            int childPosition = tileY * 10 + tileX;

            GetRotationAndSpawn(size, tileX, tileY);
        }
    }

    private void SpawnDestroyedShip(int childPosition, int size, int angle)
    {

        GameObject shipObj = new GameObject();
        switch (size)
        {
            case 1:
                shipObj = Instantiate(ships[2], enemyGridManager.transform.GetChild(childPosition));
                break;
            case 2:
                shipObj = Instantiate(ships[1], enemyGridManager.transform.GetChild(childPosition));
                break;
            case 3:
                shipObj = Instantiate(ships[0], enemyGridManager.transform.GetChild(childPosition));
                break;
            case 4:
                shipObj = Instantiate(ships[3], enemyGridManager.transform.GetChild(childPosition));
                break;
        }
        shipObj.transform.localPosition = new Vector3(0, 0, 0);
        shipObj.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void GetRotationAndSpawn(int size, int tileX, int tileY)
    {

        int childPosition = 0;
        int angle = 0;

        if (size == 1)
        {
            childPosition = tileY * 10 + tileX;
            SpawnDestroyedShip(childPosition, size, angle);
            return;
        }



        if ((tileY - 1 >= 0 && enemyGameField[tileY - 1, tileX] < -1) || (tileY + 1 < gridManager.rows && enemyGameField[tileY + 1, tileX] < -1))
        {
            angle = 0;
            while (tileY - 1 >= 0 && enemyGameField[tileY - 1, tileX] < -1)
            {
                tileY--;
            }
        }
        else
        {
            angle = 90;
            while(tileX - 1 >= 0 && enemyGameField[tileY, tileX - 1] < -1)
            {
                tileX--;
            }
        }

        childPosition = tileY * 10 + tileX;
        SpawnDestroyedShip(childPosition, size, angle);

    }

    private void ShowScope()
    {
        if (scopeObj != null)
        {
            scope.gameObject.SetActive(true);
            return;
        }
            

        scopeObj = Instantiate(scopeRef, enemyGridManager.gameObject.transform.GetChild(0).transform);
        scope = scopeObj.GetComponent<Scope>();
        scope.SetScope(enemyGridManager.tileSize * enemyGridManager.columns, enemyGridManager.tileSize * enemyGridManager.rows);
    }

    private void ShowEnemyScope(int childPosition)
    {
        enemyScopeObj = Instantiate(scopeRef, gridManager.gameObject.transform.GetChild(childPosition).transform);
        ShowEnemyScopeAnimation();
        Destroy(enemyScopeObj, 1f);
    }

    public void ShipSpawned(int shipSize, Rotation shipRotation, int tileX, int tileY)
    {
        allShipsPieces += shipSize;
        int x = 0;
        int y = 0;
        x = tileY;
        y = tileX;
        tileX = x;
        tileY = y;
        switch (shipRotation)
        {
            case Rotation.Down:
                for (int i = tileY - 1; i < tileY + 2; i++)
                {
                    try
                    {
                        playerGameField[tileX - 1, i] = -1;
                    }
                    catch
                    {

                    }
                }
                for (int i = 0; i < shipSize; i++)
                {
                    playerGameField[tileX + i, tileY] = 1;
                    if (tileY - 1 >= 0)
                    {
                        playerGameField[tileX + i, tileY - 1] = -1;
                    }
                    if (tileY + 1 < gridManager.columns)
                    {
                        playerGameField[tileX + i, tileY + 1] = -1;
                    }
                }
                for (int i = tileY - 1; i < tileY + 2; i++)
                {
                    try
                    {
                        playerGameField[tileX + shipSize, i] = -1;
                    }
                    catch
                    {

                    }
                }
                break;
            case Rotation.Left:
                for (int i = tileX - 1; i < tileX + 2; i++)
                {
                    try
                    {
                        playerGameField[i, tileY + 1] = -1;
                    }
                    catch
                    {

                    }
                }
                for (int i = 0; i < shipSize; i++)
                {
                    playerGameField[tileX, tileY - i] = 1;
                    if (tileX - 1 >= 0)
                    {
                        playerGameField[tileX - 1, tileY - i] = -1;
                    }
                    if (tileX + 1 < gridManager.rows)
                    {
                        playerGameField[tileX + 1, tileY - i] = -1;
                    }
                }
                for (int i = tileX - 1; i < tileX + 2; i++)
                {
                    try
                    {
                        playerGameField[i, tileY - shipSize] = -1;
                    }
                    catch
                    {

                    }
                }
                break;
            case Rotation.Up:
                for (int i = tileY - 1; i < tileY + 2; i++)
                {
                    try
                    {
                        playerGameField[tileX + 1, i] = -1;
                    }
                    catch
                    {

                    }
                }
                for (int i = 0; i < shipSize; i++)
                {
                    playerGameField[tileX - i, tileY] = 1;
                    if (tileY - 1 >= 0)
                    {
                        playerGameField[tileX - i, tileY - 1] = -1;
                    }
                    if (tileY + 1 < gridManager.columns)
                    {
                        playerGameField[tileX - i, tileY + 1] = -1;
                    }
                }
                for (int i = tileY - 1; i < tileY + 2; i++)
                {
                    try
                    {
                        playerGameField[tileX - shipSize, i] = -1;
                    }
                    catch
                    {

                    }
                }
                break;
            case Rotation.Right:
                for (int i = tileX - 1; i < tileX + 2; i++)
                {
                    try
                    {
                        playerGameField[i, tileY - 1] = -1;
                    }
                    catch
                    {

                    }
                }
                for (int i = 0; i < shipSize; i++)
                {
                    playerGameField[tileX, tileY + i] = 1;
                    if (tileX - 1 >= 0)
                    {
                        playerGameField[tileX - 1, tileY + i] = -1;
                    }
                    if (tileX + 1 < gridManager.rows)
                    {
                        playerGameField[tileX + 1, tileY + i] = -1;
                    }
                }
                for (int i = tileX - 1; i < tileX + 2; i++)
                {
                    try
                    {
                        playerGameField[i, tileY + shipSize] = -1;
                    }
                    catch
                    {

                    }
                }
                break;
        }

        allShipsCount--;
        if (allShipsCount == 0)
            playButton.interactable = true;
    }

    public void StartGame()
    {
        textsPanel.SetActive(false);
        SetCursorVisibility();
        Invoke(nameof(ShowShipsButtonsPanelAnimation), 0.5f);
        Invoke(nameof(ShowPlayerGridAnimation), 0.5f);
        Invoke(nameof(ShowEnemyGridAnimation), 0.5f);
    }

    #region animations
    private void ShowShipsButtonsPanelAnimation()
    {
        shipsButtonsPanel.GetComponent<Animator>().SetTrigger("Hide");
    }

    private void ShowPlayerGridAnimation()
    {
        gridManager.GetComponent<Animator>().SetTrigger("LTR");
    }

    private void ShowEnemyGridAnimation()
    {
        enemyGridManager.GetComponent<Animator>().SetTrigger("RTL");
        PlayerMove();
    }

    private void ShowEnemyScopeAnimation()
    {
        enemyScopeObj.GetComponent<Animator>().SetTrigger("Show");
    }

    #endregion

    private void PlayerMove()
    {
        playerMove = true;
        turnText.text = "YOUR TURN";
        ShowScope();
    }

    private void SetCursorVisibility()
    {
        Cursor.visible = !Cursor.visible;
    }

    private void GameOver(bool playerLose)
    {
        SetCursorVisibility();
        if (playerLose)
        {
            gameInfoPanel.transform.GetChild(2).GetComponent<Text>().text = "YOU LOSE :(";

        }
        else
        {
            gameInfoPanel.transform.GetChild(2).GetComponent<Text>().text = "YOU WIN :)";
        }

        gameInfoPanel.SetActive(true);
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        SetCursorVisibility();
        Time.timeScale = 1;
        Invoke(nameof(PauseOff), 0.5f);
    }

    private void PauseOff()
    {
        pause = false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void SpawnEnemyShips()
    {
        enemiesShipManager.SpawnEnemyShips();
    }
}
