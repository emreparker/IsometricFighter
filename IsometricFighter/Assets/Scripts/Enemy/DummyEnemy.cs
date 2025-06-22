using UnityEngine;

/// <summary>
/// Dummy enemy that stands still, takes damage, dies, and respawns after a delay.
/// Uses EnemyHealth for health and respawn logic.
/// </summary>
[RequireComponent(typeof(EnemyHealth))]
public class DummyEnemy : MonoBehaviour
{
    [Header("Visuals & Animation")]
    [SerializeField] private Animator animator;

    // Private variables
    private EnemyHealth enemyHealth;

    // Animation parameter names (customize as needed)
    private const string ANIM_IS_DEAD = "IsDead";
    private const string ANIM_HIT = "Hit";

    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        // Subscribe to health events
        enemyHealth.OnEnemyDied.AddListener(OnDied);
        enemyHealth.OnEnemyRespawned.AddListener(OnRespawned);
    }

    /// <summary>
    /// Called when the dummy takes damage (optional: trigger hit animation).
    /// </summary>
    public void OnHit()
    {
        if (animator != null)
        {
            animator.SetTrigger(ANIM_HIT);
        }
    }

    /// <summary>
    /// Called when the dummy takes damage from EnemyHealth
    /// </summary>
    public void OnTakeDamage()
    {
        OnHit(); // Trigger hit animation/feedback
    }

    /// <summary>
    /// Called when the dummy dies.
    /// </summary>
    private void OnDied()
    {
        if (animator != null)
        {
            animator.SetBool(ANIM_IS_DEAD, true);
        }
    }

    /// <summary>
    /// Called when the dummy respawns.
    /// </summary>
    private void OnRespawned()
    {
        if (animator != null)
        {
            animator.SetBool(ANIM_IS_DEAD, false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events to avoid memory leaks
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDied.RemoveListener(OnDied);
            enemyHealth.OnEnemyRespawned.RemoveListener(OnRespawned);
        }
    }
} 