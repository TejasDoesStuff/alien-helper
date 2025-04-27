using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float projectileSpeed = 50f;
    public float shootCooldown = 0.2f;
    private bool canShoot = true;
    public Camera playerCamera;

    void Update()
    {
        if (Input.GetButton("Fire1") && canShoot)
        {
            Shoot();
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
            rb.isKinematic = true;
        }

        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Init(shootDirection, projectileSpeed);
        }

        canShoot = false;
        Invoke(nameof(ResetShot), shootCooldown);
    }

    void ResetShot()
    {
        canShoot = true;
    }
}
