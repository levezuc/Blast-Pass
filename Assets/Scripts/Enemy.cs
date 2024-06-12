using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody rigidBody;

    public AudioSource Sound_Activate;

    public AudioSource Sound_Deactivate;

    float torque = 100.0f;

    float force = 220.0f;

    [HideInInspector]
    public Vector3 FlyDirection = new Vector3(0, 1, 0);

    [HideInInspector]
    public Vector3 TorqueDirection = new Vector3(0, 0, 0);

    [HideInInspector]
    public GameManager gameManager;

    private bool IsActiveEnemy = true;

    void Start()
    {
        Sound_Activate.Play();
    }

    public IEnumerator SetLossTimer(float lossTimer)
    {
        yield return new WaitForSeconds(lossTimer);
        if (IsActiveEnemy && gameManager != null)
        {
            gameManager.LoseGame();
        }
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
        rigidBody.AddForce(FlyDirection * force);
        rigidBody.AddTorque(TorqueDirection * torque);
        rigidBody.useGravity = true;
        IsActiveEnemy = false;
        gameManager.IncreaseScore();
        //Make random volume to decrease tedium
        Sound_Deactivate.volume = 1 / Random.Range(1, 2);
        Sound_Deactivate.Play();
        StartCoroutine(WaitForDelete());
    }

    private IEnumerator WaitForDelete()
    {
        yield return new WaitForSeconds(3f);
        Destroy(this);
    }
}
