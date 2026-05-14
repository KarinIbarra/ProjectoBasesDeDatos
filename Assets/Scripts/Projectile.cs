using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 4f;
    public int damage = 1;

    private Vector3 direction;
    private GameObject owner; 

    public void Init(Vector3 dir, GameObject shooter)
    {
        direction = dir.normalized;
        owner = shooter;

        Collider myCollider = GetComponent<Collider>();
        Collider shooterCollider = shooter.GetComponent<Collider>();

        if (myCollider != null && shooterCollider != null)
        {
            Physics.IgnoreCollision(myCollider, shooterCollider);
        }

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile hit: " + other.name);

        if (other.gameObject == owner) return;

        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}