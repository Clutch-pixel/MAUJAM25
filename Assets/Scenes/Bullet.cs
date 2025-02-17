using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log($"Bullet {gameObject.name} created with components:");
        Debug.Log($"Has Collider2D: {GetComponent<Collider2D>() != null}");
        Debug.Log($"Has Rigidbody2D: {GetComponent<Rigidbody2D>() != null}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Bullet triggered with {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
        HandleCollision(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Bullet collided with {collision.gameObject.name} (Tag: {collision.gameObject.tag})");
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Bullet hit player, ignoring collision.");
            return; // Player'a çarptığında hiçbir şey yapma
        }
        if (other.CompareTag("Light"))
        {
            Debug.Log("Bullet hit player, ignoring collision.");
            return; // Player'a çarptığında hiçbir şey yapma
        }
        if (other.CompareTag("Enemy"))
        {
            Debug.Log($"Destroying enemy: {other.name}");
            Destroy(other);
        }
        
        Debug.Log($"Destroying bullet: {gameObject.name}");
        Destroy(gameObject);
    }
}