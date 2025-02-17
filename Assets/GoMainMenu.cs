using UnityEngine;
using UnityEngine.SceneManagement;

public class GoMainMenu : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Son")) 
        {
            SceneManager.LoadScene("Main_Menu");
        }
    }
    
}
