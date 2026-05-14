using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!LevelManager.Instance) return;

        if (other.CompareTag("Player"))
        {
            if (LevelManager.Instance.IsEscapeActive())
            {
                Debug.Log("¡VICTORIA!");

                // OPCIÓN 1: Reiniciar escena
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                // OPCIÓN 2 (mejor): cargar escena de victoria
                // SceneManager.LoadScene("VictoryScene");
            }
        }
    }
}
