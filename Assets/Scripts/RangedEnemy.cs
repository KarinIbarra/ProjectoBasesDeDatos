using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float preferredDistance = 8f;
    public float stopThreshold = 1f;
    public float shootWindup = 1f;
    public float spreadAngle = 20f;
    public float shootCooldown = 3f;

    [HideInInspector] public float shootCooldownTimer = 0f;

    protected override void Start()
    {
        base.Start(); 
    }

    protected override void Update()
    {
        
        if (shootCooldownTimer > 0f)
            shootCooldownTimer -= Time.deltaTime;

        base.Update(); 
    }

    public void FireProjectiles()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogWarning("RangedEnemy: falta projectilePrefab o firePoint");
            return;
        }

        Vector3 origin = firePoint.position;
        Vector3 dirToPlayer = (player.position + Vector3.up - origin).normalized;

        float[] angles = { 0f, spreadAngle, -spreadAngle };

        foreach (float angle in angles)
        {
            Vector3 spreadDir = Quaternion.Euler(0, angle, 0) * dirToPlayer;
            GameObject proj = Instantiate(projectilePrefab, origin, Quaternion.identity);
            Projectile p = proj.GetComponent<Projectile>();
            if (p != null) p.Init(spreadDir, gameObject);
        }

        shootCooldownTimer = shootCooldown;
    }

    public bool IsReadyToShoot()
    {
        return shootCooldownTimer <= 0f;
    }
}