using UnityEngine;
using System.Collections;

/// <summary>
/// Basic AI for an enemy that chases and attacks the player.
/// Uses Rigidbody physics for movement and attacks when in range.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int attackDamage = 10;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("UI Feedback")]
    [SerializeField] public GameUI gameUI; // Reference to UI for punch effects

    // Private variables
    private Rigidbody rb;
    private EnemyHealth enemyHealth;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool isInRecoil = false;
    private float recoilEndTime = 0f;

    // Animation parameter names
    private const string ANIM_IS_MOVING = "IsMoving";
    private const string ANIM_ATTACK = "Attack";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        enemyHealth = GetComponent<EnemyHealth>();

        // Auto-find player if not assigned
        if (player == null && GameObject.FindWithTag("Player") != null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (enemyHealth.IsDead || player == null) return;
        
        // Check if in recoil - don't move during recoil
        if (isInRecoil)
        {
            if (Time.time >= recoilEndTime)
            {
                isInRecoil = false;
            }
            else
            {
                return; // Don't process AI during recoil
            }
        }
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && distanceToPlayer > attackRange)
        {
            ChasePlayer();
        }
        else if (distanceToPlayer <= attackRange)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                StartCoroutine(AttackPlayer());
            }
            else
            {
                StopMoving();
            }
        }
        else
        {
            StopMoving();
        }
        UpdateAnimations();
    }

    /// <summary>
    /// Moves the enemy toward the player using Rigidbody physics.
    /// </summary>
    private void ChasePlayer()
    {
        if (isAttacking) return;
        Vector3 direction = (player.position - transform.position).normalized;
        Vector3 targetVelocity = direction * moveSpeed;
        targetVelocity.y = rb.linearVelocity.y; // Preserve vertical velocity
        rb.linearVelocity = targetVelocity;
        // Face the player
        transform.rotation = Quaternion.LookRotation(direction);
    }

    /// <summary>
    /// Stops the enemy's movement.
    /// </summary>
    private void StopMoving()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    /// <summary>
    /// Coroutine to attack the player with cooldown.
    /// </summary>
    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        if (animator != null)
        {
            animator.SetTrigger(ANIM_ATTACK);
        }
        // Wait for animation windup (optional, adjust as needed)
        yield return new WaitForSeconds(0.3f);
        // Check if player is still in range
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange + 0.2f)
        {
            // Deal damage to player
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage, transform); // Pass attacker transform for recoil
                Debug.Log($"Enemy attacked player for {attackDamage} damage!");
                
                // Show UI feedback
                if (gameUI != null)
                {
                    gameUI.ShowPunchHitEffect(attackDamage, player.position);
                }
            }
        }
        // Wait for cooldown
        yield return new WaitForSeconds(attackCooldown - 0.3f);
        isAttacking = false;
    }

    /// <summary>
    /// Updates animation parameters based on movement and state.
    /// </summary>
    private void UpdateAnimations()
    {
        if (animator == null) return;
        bool isMoving = rb.linearVelocity.magnitude > 0.1f && !isAttacking;
        animator.SetBool(ANIM_IS_MOVING, isMoving);
    }

    /// <summary>
    /// Draws gizmos for chase and attack ranges in the editor.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    /// <summary>
    /// Called when enemy takes damage to set recoil state
    /// </summary>
    public void OnTakeDamage()
    {
        // Set recoil state to prevent AI interference
        isInRecoil = true;
        recoilEndTime = Time.time + 1.0f; // 1 second recoil duration
    }
} 