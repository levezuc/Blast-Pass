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

    public GameObject GameOverObject;

    [HideInInspector]
    public int Score { private set; get; }

    [HideInInspector]
    public bool isGameActive = true;

    protected float TimerReset = 1f;

    protected float TimeDecay = 0.05f;

    protected float ShortestPossibleTime = 0.5f;

    protected float LossTimer = 1.0f;

    private float currTimer = 0.0f;

    private bool playingGame = true;

    // Start is called before the first frame update
    void Start()
    {
        Score = 0;
        currTimer = TimerReset;
        ScoreText.text = GetScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        if(currTimer <= 0.0f && playingGame)
        {
            currTimer = TimerReset;

            TimerReset = TimerReset - TimeDecay >= ShortestPossibleTime ? TimerReset - TimeDecay : ShortestPossibleTime;

            SpawnEnemy();
        }

        currTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Spawns an enemy at the one of the possible Spawn Locations
    /// </summary>
    private void SpawnEnemy()
    {
        int direction = Random.Range(0, possibleSpawns.Length);
        GameObject newEnemy = Instantiate(Enemy, possibleSpawns[direction].position, Quaternion.identity);
        newEnemy.transform.SetParent(this.transform);
        var enemyScript = newEnemy.GetComponent<Enemy>();
        enemyScript.gameManager = this;
        switch (direction)
        {
            case 0:
                enemyScript.TorqueDirection = new Vector3(1, 0, 0);
                enemyScript.FlyDirection = new Vector3(0, 1, 1);
                break;
            case 1:
                enemyScript.TorqueDirection = new Vector3(0, 0, -1);
                enemyScript.FlyDirection = new Vector3(1, 1, 0);
                break;
            case 2:
                enemyScript.TorqueDirection = new Vector3(-1, 0, 0);
                enemyScript.FlyDirection = new Vector3(0, 1, -1);
                break;
            case 3:
                enemyScript.TorqueDirection = new Vector3(0, 0, 1);
                enemyScript.FlyDirection = new Vector3(-1, 1, 0);
                break;
            default:
                break;
        }
        StartCoroutine(enemyScript.SetLossTimer(LossTimer));
    }

    public void IncreaseScore()
    {
        Score++;
        ScoreText.text = GetScoreText();
    }


    public void LoseGame()
    {
        if (playingGame)
        {
            Debug.Log("You Lost :(");
            playingGame = false;
            Player.SetPlayingGame(false);
            ScoreText.gameObject.SetActive(false);
            GameOverObject.SetActive(true);
            GameOverScoreText.text = GetScoreText();
        }
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    private string GetScoreText()
    {
        return "Score: " + Score;
    }
}
