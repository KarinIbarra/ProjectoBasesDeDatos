using UnityEngine;

public class BlackoutEvent : MonoBehaviour
{
    [Header("Probabilidad")]
    [Range(0f, 1f)]
    public float blackoutChance = 0.3f;

    [Header("Oscuridad")]
    [Range(0f, 1f)]
    public float ambientIntensity = 0f;

    public Material blackoutSkybox;

    [Header("Jugador")]
    public Light playerLight;

    [Range(0f, 10f)]
    public float playerLightIntensity = 2f;

    [Range(1f, 10f)]
    public float playerLightRange = 3f;

    private Material originalSkybox;
    private float originalAmbientIntensity;

    void Start()
    {
        originalSkybox = RenderSettings.skybox;
        originalAmbientIntensity = RenderSettings.ambientIntensity;

        if (Random.value <= blackoutChance)
        {
            ActivateBlackout();
        }
    }

    void ActivateBlackout()
    {
        Debug.Log("APAGÓN");

        // Quitar iluminación ambiental
        RenderSettings.ambientIntensity = ambientIntensity;

        // Cambiar skybox
        if (blackoutSkybox != null)
        {
            RenderSettings.skybox = blackoutSkybox;
        }

        // Luz del jugador
        if (playerLight != null)
        {
            playerLight.enabled = true;
            playerLight.intensity = playerLightIntensity;
            playerLight.range = playerLightRange;
        }

        DynamicGI.UpdateEnvironment();
    }
}