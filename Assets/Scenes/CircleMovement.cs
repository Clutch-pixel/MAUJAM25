using UnityEngine;
using UnityEngine.UI; // UI için gerekli

public class CircleMovement : MonoBehaviour
{
    public GameObject player;
    private Vector2 startPosition;
    private Color startColor;
    private float moveTime = 6f;
    private float moveDistance = 140f;
    private float barPadding = 1f; // Bar kenarlarından içeri padding
    private float speed;
    private GameObject lightObject;
    private RectTransform rectTransform;
    private Image circleImage;
    public GameObject enemyPrefab; // Unity Inspector'da atanacak düşman prefabı
    private float stayTimer = 0f; // Durma süresini takip etmek için
    private const float SPAWN_DELAY = 1f; // Spawnlama için bekleme süresi
    private const float SPAWN_COOLDOWN = 5f; // Yeni spawnlama için bekleme süresi
    private const float ENEMY_SPAWN_DISTANCE = 6f; // Player'dan uzaklık arttırıldı
    private const float ENEMY_MOVE_SPEED = 5f; // Düşman hareket hızı azaltıldı
    private float spawnCooldownTimer = 0f; // Spawn cooldown için timer
    private bool canSpawn = true; // Spawn yapılabilir mi kontrolü
    private const float MIN_SPAWN_DISTANCE = 8f; // Minimum spawn mesafesi
    private const float MAX_SPAWN_DISTANCE = 13f; // Maximum spawn mesafesi

    private void Start()
    {
        // Canvas ayarlarını yap
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 1;
            }
        }

        // Komponentleri al
        rectTransform = GetComponent<RectTransform>();
        circleImage = GetComponent<Image>();
        
        // Bar'ı ayarla
        GameObject barObject = GameObject.Find("Bar");
        if (barObject != null)
        {
            RectTransform barRect = barObject.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0, 1);
            barRect.anchorMax = new Vector2(0, 1);
            barRect.pivot = new Vector2(0.5f, 0.5f);
            barRect.anchoredPosition = new Vector2(120, -30);
            barRect.sizeDelta = new Vector2(moveDistance + 40, 10);

            // Circle'ı ayarla
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(30, 30);
            
            // Circle'ı bar'ın ortasına konumlandır
            rectTransform.anchoredPosition = new Vector2(barRect.anchoredPosition.x, barRect.anchoredPosition.y);
        }

        // Başlangıç değerlerini ayarla
        startPosition = rectTransform.anchoredPosition;
        startColor = Color.white;
        circleImage.color = startColor;
        lightObject = GameObject.Find("Light");
        speed = moveDistance / moveTime;
    }

    private void Update()
    {
        // Spawn cooldown kontrolü
        if (!canSpawn)
        {
            spawnCooldownTimer += Time.deltaTime;
            if (spawnCooldownTimer >= SPAWN_COOLDOWN)
            {
                canSpawn = true;
                spawnCooldownTimer = 0f;
            }
        }

        if (player != null && lightObject != null)
        {
            bool isPlayerTouchingLight = player.GetComponent<Collider2D>().IsTouching(lightObject.GetComponent<Collider2D>());
            
            if (isPlayerTouchingLight)
            {
                MoveRight();
            }
            else
            {
                MoveLeft();
            }

            // Bar'ın en solunda veya en sağında durma kontrolü
            CheckEdgeStay();
        }
        else
        {
            Debug.LogWarning("Player veya Light objesi bulunamadı!");
        }
    }

    private void MoveRight()
    {
        Vector2 targetPosition = startPosition + Vector2.right * (moveDistance/2 - barPadding);
        rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, speed * Time.deltaTime);
        ChangeColor(true);
    }

    private void MoveLeft()
    {
        Vector2 targetPosition = startPosition - Vector2.right * (moveDistance/2 - barPadding);
        rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, speed * Time.deltaTime);
        ChangeColor(false);
    }

    private void ChangeColor(bool isRight)
    {
        if (rectTransform.anchoredPosition.x > startPosition.x)
        {
            circleImage.color = Color.yellow;
        }
        else if (rectTransform.anchoredPosition.x < startPosition.x)
        {
            circleImage.color = new Color(0, 139f / 255f, 139f / 255f);
        }
        else
        {
            circleImage.color = startColor;
        }
    }

    private void CheckEdgeStay()
    {
        if (!canSpawn) return; // Eğer spawn yapılamıyorsa hiç kontrol etme

        Vector2 currentPos = rectTransform.anchoredPosition;
        Vector2 leftEdge = startPosition - Vector2.right * (moveDistance/2 - barPadding);
        Vector2 rightEdge = startPosition + Vector2.right * (moveDistance/2 - barPadding);

        if (Vector2.Distance(currentPos, leftEdge) < 0.1f || Vector2.Distance(currentPos, rightEdge) < 0.1f)
        {
            stayTimer += Time.deltaTime;
            if (stayTimer >= SPAWN_DELAY)
            {
                SpawnEnemies();
                stayTimer = 0f;
                canSpawn = false; // Spawn sonrası cooldown başlat
                spawnCooldownTimer = 0f;
            }
        }
        else
        {
            stayTimer = 0f;
        }
    }

    private void SpawnEnemies()
    {
        // Tek bir düşman için rastgele pozisyon oluştur
        Vector3 randomSpawnPos = GetRandomSpawnPosition();
        SpawnEnemy(randomSpawnPos);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Rastgele bir açı seç (0-360 derece)
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        // Rastgele bir mesafe seç (MIN_SPAWN_DISTANCE ile MAX_SPAWN_DISTANCE arasında)
        float randomDistance = Random.Range(MIN_SPAWN_DISTANCE, MAX_SPAWN_DISTANCE);
        
        // Polar koordinatları kartezyen koordinatlara çevir
        float x = Mathf.Cos(randomAngle) * randomDistance;
        float y = Mathf.Sin(randomAngle) * randomDistance;
        
        // Player'ın pozisyonuna göre offset ekle
        Vector3 spawnPosition = player.transform.position + new Vector3(x, y, 0);
        
        return spawnPosition;
    }

    private void SpawnEnemy(Vector3 spawnPosition)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        EnemyBehavior enemyBehavior = enemy.AddComponent<EnemyBehavior>();
        enemyBehavior.Initialize(player, ENEMY_MOVE_SPEED);
    }
}

// Yeni script oluşturun: EnemyBehavior.cs
public class EnemyBehavior : MonoBehaviour
{
    private GameObject player;
    private float moveSpeed;
    private Rigidbody2D rb;

    public void Initialize(GameObject target, float speed)
    {
        player = target;
        moveSpeed = speed;
        rb = GetComponent<Rigidbody2D>();
        
        // Yerçekimini kaldır
        rb.gravityScale = 0f;
    }

    private void Update()
    {
        if (player == null) return;

        // Player'a doğru direkt hareket et
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
    }
}