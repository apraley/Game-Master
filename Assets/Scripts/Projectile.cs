using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private LayerMask hitMask = ~0;

    private int damage;
    private float speed;
    private float splashRadius;
    private GameObject owner;

    public void Initialize(int baseDamage, float projectileSpeed, float splash, GameObject sourceOwner)
    {
        damage = baseDamage;
        speed = projectileSpeed;
        splashRadius = splash;
        owner = sourceOwner;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & hitMask) == 0)
        {
            return;
        }

        if (other.gameObject == owner)
        {
            return;
        }

        if (splashRadius > 0f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, splashRadius, hitMask);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == owner)
                {
                    continue;
                }

                IDamageable splashTarget = hit.GetComponentInParent<IDamageable>();
                splashTarget?.ApplyDamage(damage);
            }
        }
        else
        {
            IDamageable directTarget = other.GetComponentInParent<IDamageable>();
            directTarget?.ApplyDamage(damage);
        }

        Destroy(gameObject);
    }
}
