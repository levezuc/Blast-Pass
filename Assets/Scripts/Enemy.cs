using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody rigidBody;

    public AudioSource SoundActivate;

    public AudioSource SoundDeactivate;

    private float minTorque = 100.0f;

    private float maxTorque = 200.0f;

    private float maxForce = 300.0f;

    private float minForce = 200.0f;

    private Vector3 FlyDirection = new Vector3(0, 1, 0);

    private Vector3 TorqueDirection = new Vector3(0, 0, 0);

    private GameManager gameManager;

    private int direction;

    private bool IsActiveEnemy = true;

    private float lossTimer = float.MaxValue;

    void Start()
    {
        SoundActivate.Play();
    }

    public void StartUpEnemy(GameManager gameManager, int direction)
    {
        this.gameManager = gameManager;
        SetDirection(direction);
        switch (direction)
        {
            case 0:
                TorqueDirection = new Vector3(1, 0, 0);
                FlyDirection = new Vector3(0, 1, 1);
                break;
            case 1:
                TorqueDirection = new Vector3(0, 0, -1);
                FlyDirection = new Vector3(1, 1, 0);
                break;
            case 2:
                TorqueDirection = new Vector3(-1, 0, 0);
                FlyDirection = new Vector3(0, 1, -1);
                break;
            case 3:
                TorqueDirection = new Vector3(0, 0, 1);
                FlyDirection = new Vector3(-1, 1, 0);
                break;
            default:
                break;
        }
    }

    public IEnumerator SetLossTimer(float lossTimer)
    {
        this.lossTimer = lossTimer;
        yield return new WaitForSeconds(lossTimer);
        if (IsActiveEnemy && gameManager != null)
        {
            gameManager.LoseGame();
        }
    }

    public void SetDirection(int direction)
    {
        this.direction = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            InteractWithPlayer();
        }
    }

    private void InteractWithPlayer()
    {
        rigidBody.AddForce(FlyDirection * Random.Range(minForce, maxForce));
        rigidBody.AddTorque(TorqueDirection * Random.Range(minTorque, maxTorque));
        rigidBody.useGravity = true;
        IsActiveEnemy = false;
        gameManager.IncreaseScore(direction);
        SoundDeactivate.volume = 1 / Random.Range(1, 2);
        SoundDeactivate.Play();
        StartCoroutine(WaitForDelete());
    }

    private IEnumerator WaitForDelete()
    {
        var destroyTimer = 3f;
        if(destroyTimer < lossTimer)
        {
            Debug.LogWarning("destroy timer is less than loss timer");
        }
        yield return new WaitForSeconds(destroyTimer);
        Destroy(this);
    }
}
