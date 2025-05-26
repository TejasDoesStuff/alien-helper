using System.Collections;
using System.Threading;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 1f;
    public Camera playerCamera;

    public DeckManager deckManager;
    public PlayerMovement playerMovement;
    public CardManager cardManager;

    private bool inputLocked = false;

    void Update()
    {
        if (Input.GetMouseButton(0) && !inputLocked)
        {
            StartCoroutine(LockInputBriefly());
            tryUsing();
        }
    }

    void tryUsing()
    {
        Card selectedCard = deckManager.getCard();
        if (selectedCard != null)
        {
            if (selectedCard.IsOnCooldown())
            {
                return;
            }

            if (selectedCard.use())
            {
                useCard(selectedCard);
                cardManager.UpdateHandUI();
            }
            else
            {
                Debug.Log("Card has no uses left");
            }
        }
        else
        {
            Debug.Log("No valid card selected");
        }
    }

    void Shoot()
    {
        Vector3 shootDirection = playerCamera.transform.forward;

        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;
        }

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Init(shootDirection, projectileSpeed);
        }
        cardManager.UpdateHandUI();
    }

    IEnumerator BurstShot()
    {
        for (int i = 0; i < 3; i++)
        {
            Shoot();
            yield return new WaitForSeconds(0.1f);
        }
        cardManager.UpdateHandUI();
    }
    void SpinAttack(float radius, int bulletCount = 8)
    {
        Vector3 center = transform.position;
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep;
            float angleRad = angle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)) * radius;
            Vector3 spawnPos = center + offset;

            GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(offset));

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.linearVelocity = offset.normalized * projectileSpeed;
            }
        }

        cardManager.UpdateHandUI();
    }

    void useCard(Card card)
    {
        switch (card.cardType)
        {
            case CardType.Shoot:
                Shoot();
                break;

            case CardType.Burst:
                StartCoroutine(BurstShot());
                break;

            case CardType.DoubleJump:
                playerMovement.DoubleJump();
                break;

            case CardType.Spin:
                SpinAttack(1.5f);
                break;

        }
    }

    IEnumerator LockInputBriefly()
    {
        inputLocked = true;
        yield return new WaitForSeconds(0.1f);
        inputLocked = false;
    }
}
