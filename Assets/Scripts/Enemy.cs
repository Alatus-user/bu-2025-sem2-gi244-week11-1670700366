using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody rb;
    private GameObject player;
    public float lifeTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lifeTime>=100)
        {
            lifeTime ++;
            Destroy(gameObject);
        }
        
        PlayerPowetUpStun();
        
    }

    void PlayerPowetUpStun()
    {
        if (player.GetComponent<PlayerController>().hasStunPowerUp)
        {
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            Vector3 dir = player.transform.position - transform.position;
            dir.Normalize();
            rb.AddForce(dir * speed);
        }
    }
}