using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private Rigidbody rb;

    public void Init(Vector3 shootDirection, float projectileSpeed)
    {
        direction = shootDirection;
        speed = projectileSpeed;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("damage"))
        {
            collision.gameObject.GetComponent<DamageableObject>().takeDamage(1);
        }

        Debug.Log("Hit object: " + collision.gameObject.name + " with tag: " + collision.gameObject.tag);

        if (!collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("bullet"))
        {
            Destroy(gameObject);
        }
    }
}
