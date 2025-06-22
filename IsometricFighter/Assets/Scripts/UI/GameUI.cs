using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Main UI controller for the fighting game.
/// Manages health bars, punch feedback, death/respawn notifications, and other UI elements.
/// </summary>
public class GameUI : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] public Slider playerHealthBar;
    [SerializeField] public TextMeshProUGUI playerHealthText;
    [SerializeField] public Slider enemyHealthBar;
    [SerializeField] public TextMeshProUGUI enemyHealthText;
    
    [Header("Punch Feedback")]
    [SerializeField] public GameObject punchHitEffect;
    [SerializeField] public TextMeshProUGUI punchDamageText;
    [SerializeField] private float punchEffectDuration = 0.5f;
    
    [Header("Death/Respawn UI")]
    [SerializeField] public GameObject deathPanel;
    [SerializeField] public TextMeshProUGUI deathText;
    [SerializeField] public GameObject respawnPanel;
    [SerializeField] public TextMeshProUGUI respawnText;
    [SerializeField] private float respawnCountdownDuration = 10f;
    
    [Header("Game Status")]
    [SerializeField] public TextMeshProUGUI gameStatusText;
    [SerializeField] public GameObject victoryPanel;
    [SerializeField] public GameObject defeatPanel;
    
    [Header("Controls Display")]
    [SerializeField] public GameObject controlsPanel;
    [SerializeField] public TextMeshProUGUI controlsText;
    
    // Private variables
    private PlayerHealth playerHealth;
    private EnemyHealth currentTargetEnemy;
    private Coroutine respawnCoroutine;
    private bool isGamePaused = false;
    
    void Start()
    {
        // Auto-find UI elements if not assigned
        AutoFindUIElements();
        
        // Find player health
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                // Subscribe to player health events
                playerHealth.OnHealthChanged.AddListener(UpdatePlayerHealthUI);
                playerHealth.OnPlayerDied.AddListener(OnPlayerDied);
                
                // Initialize player health UI
                UpdatePlayerHealthUI(playerHealth.CurrentHealth);
            }
        }
        
        // Initialize UI
        InitializeUI();
        
        // Show controls briefly
        StartCoroutine(ShowControlsBriefly());
    }
    
    /// <summary>
    /// Automatically finds UI elements if they're not assigned
    /// </summary>
    private void AutoFindUIElements()
    {
        // Find UI elements by name
        if (playerHealthBar == null)
            playerHealthBar = GameObject.Find("PlayerHealthBar")?.GetComponent<Slider>();
        
        if (playerHealthText == null)
            playerHealthText = GameObject.Find("PlayerHealthText")?.GetComponent<TextMeshProUGUI>();
        
        if (enemyHealthBar == null)
            enemyHealthBar = GameObject.Find("EnemyHealthBar")?.GetComponent<Slider>();
        
        if (enemyHealthText == null)
            enemyHealthText = GameObject.Find("EnemyHealthText")?.GetComponent<TextMeshProUGUI>();
        
        if (gameStatusText == null)
            gameStatusText = GameObject.Find("GameStatusText")?.GetComponent<TextMeshProUGUI>();
        
        if (controlsText == null)
            controlsText = GameObject.Find("ControlsText")?.GetComponent<TextMeshProUGUI>();
        
        Debug.Log("Auto-find UI elements completed!");
    }
    
    /// <summary>
    /// Initializes all UI elements to their default state
    /// </summary>
    private void InitializeUI()
    {
        // Hide all panels initially
        if (deathPanel != null) deathPanel.SetActive(false);
        if (respawnPanel != null) respawnPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (defeatPanel != null) defeatPanel.SetActive(false);
        if (punchHitEffect != null) punchHitEffect.SetActive(false);
        
        // Set up controls text
        if (controlsText != null)
        {
            controlsText.text = "WASD: Move\nSPACE: Jump\nV: Punch";
        }
        
        // Set up game status
        if (gameStatusText != null)
        {
            gameStatusText.text = "Fight!";
        }
    }
    
    /// <summary>
    /// Updates the player health bar and text
    /// </summary>
    private void UpdatePlayerHealthUI(int currentHealth)
    {
        if (playerHealth == null) return;
        
        float healthPercentage = (float)currentHealth / playerHealth.MaxHealth;
        
        // Update health bar
        if (playerHealthBar != null)
        {
            playerHealthBar.value = healthPercentage;
            
            // Change color based on health
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage);
            playerHealthBar.fillRect.GetComponent<Image>().color = healthColor;
        }
        
        // Update health text
        if (playerHealthText != null)
        {
            playerHealthText.text = $"Player: {currentHealth}/{playerHealth.MaxHealth}";
        }
    }
    
    /// <summary>
    /// Updates the enemy health bar and text
    /// </summary>
    public void UpdateEnemyHealthUI(EnemyHealth enemyHealth)
    {
        if (enemyHealth == null) return;
        
        currentTargetEnemy = enemyHealth;
        float healthPercentage = (float)enemyHealth.CurrentHealth / enemyHealth.MaxHealth;
        
        // Update health bar
        if (enemyHealthBar != null)
        {
            enemyHealthBar.value = healthPercentage;
            
            // Change color based on health
            Color healthColor = Color.Lerp(Color.red, Color.green, healthPercentage);
            enemyHealthBar.fillRect.GetComponent<Image>().color = healthColor;
        }
        
        // Update health text
        if (enemyHealthText != null)
        {
            enemyHealthText.text = $"Enemy: {enemyHealth.CurrentHealth}/{enemyHealth.MaxHealth}";
        }
    }
    
    /// <summary>
    /// Updates enemy health UI when enemy takes damage (called from EnemyHealth)
    /// </summary>
    public void OnEnemyHealthChanged(EnemyHealth enemyHealth)
    {
        // Only update if this is the current target enemy
        if (enemyHealth == currentTargetEnemy)
        {
            UpdateEnemyHealthUI(enemyHealth);
        }
    }
    
    /// <summary>
    /// Shows punch hit effect with damage text
    /// </summary>
    public void ShowPunchHitEffect(int damage, Vector3 worldPosition)
    {
        if (punchHitEffect == null) return;
        
        StartCoroutine(PunchHitEffectCoroutine(damage, worldPosition));
    }
    
    /// <summary>
    /// Coroutine for punch hit effect
    /// </summary>
    private IEnumerator PunchHitEffectCoroutine(int damage, Vector3 worldPosition)
    {
        // Convert world position to screen position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        
        // Position the effect
        punchHitEffect.transform.position = screenPosition;
        
        // Set damage text
        if (punchDamageText != null)
        {
            punchDamageText.text = $"-{damage}";
        }
        
        // Show effect
        punchHitEffect.SetActive(true);
        
        // Wait for duration
        yield return new WaitForSeconds(punchEffectDuration);
        
        // Hide effect
        punchHitEffect.SetActive(false);
    }
    
    /// <summary>
    /// Called when player dies
    /// </summary>
    private void OnPlayerDied()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }
        
        if (deathText != null)
        {
            deathText.text = "YOU DIED!";
        }
        
        // Start respawn countdown
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
        }
        respawnCoroutine = StartCoroutine(RespawnCountdown());
    }
    
    /// <summary>
    /// Coroutine for respawn countdown
    /// </summary>
    private IEnumerator RespawnCountdown()
    {
        float timeRemaining = respawnCountdownDuration;
        
        while (timeRemaining > 0)
        {
            if (respawnPanel != null)
            {
                respawnPanel.SetActive(true);
            }
            
            if (respawnText != null)
            {
                respawnText.text = $"Respawning in {Mathf.CeilToInt(timeRemaining)}...";
            }
            
            timeRemaining -= Time.deltaTime;
            yield return null;
        }
        
        // Hide respawn panel
        if (respawnPanel != null)
        {
            respawnPanel.SetActive(false);
        }
        
        // Hide death panel
        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Shows controls briefly at game start
    /// </summary>
    private IEnumerator ShowControlsBriefly()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
            yield return new WaitForSeconds(3f);
            controlsPanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// Toggles pause menu
    /// </summary>
    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;
        
        // You can add pause panel logic here
    }
    
    /// <summary>
    /// Updates game status text
    /// </summary>
    public void UpdateGameStatus(string status)
    {
        if (gameStatusText != null)
        {
            gameStatusText.text = status;
        }
    }
    
    /// <summary>
    /// Shows victory screen
    /// </summary>
    public void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }
    }
    
    /// <summary>
    /// Shows defeat screen
    /// </summary>
    public void ShowDefeat()
    {
        if (defeatPanel != null)
        {
            defeatPanel.SetActive(true);
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdatePlayerHealthUI);
            playerHealth.OnPlayerDied.RemoveListener(OnPlayerDied);
        }
    }
} 