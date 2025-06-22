using UnityEngine;

/// <summary>
/// Controls player movement, jumping, and punching in an isometric fighting game.
/// Handles WASD movement, Space for jumping, and V for punching with cooldown.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 8f; // Updated to match Inspector value
    [SerializeField] private LayerMask groundLayer = 1;
    [SerializeField] private float acceleration = 25f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float gravityMultiplier = 0.8f; // Updated to match Inspector value
    
    [Header("Combat Settings")]
    [SerializeField] private float punchCooldown = 0.5f;
    [SerializeField] private float punchRange = 2f;
    [SerializeField] private int punchDamage = 10;
    [SerializeField] private float punchRecoilForce = 80f; // Force applied to enemies when punched (dramatically increased)
    [SerializeField] private float punchRecoilUpwardForce = 15f; // Upward force for more dramatic effect (dramatically increased)
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer; // For sprite-based animations
    
    [Header("UI Feedback")]
    [SerializeField] public GameUI gameUI; // Reference to UI for punch effects
    
    // Private variables
    private Rigidbody rb;
    private bool isGrounded;
    private bool canPunch = true;
    private float lastPunchTime;
    private PlayerHealth playerHealth;
    
    // Animation parameter names
    private const string ANIM_IS_MOVING = "IsMoving";
    private const string ANIM_IS_JUMPING = "IsJumping";
    private const string ANIM_PUNCH = "Punch";
    private const string ANIM_SPEED = "Speed"; // For sprite-based movement speed
    
    void Start()
    {
        // Get required components
        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<PlayerHealth>();
        
        // Auto-find animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log($"Auto-found Animator: {(animator != null ? "SUCCESS" : "FAILED")}");
        }
        else
        {
            Debug.Log("Animator already assigned");
        }
        
        // Auto-find sprite renderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            Debug.Log($"Auto-found SpriteRenderer: {(spriteRenderer != null ? "SUCCESS" : "FAILED")}");
        }
        else
        {
            Debug.Log("SpriteRenderer already assigned");
        }
        
        // Comprehensive animation diagnostics
        Debug.Log("=== ANIMATION DIAGNOSTICS ===");
        if (animator != null)
        {
            Debug.Log($"Animator enabled: {animator.enabled}");
            Debug.Log($"Animator controller: {(animator.runtimeAnimatorController != null ? animator.runtimeAnimatorController.name : "NULL")}");
            Debug.Log($"Animator state: {animator.GetCurrentAnimatorStateInfo(0).IsName("Any State")}");
            Debug.Log($"Animator layer count: {animator.layerCount}");
        }
        else
        {
            Debug.LogError("ANIMATOR IS NULL - Add Animator component to Player GameObject!");
        }
        
        if (spriteRenderer != null)
        {
            Debug.Log($"SpriteRenderer enabled: {spriteRenderer.enabled}");
            Debug.Log($"Sprite assigned: {(spriteRenderer.sprite != null ? spriteRenderer.sprite.name : "NULL")}");
            Debug.Log($"SpriteRenderer visible: {spriteRenderer.isVisible}");
            Debug.Log($"SpriteRenderer color: {spriteRenderer.color}");
            Debug.Log($"SpriteRenderer sorting layer: {spriteRenderer.sortingLayerName}");
            Debug.Log($"SpriteRenderer order in layer: {spriteRenderer.sortingOrder}");
        }
        else
        {
            Debug.LogError("SPRITE RENDERER IS NULL - Add SpriteRenderer component to Player GameObject!");
        }
        
        Debug.Log("=== END DIAGNOSTICS ===");
        
        // Ensure we have a Rigidbody
        if (rb == null)
        {
            Debug.LogError("PlayerController requires a Rigidbody component!");
        }
        else
        {
            // Configure Rigidbody for proper jumping
            ConfigureRigidbody();
        }
    }
    
    /// <summary>
    /// Configures the Rigidbody for optimal jumping performance
    /// </summary>
    private void ConfigureRigidbody()
    {
        // Ensure gravity is enabled
        rb.useGravity = true;
        
        // Set reasonable mass for jumping
        if (rb.mass > 10f)
        {
            rb.mass = 1f;
            Debug.Log("Rigidbody mass adjusted to 1f for better jumping");
        }
        
        // Reduce drag for better jump response
        if (rb.linearDamping > 1f)
        {
            rb.linearDamping = 0.5f;
            Debug.Log("Rigidbody drag reduced to 0.5f for better jumping");
        }
        
        // Ensure angular drag is reasonable
        if (rb.angularDamping > 5f)
        {
            rb.angularDamping = 0.05f;
            Debug.Log("Rigidbody angular drag reduced for better performance");
        }
        
        Debug.Log($"Rigidbody configured - Mass: {rb.mass}, Drag: {rb.linearDamping}, UseGravity: {rb.useGravity}");
    }
    
    void Update()
    {
        HandleInput();
        CheckGrounded();
        UpdateAnimations();
        ApplyCustomGravity();
        CheckPlayerVisibility();
    }
    
    /// <summary>
    /// Handles all player input for movement, jumping, and punching
    /// </summary>
    private void HandleInput()
    {
        // Check if player is in recoil - prevent movement during recoil
        if (playerHealth != null && playerHealth.IsInRecoil)
        {
            // Stop movement during recoil
            Vector3 currentVelocity = rb.linearVelocity;
            currentVelocity.x = 0f;
            currentVelocity.z = 0f;
            rb.linearVelocity = currentVelocity;
            return; // Exit early - no input processing during recoil
        }
        
        // Movement input (WASD)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Convert to isometric movement
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        
        // Apply movement
        Vector3 movementVelocity = rb.linearVelocity;
        
        if (moveDirection.magnitude > 0.1f)
        {
            // Set target velocity directly
            movementVelocity.x = moveDirection.x * moveSpeed;
            movementVelocity.z = moveDirection.z * moveSpeed;
            
            // Rotate player to face movement direction
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
        else
        {
            // Stop horizontal movement
            movementVelocity.x = 0f;
            movementVelocity.z = 0f;
        }
        
        // Apply the final velocity
        rb.linearVelocity = movementVelocity;
        
        // Jump input (Space)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
        
        // Punch input (V)
        if (Input.GetKeyDown(KeyCode.V) && canPunch)
        {
            Punch();
        }
    }
    
    /// <summary>
    /// Makes the player jump by applying upward force
    /// </summary>
    private void Jump()
    {
        Debug.Log($"=== JUMP DEBUG ===");
        Debug.Log($"Jump force value: {jumpForce}");
        Debug.Log($"Rigidbody mass: {rb.mass}");
        Debug.Log($"Rigidbody drag: {rb.linearDamping}");
        Debug.Log($"Rigidbody angular drag: {rb.angularDamping}");
        Debug.Log($"Rigidbody use gravity: {rb.useGravity}");
        Debug.Log($"Velocity before jump: {rb.linearVelocity}");
        
        // Apply jump force using AddForce for better physics
        Vector3 jumpVector = Vector3.up * jumpForce;
        rb.AddForce(jumpVector, ForceMode.Impulse);
        
        Debug.Log($"Jump vector applied: {jumpVector}");
        Debug.Log($"Velocity after jump: {rb.linearVelocity}");
        
        isGrounded = false;
        
        // Trigger jump animation
        if (animator != null)
        {
            SetAnimationTrigger(ANIM_IS_JUMPING);
        }
    }
    
    /// <summary>
    /// Performs a punch attack with cooldown and damage
    /// </summary>
    private void Punch()
    {
        // Check cooldown
        if (Time.time - lastPunchTime < punchCooldown)
        {
            return;
        }
        
        // Update punch time and set cooldown
        lastPunchTime = Time.time;
        canPunch = false;
        
        // Trigger punch animation
        if (animator != null)
        {
            SetAnimationTrigger(ANIM_PUNCH);
        }
        
        // Perform punch attack
        PerformPunchAttack();
        
        // Reset punch ability after cooldown
        Invoke(nameof(ResetPunch), punchCooldown);
    }
    
    /// <summary>
    /// Performs the actual punch attack by detecting enemies in range
    /// </summary>
    private void PerformPunchAttack()
    {
        // Find all enemies within punch range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, punchRange);
        
        foreach (Collider hitCollider in hitColliders)
        {
            // Check if the hit object is an enemy
            var enemyHealth = hitCollider.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Deal damage to the enemy
                enemyHealth.TakeDamage(punchDamage);
                Debug.Log($"Punched enemy for {punchDamage} damage!");
                
                // Show UI feedback
                if (gameUI != null)
                {
                    gameUI.ShowPunchHitEffect(punchDamage, hitCollider.transform.position);
                    gameUI.UpdateEnemyHealthUI(enemyHealth);
                }
                
                // Apply recoil force to the enemy
                ApplyPunchRecoil(hitCollider.transform);
            }
        }
    }
    
    /// <summary>
    /// Applies recoil force to enemies when punched
    /// </summary>
    private void ApplyPunchRecoil(Transform enemyTransform)
    {
        // Get enemy rigidbody
        Rigidbody enemyRb = enemyTransform.GetComponent<Rigidbody>();
        if (enemyRb == null) 
        {
            Debug.LogWarning($"Enemy {enemyTransform.name} has no Rigidbody component!");
            return;
        }
        
        // Calculate recoil direction (away from player)
        Vector3 recoilDirection = (enemyTransform.position - transform.position).normalized;
        
        // Check enemy type for different recoil behavior
        var dummyEnemy = enemyTransform.GetComponent<DummyEnemy>();
        var enemyAI = enemyTransform.GetComponent<EnemyAI>();
        
        if (dummyEnemy != null)
        {
            // Dummy enemy: no recoil (stays completely still)
            Debug.Log($"Dummy enemy {enemyTransform.name} hit - no recoil applied");
        }
        else if (enemyAI != null)
        {
            // AI enemy: strong recoil (gets knocked back significantly)
            Vector3 totalForce = (recoilDirection * punchRecoilForce) + (Vector3.up * punchRecoilUpwardForce);
            enemyRb.AddForce(totalForce, ForceMode.Impulse);
            Debug.Log($"AI enemy {enemyTransform.name} hit with force: {totalForce.magnitude} (Direction: {recoilDirection}, Upward: {punchRecoilUpwardForce})");
        }
        else
        {
            // Generic enemy: default recoil
            Vector3 totalForce = (recoilDirection * (punchRecoilForce * 0.7f)) + (Vector3.up * (punchRecoilUpwardForce * 0.7f));
            enemyRb.AddForce(totalForce, ForceMode.Impulse);
            Debug.Log($"Generic enemy {enemyTransform.name} hit with force: {totalForce.magnitude}");
        }
    }
    
    /// <summary>
    /// Resets the punch ability after cooldown
    /// </summary>
    private void ResetPunch()
    {
        canPunch = true;
    }
    
    /// <summary>
    /// Checks if the player is grounded using a raycast
    /// </summary>
    private void CheckGrounded()
    {
        // Cast a ray downward to check for ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayer);
    }
    
    /// <summary>
    /// Updates animation parameters based on current state
    /// </summary>
    private void UpdateAnimations()
    {
        // Debug animation system
        if (animator == null)
        {
            Debug.LogWarning("Animator is null! Animation system not working.");
            return;
        }
        
        // Calculate movement speed for animation
        float speed = rb.linearVelocity.magnitude;
        
        // Update movement animation
        bool isMoving = speed > 0.1f;
        
        // Debug animation parameters
        Debug.Log($"=== ANIMATION DEBUG ===");
        Debug.Log($"Speed: {speed}, IsMoving: {isMoving}, IsGrounded: {isGrounded}");
        
        // Safely set animation parameters (only if they exist)
        SetAnimationParameter(ANIM_IS_MOVING, isMoving);
        SetAnimationParameter(ANIM_SPEED, speed);
        SetAnimationParameter(ANIM_IS_JUMPING, !isGrounded);
        
        // Check if animator is enabled
        if (!animator.enabled)
        {
            Debug.LogWarning("Animator component is disabled!");
        }
        
        // Check if animator has controller
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("Animator has no controller assigned!");
        }
        
        // Handle sprite flipping for movement direction (for sprite-based animations)
        if (spriteRenderer != null && isMoving)
        {
            // Flip sprite based on movement direction
            if (rb.linearVelocity.x < -0.1f)
            {
                spriteRenderer.flipX = true; // Face left
            }
            else if (rb.linearVelocity.x > 0.1f)
            {
                spriteRenderer.flipX = false; // Face right
            }
        }
    }
    
    /// <summary>
    /// Safely sets animation parameters only if they exist
    /// </summary>
    private void SetAnimationParameter(string parameterName, bool value)
    {
        try
        {
            animator.SetBool(parameterName, value);
            Debug.Log($"Set {parameterName}: {value}");
        }
        catch (System.ArgumentException)
        {
            Debug.LogWarning($"Animation parameter '{parameterName}' does not exist in Animator Controller!");
        }
    }
    
    /// <summary>
    /// Safely sets animation parameters only if they exist
    /// </summary>
    private void SetAnimationParameter(string parameterName, float value)
    {
        try
        {
            animator.SetFloat(parameterName, value);
            Debug.Log($"Set {parameterName}: {value}");
        }
        catch (System.ArgumentException)
        {
            Debug.LogWarning($"Animation parameter '{parameterName}' does not exist in Animator Controller!");
        }
    }
    
    /// <summary>
    /// Safely sets animation triggers only if they exist
    /// </summary>
    private void SetAnimationTrigger(string triggerName)
    {
        try
        {
            animator.SetTrigger(triggerName);
            Debug.Log($"Set {triggerName}: Triggered");
        }
        catch (System.ArgumentException)
        {
            Debug.LogWarning($"Animation trigger '{triggerName}' does not exist in Animator Controller!");
        }
    }
    
    /// <summary>
    /// Applies custom gravity only when player is in the air
    /// </summary>
    private void ApplyCustomGravity()
    {
        // Only apply extra gravity when not grounded and falling (not rising from jump)
        if (!isGrounded && rb.linearVelocity.y < 0)
        {
            // Apply extra downward force for faster falling, but only when already falling
            rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);
        }
    }
    
    /// <summary>
    /// Draws gizmos in the editor to visualize punch range
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Draw punch range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, punchRange);
        
        // Draw ground check ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * 1.1f);
    }
    
    /// <summary>
    /// Checks if the player is visible and properly positioned
    /// </summary>
    private void CheckPlayerVisibility()
    {
        if (spriteRenderer != null)
        {
            Debug.Log($"=== VISIBILITY CHECK ===");
            Debug.Log($"Player position: {transform.position}");
            Debug.Log($"Player scale: {transform.localScale}");
            Debug.Log($"Player rotation: {transform.rotation}");
            Debug.Log($"SpriteRenderer bounds: {spriteRenderer.bounds}");
            Debug.Log($"SpriteRenderer size: {(spriteRenderer.sprite != null ? spriteRenderer.sprite.rect.size : Vector2.zero)}");
            Debug.Log($"Camera position: {(Camera.main != null ? Camera.main.transform.position.ToString() : "No main camera")}");
            
            // Check if player is in camera view
            if (Camera.main != null)
            {
                Vector3 viewportPoint = Camera.main.WorldToViewportPoint(transform.position);
                Debug.Log($"Player in viewport: {viewportPoint.x:F2}, {viewportPoint.y:F2}, {viewportPoint.z:F2}");
                bool inView = viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0;
                Debug.Log($"Player visible in camera: {inView}");
            }
            Debug.Log($"=== END VISIBILITY CHECK ===");
        }
    }
} 