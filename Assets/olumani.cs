using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeath : MonoBehaviour
{
    public Animator animator;  // Karakterin animatörü
    private bool isDead = false;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("thorn") && !isDead) 
        {
            isDead = true;
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        GetComponent<Anasininami>().enabled = false; // Hareket scriptini devre dışı bırak
        rb.linearVelocity = Vector2.zero; // Hareketi durdur
        animator.SetTrigger("die"); // Ölüm animasyonunu tetikle
        yield return new WaitForSeconds(0.20f); // 1 saniye bekle
        GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(1f); // 1 saniye bekle

        SceneManager.LoadScene("Main_Menu"); // Main_Menu sahnesine geçiş yap
    }
}
