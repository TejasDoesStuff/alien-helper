using System.Collections;
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
        if (selectedCard == null)
        {
            Debug.Log("No valid card selected");
            return;
        }

        projectileSpeed = selectedCard.bulletSpeed;

        if (selectedCard.IsOnCooldown()) return;

        if (selectedCard.use())
        {
            useCard(selectedCard);
            cardManager.UpdateHandUI();
        }
    }

    void SpawnProjectile(Vector3 dir, Vector3 spawnPos)
    {
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(dir));
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.linearVelocity = dir.normalized * projectileSpeed;
        }

        Projectile proj = projectile.GetComponent<Projectile>();
        if (proj != null)
            proj.Init(dir, projectileSpeed);
    }

    void Shoot()
    {
        Vector3 dir = playerCamera.transform.forward;
        SpawnProjectile(dir, shootPoint.position);
    }

    IEnumerator BurstShot(int pelletCount = 5, float spread = 0.05f)
    {
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 dir = playerCamera.transform.forward;
            dir += playerCamera.transform.right * Random.Range(-spread, spread);
            dir += playerCamera.transform.up * Random.Range(-spread, spread);
            dir.Normalize();
            SpawnProjectile(dir, shootPoint.position);
            yield return new WaitForSeconds(0.07f);
        }
    }

    void SpinAttack(float radius, int bulletCount = 12)
    {
        float angleStep = 360f / bulletCount;

        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            Vector3 spawnPos = transform.position + dir * 0.5f;
            SpawnProjectile(dir, spawnPos);
        }
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

