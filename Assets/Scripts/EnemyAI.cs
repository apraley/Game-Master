using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    public enum EnemyType
    {
        Orderly,
        Nurse,
        Roomba,
        Patient,
    }

    [Header("Identity")]
    [SerializeField] private EnemyType enemyType = EnemyType.Orderly;

    [Header("Stats")]
    [SerializeField] private int maxHealth = 40;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float detectRange = 16f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.2f;

    [Header("Patrol")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float patrolWaitTime = 1.5f;

    [Header("Ranged Attack (Nurse)")]
    [SerializeField] private GameObject syringeProjectilePrefab;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float projectileSpeed = 14f;

    [Header("Roomba")]
    [SerializeField] private float roombaExplodeRadius = 3f;

    [Header("Patient Voice")]
    [SerializeField] private AudioSource voiceSource;
    [SerializeField] private AudioClip[] patientVoiceLines;
    [SerializeField] private Vector2 patientVoiceInterval = new Vector2(3f, 7f);

    private NavMeshAgent agent;
    private Transform player;
    private PlayerHealth playerHealth;

    private int currentHealth;
    private int patrolIndex;
    private float lastAttackTime;
    private float waitTimer;
    private float patientVoiceTimer;
    private float nextPatientVoiceTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        RollPatientVoiceTime();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerHealth>();
        }

        ApplyEnemyTuning();
    }

    private void Update()
    {
        if (player == null || currentHealth <= 0)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectRange)
        {
            agent.SetDestination(player.position);

            if (enemyType == EnemyType.Patient)
            {
                HandlePatientVoice();
            }

            if (distance <= attackRange)
            {
                TryAttack();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[patrolIndex].position);
                waitTimer = 0f;
            }
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        switch (enemyType)
        {
            case EnemyType.Orderly:
            case EnemyType.Patient:
                playerHealth?.ApplyDamage(attackDamage);
                break;

            case EnemyType.Nurse:
                FireSyringe();
                break;

            case EnemyType.Roomba:
                Explode();
                break;
        }
    }

    private void HandlePatientVoice()
    {
        if (voiceSource == null || patientVoiceLines == null || patientVoiceLines.Length == 0)
        {
            return;
        }

        patientVoiceTimer += Time.deltaTime;
        if (patientVoiceTimer < nextPatientVoiceTime || voiceSource.isPlaying)
        {
            return;
        }

        patientVoiceTimer = 0f;
        voiceSource.PlayOneShot(patientVoiceLines[Random.Range(0, patientVoiceLines.Length)]);
        RollPatientVoiceTime();
    }

    private void RollPatientVoiceTime()
    {
        nextPatientVoiceTime = Random.Range(patientVoiceInterval.x, patientVoiceInterval.y);
    }

    private void FireSyringe()
    {
        if (syringeProjectilePrefab == null || attackPoint == null)
        {
            playerHealth?.ApplyDamage(attackDamage);
            return;
        }

        GameObject projectileObject = Instantiate(syringeProjectilePrefab, attackPoint.position, attackPoint.rotation);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(attackDamage, projectileSpeed, 0f, gameObject);
        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, roombaExplodeRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth hp = hit.GetComponent<PlayerHealth>();
                hp?.ApplyDamage(attackDamage);
            }
        }

        Die();
    }

    public void ApplyDamage(int amount)
    {
        if (currentHealth <= 0 || amount <= 0)
        {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void ApplyEnemyTuning()
    {
        switch (enemyType)
        {
            case EnemyType.Orderly:
                agent.speed = 2.6f;
                attackRange = 1.8f;
                break;

            case EnemyType.Nurse:
                agent.speed = 3.8f;
                attackRange = 8f;
                break;

            case EnemyType.Roomba:
                agent.speed = 5.6f;
                detectRange = 20f;
                attackRange = 1f;
                break;

            case EnemyType.Patient:
                agent.speed = 3.2f;
                attackRange = 1.6f;
                detectRange = 18f;
                break;
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }
}
