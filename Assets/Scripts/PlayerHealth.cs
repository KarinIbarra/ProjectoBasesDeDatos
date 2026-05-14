using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public int hearts = 3;
    public TextMeshProUGUI heartsText;

    void Start()
    {
        UpdateHeartsUI();
    }

    public void TakeDamage(int amount)
    {
        hearts -= amount;
        hearts = Mathf.Clamp(hearts, 0, 3);

        Debug.Log("Jugador golpeado. Corazones restantes: " + hearts);

        UpdateHeartsUI();

        if (hearts <= 0)
        {
            Die();
        }
    }

    void UpdateHeartsUI()
    {
        if (heartsText != null)
        {
            heartsText.text = " " + hearts;
        }
    }

    void Die()
    {
        Debug.Log("Jugador muerto");

   
        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
            pc.canMove = false;

  
        gameObject.SetActive(false);

        
        Invoke(nameof(RestartScene), 1.5f);
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

