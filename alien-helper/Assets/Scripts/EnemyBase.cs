using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health;
    private Renderer objectRenderer;
    public Color damagedColor = Color.red;
    public Color originalColor = Color.white;

    public GameObject dropPrefab;

    void Start()
    {
        health = 10;
        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
    }

    public virtual void takeDamage(int damage)
    {
        if (health - damage > 0)
        {
            health -= damage;
            StartCoroutine(changeColor());
        }
        else
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Instantiate(dropPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private IEnumerator changeColor()
    {
        objectRenderer.material.color = damagedColor;
        yield return new WaitForSeconds(0.1f);
        objectRenderer.material.color = originalColor;
    }
}
