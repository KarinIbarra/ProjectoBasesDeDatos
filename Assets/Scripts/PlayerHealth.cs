using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int hearts = 3;
    public TextMeshProUGUI heartsText;

    // Tiempo de invulnerabilidad
    public float invulnerabilityTime = 5f;

    // Controla si el jugador puede recibir daño
    private bool isInvulnerable = false;

    void Start()
    {
        UpdateHeartsUI();
    }

    public void TakeDamage(int amount)
    {
        // Si es invulnerable, ignorar daño
        if (isInvulnerable)
            return;

        hearts -= amount;
        hearts = Mathf.Clamp(hearts, 0, 3);

        Debug.Log("Jugador golpeado. Corazones restantes: " + hearts);

        UpdateHeartsUI();

        if (hearts <= 0)
        {
            Die();
        }
        else
        {
            // Activar invulnerabilidad
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        Debug.Log("Jugador invulnerable");

        yield return new WaitForSeconds(invulnerabilityTime);

        isInvulnerable = false;

        Debug.Log("Jugador vulnerable otra vez");
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

