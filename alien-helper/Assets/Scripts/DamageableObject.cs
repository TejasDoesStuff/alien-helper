using System.Collections;
using UnityEngine;

public class DamageableObject : MonoBehaviour
{
    public int health;
    private Renderer objectRenderer;
    public Color damagedColor = Color.red;
    public Color originalColor = Color.white;

    void Start()
    {
        health = 10;
        objectRenderer = GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
    }

    void Update()
    {

    }

    public void takeDamage(int damage)
    {
        if (health - damage > 0)
        {
            health -= damage;
            StartCoroutine(changeColor());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator changeColor()
    {
        objectRenderer.material.color = damagedColor;
        yield return new WaitForSeconds(0.1f);
        objectRenderer.material.color = originalColor;
    }
}
