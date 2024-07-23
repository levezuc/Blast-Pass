using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform[] possibleSpawns;

    public GameObject Enemy;

    public Player Player;

    [Header("Score and Game Over")]
    public TMP_Text ScoreText;

    public TMP_Text GameOverScoreText;

    public GameObject GameOverTextObject;

    public TMP_Text CountDownText;

    public GameObject CountDownTextObject;

    [HideInInspector]
    public int enemyCount = 0;

    [HideInInspector]
    public int Score { private set; get; }

    [HideInInspector]
    public bool isGameActive = true;

    protected float StartTimer = 3.0f;

    protected float TimerReset = 1f;

    protected float TimeDecay = 0.05f;

    protected float ShortestPossibleTime = 0.25f;

    protected float LossTimer = 1.0f;

    protected bool[] occupiedSpawns;

    private float currTimer = 0.0f;

    private int lastEnemyLocation = -1;

    private enum GameState
    {
        Starting,
        Playing,
        Ending
    };

    private GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        GameOverTextObject.SetActive(false);
        Score = 0;
        gameState = GameState.Starting;
        Player.SetPlayingGame(false);
        occupiedSpawns = new bool[possibleSpawns.Length];
        currTimer = StartTimer;
        ScoreText.text = GetScoreText();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameState == GameState.Playing)
        {
            HandlePlayingState();
        }
        else if (gameState == GameState.Starting)
        {
            HandleStartingState();
        }

        currTimer -= Time.deltaTime;
    }

    private void HandlePlayingState()
    {
        if (currTimer <= 0.0f)
        {
            currTimer = TimerReset;

            TimerReset = TimerReset - TimeDecay >= ShortestPossibleTime ? TimerReset - TimeDecay : ShortestPossibleTime;

            SpawnEnemy();
        }
    }

    private void HandleStartingState()
    {
        if (currTimer <= 0.0f)
        {
            gameState = GameState.Playing;
            Player.SetPlayingGame(true);
            CountDownTextObject.gameObject.SetActive(false);
        }
        else
        {
            double dCurrTimer = Math.Ceiling(currTimer);
            CountDownText.text = dCurrTimer.ToString();
        }
    }

    /// <summary>
    /// Spawns an enemy at the one of the possible Spawn Locations
    /// </summary>
    private void SpawnEnemy()
    {
        int direction = GetRandomAvailableSpawnDirection();

        if(direction == -1)
        {
            LoseGame();
            return;
        }

        occupiedSpawns[direction] = true;
        enemyCount++;

        GameObject newEnemy = Instantiate(Enemy, possibleSpawns[direction].position, Quaternion.identity);
        newEnemy.transform.SetParent(this.transform);
        var enemyScript = newEnemy.GetComponent<Enemy>();
        enemyScript.StartUpEnemy(this, direction);

        StartCoroutine(enemyScript.SetLossTimer(LossTimer));
    }

    public void IncreaseScore(int direction)
    {
        Score++;
        enemyCount--;
        lastEnemyLocation = direction;
        occupiedSpawns[direction] = false;
        ScoreText.text = GetScoreText();
    }


    public void LoseGame()
    {
        if (gameState == GameState.Playing)
        {
            gameState = GameState.Ending;
            Player.SetPlayingGame(false);
            ScoreText.gameObject.SetActive(false);
            GameOverTextObject.SetActive(true);
            GameOverScoreText.text = GetScoreText();
        }
    }


    private string GetScoreText()
    {
        return "Score: " + Score;
    }

    private int GetRandomAvailableSpawnDirection()
    {
        List<int> openSpawns = new List<int>();

        for(int i = 0; i < occupiedSpawns.Length; i++)
        {
            if (!occupiedSpawns[i] && lastEnemyLocation != i)
            {
                openSpawns.Add(i);
            }
        }

        if(openSpawns.Count == 0)
        {
            return -1;
        }

        return openSpawns[UnityEngine.Random.Range(0, openSpawns.Count)];
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
