using UnityEngine;

public class Player : MonoBehaviour
{
    [Tooltip("0 North, 1 East, 2 South, 3 West")]
    public Transform[] Positions;

    public Transform StartPosition;

    public AudioSource Sound_Move;

    public int speed = 1;

    public GameManager GameManager;

    protected int currPositionIndex = 1;

    private int currDirection = -1;

    private bool playingGame = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (playingGame)
        {
            HandleMovementInput();
        }
        else
        {
            transform.position = StartPosition.position;
            currDirection = -1;
        }
    }

    public void SetPlayingGame(bool _playingGame)
    {
        playingGame = _playingGame;
    }

    protected void HandleMovementInput()
    {
        var result = Vector3.zero;
        if (Input.GetKey(KeyCode.A))
        {
            UpdatePostition(3);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            UpdatePostition(1);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            UpdatePostition(0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            UpdatePostition(2);
        }
        else
        {
            transform.position = StartPosition.position;
            currDirection = -1;
        }
    }

    protected void UpdatePostition(int direction)
    {
        if (direction >= 0 && direction < Positions.Length)
        {
            bool isNewDirection = currDirection != direction && currDirection == -1;
            bool isCurrDirection = direction == currDirection;
            if (isNewDirection || isCurrDirection)
            {
                if (isNewDirection)
                {
                    Sound_Move.PlayOneShot(Sound_Move.clip);
                    currDirection = direction;
                }

                MoveInDirection(direction);
            }
        }
    }

    protected void MoveInDirection(int direction)
    {
        var result = Vector3.zero;
        switch (direction)
        {
            case 0:
                result.z = (transform.position.z + (0.1f * speed)) <= Positions[direction].transform.position.z ? result.z += (0.1f * speed) : 0;
                break;
            case 1:
                result.x = (transform.position.x + (0.1f * speed)) <= Positions[direction].transform.position.x ? result.x += (0.1f * speed) : 0;
                break;
            case 2:
                result.z = (transform.position.z - (0.1f * speed)) >= Positions[direction].transform.position.z ? result.z -= (0.1f * speed) : 0;
                break;
            case 3:
                result.x = (transform.position.x - (0.1f * speed)) >= Positions[direction].transform.position.x ? result.x -= (0.1f * speed) : 0;
                break;
            default:
                break;
        }

        transform.position += result; 
    }
}
