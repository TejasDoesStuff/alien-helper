using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMove : MonoBehaviour
{
    public float speed = 2f;
    public float changeDirection = 3f;
    private Vector3 moveDirection;
    private float timer;

    private Transform player;

    public bool shouldFollow;
    private Quaternion targetRotation;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        PickNewDirection();
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (shouldFollow && player)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0f;

            targetRotation = Quaternion.LookRotation(directionToPlayer);
        }
        else
        {
            timer += Time.deltaTime;
            if (timer >= changeDirection)
            {
                PickNewDirection();
                timer = 0f;
            }
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * Time.deltaTime);
    }

    public void setShouldFollow(bool sf)
    {
        shouldFollow = sf;
    }

    void PickNewDirection()
    {
        float angle = Random.Range(0f, 360f);
        targetRotation = Quaternion.Euler(0f, angle, 0f);
    }
}
