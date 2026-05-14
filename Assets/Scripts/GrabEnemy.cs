using UnityEngine;

public class GrabEnemy : Enemy
{
    [Header("Habilidad de agarre")]
    public float grabCooldown = 6f;
    public float grabWindup = 1.5f;
    public float grabRange = 10f;
    public float grabForce = 20f;
    public float safeDistance = 12f;

    [HideInInspector] public float grabCooldownTimer;

    protected override void Start()
    {
        base.Start();
        grabCooldownTimer = grabCooldown; 
    }

    protected override void Update()
    {
        if (grabCooldownTimer > 0f)
            grabCooldownTimer -= Time.deltaTime;

        base.Update();
    }

    public bool IsReadyToGrab()
    {
        return grabCooldownTimer <= 0f;
    }

    public void ResetGrabCooldown()
    {
        grabCooldownTimer = grabCooldown;
    }
}