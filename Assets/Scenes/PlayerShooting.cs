using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Unity'de atanacak mermi prefabı
    public float bulletSpeed = 8f; // Mermi hızı
    public float singleShotCooldown = 2f; // Tek atış bekleme süresi
    public float multiShotCooldown = 7f; // Çoklu atış bekleme süresi

    private float singleShotTimer = 0f;
    private float multiShotTimer = 0f;
    private bool canSingleShoot = true;
    private bool canMultiShoot = true;

    private void Update()
    {
        // Cooldown timerları güncelle
        UpdateTimers();

        // Sol tık - Tek atış
        if (Input.GetMouseButtonDown(0) && canSingleShoot)
        {
            SingleShot();
        }

        // Sağ tık - Çoklu atış
        if (Input.GetMouseButtonDown(1) && canMultiShoot)
        {
            MultiShot();
        }
    }

    private void UpdateTimers()
    {
        if (!canSingleShoot)
        {
            singleShotTimer += Time.deltaTime;
            if (singleShotTimer >= singleShotCooldown)
            {
                canSingleShoot = true;
                singleShotTimer = 0f;
            }
        }

        if (!canMultiShoot)
        {
            multiShotTimer += Time.deltaTime;
            if (multiShotTimer >= multiShotCooldown)
            {
                canMultiShoot = true;
                multiShotTimer = 0f;
            }
        }
    }

    private void SingleShot()
    {
        Vector2 direction = GetMouseDirection();
        FireBullet(direction);
        canSingleShoot = false;
    }

    private void MultiShot()
    {
        Vector2 baseDirection = GetMouseDirection();
        
        // Ana yön
        FireBullet(baseDirection);
        
        // Yukarı açılar
        FireBullet(RotateVector(baseDirection, 10));
        FireBullet(RotateVector(baseDirection, 20));
        
        // Aşağı açılar
        FireBullet(RotateVector(baseDirection, -10));
        FireBullet(RotateVector(baseDirection, -20));

        canMultiShoot = false;
    }

    private Vector2 GetMouseDirection()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return ((Vector2)(mousePos - transform.position)).normalized;
    }

    private Vector2 RotateVector(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(radians);
        float sin = Mathf.Sin(radians);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }

    private void FireBullet(Vector2 direction)
    {
        // Mermiyi player'ın biraz önünde oluştur
        Vector3 spawnPosition = transform.position + (Vector3)direction * 0.5f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);
        
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
            Debug.Log("Bullet velocity set to: " + rb.linearVelocity);
        }
        
        // 3 saniye sonra mermiyi yok et
        Destroy(bullet, 3f);
    }
}