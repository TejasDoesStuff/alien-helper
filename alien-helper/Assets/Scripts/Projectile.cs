using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    public void Init(Vector3 shootDirection, float projectileSpeed)
    {
        direction = shootDirection;
        speed = projectileSpeed;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("damage"))
        {
            collision.gameObject.GetComponent<DamageableObject>().takeDamage(1);
        }

        Destroy(gameObject);
    }
}
