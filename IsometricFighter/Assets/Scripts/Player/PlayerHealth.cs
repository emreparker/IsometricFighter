using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Manages player health, damage, and death events.
/// Handles taking damage from enemies and fires events when health changes or player dies.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    
    [Header("Visual Feedback")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material damageMaterial;
    [SerializeField] private float damageFlashDuration = 0.2f;
    
    [Header("Recoil Settings")]
    [SerializeField] private float playerRecoilForce = 60f; // Force applied to player when hit (dramatically increased)
    [SerializeField] private float playerRecoilUpwardForce = 12f; // Upward force when hit (dramatically increased)
    [SerializeField] private float recoilStunDuration = 0.2f; // How long player is stunned during recoil (reduced)
    
    [Header("Events")]
    [SerializeField] private UnityEvent<int> onHealthChanged;
    [SerializeField] private UnityEvent onPlayerDied;
    
    // Private variables
    private Renderer playerRenderer;
    private bool isDead = false;
    private bool isInRecoil = false;
    private float recoilEndTime = 0f;
    
    // Properties
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => isDead;
    public bool IsInRecoil => isInRecoil;
    
    // Public access to events
    public UnityEvent<int> OnHealthChanged => onHealthChanged;
    public UnityEvent OnPlayerDied => onPlayerDied;
    
    void Start()
    {
        // Initialize health
        currentHealth = maxHealth;
        
        // Get renderer for damage feedback
        playerRenderer = GetComponent<Renderer>();
        
        // Set initial material
        if (playerRenderer != null && normalMaterial != null)
        {
            playerRenderer.material = normalMaterial;
        }
        
        // Trigger initial health event
        onHealthChanged?.Invoke(currentHealth);
    }
    
    /// <summary>
    /// Takes damage from an enemy attack
    /// </summary>
    /// <param name="damage">Amount of damage to take</param>
    /// <param name="attacker">Optional attacker transform for recoil direction</param>
    public void TakeDamage(int damage, Transform attacker = null)
    {
        // Don't take damage if already dead
        if (isDead) return;
        
        // Apply damage
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth); // Ensure health doesn't go below 0
        
        // Trigger health changed event
        onHealthChanged?.Invoke(currentHealth);
        
        // Visual feedback
        StartCoroutine(DamageFlash());
        
        // Apply recoil if attacker is provided
        if (attacker != null)
        {
            ApplyPlayerRecoil(attacker);
        }
        
        // Check if player died
        if (currentHealth <= 0)
        {
            Die();
        }
        
        Debug.Log($"Player took {damage} damage! Health: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// Applies recoil force to the player when hit by enemies
    /// </summary>
    private void ApplyPlayerRecoil(Transform attacker)
    {
        // Get player rigidbody
        Rigidbody playerRb = GetComponent<Rigidbody>();
        if (playerRb == null) return;
        
        // Calculate recoil direction (away from attacker)
        Vector3 recoilDirection = (transform.position - attacker.position).normalized;
        
        // Check attacker type for different recoil behavior
        var dummyEnemy = attacker.GetComponent<DummyEnemy>();
        var enemyAI = attacker.GetComponent<EnemyAI>();
        
        if (dummyEnemy != null)
        {
            // Dummy enemy: no recoil (dummy doesn't attack)
            return;
        }
        else if (enemyAI != null)
        {
            // AI enemy: strong recoil with stun
            playerRb.AddForce(recoilDirection * playerRecoilForce, ForceMode.Impulse);
            playerRb.AddForce(Vector3.up * playerRecoilUpwardForce, ForceMode.Impulse);
            
            // Set recoil state
            isInRecoil = true;
            recoilEndTime = Time.time + recoilStunDuration;
            
            // Start coroutine to end recoil
            StartCoroutine(EndRecoilAfterDuration());
        }
        else
        {
            // Generic enemy: default recoil
            playerRb.AddForce(recoilDirection * (playerRecoilForce * 0.7f), ForceMode.Impulse);
            playerRb.AddForce(Vector3.up * (playerRecoilUpwardForce * 0.7f), ForceMode.Impulse);
            
            // Set recoil state
            isInRecoil = true;
            recoilEndTime = Time.time + (recoilStunDuration * 0.7f);
            
            // Start coroutine to end recoil
            StartCoroutine(EndRecoilAfterDuration());
        }
    }
    
    /// <summary>
    /// Coroutine to end recoil state after duration
    /// </summary>
    private IEnumerator EndRecoilAfterDuration()
    {
        yield return new WaitForSeconds(recoilStunDuration);
        isInRecoil = false;
    }
    
    /// <summary>
    /// Heals the player by the specified amount
    /// </summary>
    /// <param name="healAmount">Amount of health to restore</param>
    public void Heal(int healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth); // Ensure health doesn't exceed max
        
        // Trigger health changed event
        onHealthChanged?.Invoke(currentHealth);
        
        Debug.Log($"Player healed for {healAmount}! Health: {currentHealth}/{maxHealth}");
    }
    
    /// <summary>
    /// Handles player death
    /// </summary>
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Trigger death event
        onPlayerDied?.Invoke();
        
        // Disable player controller
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        
        // Disable rigidbody
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
        
        Debug.Log("Player has died!");
    }
    
    /// <summary>
    /// Respawns the player with full health
    /// </summary>
    public void Respawn()
    {
        // Reset health
        currentHealth = maxHealth;
        isDead = false;
        
        // Re-enable player controller
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        // Re-enable rigidbody
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
        
        // Trigger health changed event
        onHealthChanged?.Invoke(currentHealth);
        
        Debug.Log("Player has respawned!");
    }
    
    /// <summary>
    /// Coroutine for damage flash effect
    /// </summary>
    private System.Collections.IEnumerator DamageFlash()
    {
        if (playerRenderer == null || damageMaterial == null) yield break;
        
        // Flash red material
        playerRenderer.material = damageMaterial;
        
        // Wait for flash duration
        yield return new WaitForSeconds(damageFlashDuration);
        
        // Return to normal material
        if (normalMaterial != null)
        {
            playerRenderer.material = normalMaterial;
        }
    }
    
    /// <summary>
    /// Gets the health percentage (0-1)
    /// </summary>
    /// <returns>Health percentage as a float between 0 and 1</returns>
    public float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }
    
    /// <summary>
    /// Sets the maximum health and adjusts current health accordingly
    /// </summary>
    /// <param name="newMaxHealth">New maximum health value</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        
        // Adjust current health if it exceeds new max
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        // Trigger health changed event
        onHealthChanged?.Invoke(currentHealth);
    }
} 