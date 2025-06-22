using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Manages enemy health, damage, death, and respawn logic.
/// Can be used for both DummyEnemy and EnemyAI.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 50;
    [SerializeField] private int currentHealth;

    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 10f;
    [SerializeField] private Transform respawnPoint; // Optional: set in Inspector for custom respawn location

    [Header("Visual Feedback")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private float damageFlashDuration = 0.15f;

    [Header("Events")]
    [SerializeField] private UnityEvent<int> onHealthChanged;
    [SerializeField] private UnityEvent onEnemyDied;
    [SerializeField] private UnityEvent onEnemyRespawned;
    
    [Header("UI Reference")]
    [SerializeField] public GameUI gameUI; // Reference to UI for health updates

    // Private variables
    private Renderer enemyRenderer;
    private bool isDead = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // Properties
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    
    // Public access to events
    public UnityEvent<int> OnHealthChanged => onHealthChanged;
    public UnityEvent OnEnemyDied => onEnemyDied;
    public UnityEvent OnEnemyRespawned => onEnemyRespawned;

    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        isDead = false;

        // Store initial position for respawn
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        // Get renderer for damage feedback
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null && normalMaterial != null)
        {
            enemyRenderer.material = normalMaterial;
        }

        // Trigger initial health event
        onHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Call this to deal damage to the enemy.
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        onHealthChanged?.Invoke(currentHealth);
        
        // Notify UI if available
        if (gameUI != null)
        {
            gameUI.OnEnemyHealthChanged(this);
        }

        // Visual feedback
        StartCoroutine(DamageFlash());
        
        // Trigger damage feedback on specific enemy types
        var dummyEnemy = GetComponent<DummyEnemy>();
        if (dummyEnemy != null)
        {
            dummyEnemy.OnTakeDamage();
        }
        
        var enemyAI = GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.OnTakeDamage();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        Debug.Log($"Enemy took {damage} damage! Health: {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// Heals the enemy by the specified amount.
    /// </summary>
    public void Heal(int healAmount)
    {
        if (isDead) return;
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        onHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// Handles enemy death and starts respawn coroutine.
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        isDead = true;
        onEnemyDied?.Invoke();

        // Disable enemy logic (AI, collider, renderer, etc.)
        SetActiveState(false);

        // Start respawn coroutine
        StartCoroutine(RespawnAfterDelay());
        Debug.Log("Enemy has died!");
    }

    /// <summary>
    /// Respawns the enemy after a delay.
    /// </summary>
    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        Respawn();
    }

    /// <summary>
    /// Respawns the enemy at the initial or specified respawn point.
    /// </summary>
    public void Respawn()
    {
        // Reset health and state
        currentHealth = maxHealth;
        isDead = false;
        onHealthChanged?.Invoke(currentHealth);

        // Move to respawn point or initial position
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }
        else
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        // Reactivate enemy logic
        SetActiveState(true);
        onEnemyRespawned?.Invoke();
        Debug.Log("Enemy has respawned!");
    }

    /// <summary>
    /// Enables or disables enemy logic and visuals.
    /// </summary>
    private void SetActiveState(bool isActive)
    {
        // Enable/disable renderer
        if (enemyRenderer != null)
            enemyRenderer.enabled = isActive;

        // Enable/disable collider
        var collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = isActive;

        // Enable/disable AI or other scripts (optional, expand as needed)
        var ai = GetComponent<EnemyAI>();
        if (ai != null)
            ai.enabled = isActive;
    }

    /// <summary>
    /// Coroutine for damage flash effect.
    /// </summary>
    private IEnumerator DamageFlash()
    {
        if (enemyRenderer == null || damageMaterial == null) yield break;
        enemyRenderer.material = damageMaterial;
        yield return new WaitForSeconds(damageFlashDuration);
        if (normalMaterial != null)
            enemyRenderer.material = normalMaterial;
    }

    /// <summary>
    /// Gets the health percentage (0-1).
    /// </summary>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    /// <summary>
    /// Sets the maximum health and adjusts current health accordingly.
    /// </summary>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth);
    }
} 