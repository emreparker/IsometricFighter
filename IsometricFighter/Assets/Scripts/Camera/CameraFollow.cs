using UnityEngine;

/// <summary>
/// Camera controller that smoothly follows the player at an isometric angle.
/// Provides smooth movement and optional look-ahead functionality.
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private string playerTag = "Player";
    
    [Header("Follow Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 10f, -10f);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool useSmoothDamping = true;
    
    [Header("Look Ahead Settings")]
    [SerializeField] private bool enableLookAhead = false;
    [SerializeField] private float lookAheadDistance = 3f;
    [SerializeField] private float lookAheadSpeed = 2f;
    
    [Header("Boundaries")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;
    
    // Private variables
    private Vector3 desiredPosition;
    private Vector3 smoothedPosition;
    private Vector3 lookAheadOffset;
    
    void Start()
    {
        // Auto-find player if not assigned
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag(playerTag);
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                Debug.LogWarning($"CameraFollow: No GameObject found with tag '{playerTag}'. Please assign a target manually.");
            }
        }
        
        // Initialize camera position
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate desired position
        CalculateDesiredPosition();
        
        // Apply smooth movement
        if (useSmoothDamping)
        {
            smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        }
        else
        {
            smoothedPosition = desiredPosition;
        }
        
        // Apply boundaries if enabled
        if (useBoundaries)
        {
            smoothedPosition = ClampToBoundaries(smoothedPosition);
        }
        
        // Update camera position
        transform.position = smoothedPosition;
        
        // Make camera look at target
        LookAtTarget();
    }
    
    /// <summary>
    /// Calculates the desired camera position based on target and offset.
    /// </summary>
    private void CalculateDesiredPosition()
    {
        Vector3 basePosition = target.position + offset;
        
        // Add look-ahead if enabled
        if (enableLookAhead)
        {
            CalculateLookAhead();
            basePosition += lookAheadOffset;
        }
        
        desiredPosition = basePosition;
    }
    
    /// <summary>
    /// Calculates look-ahead offset based on player movement.
    /// </summary>
    private void CalculateLookAhead()
    {
        // Get player velocity (assuming it has a Rigidbody)
        Vector3 playerVelocity = Vector3.zero;
        Rigidbody playerRb = target.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerVelocity = playerRb.linearVelocity;
        }
        
        // Calculate look-ahead direction (ignore Y velocity)
        Vector3 lookDirection = new Vector3(playerVelocity.x, 0f, playerVelocity.z).normalized;
        
        // Apply look-ahead offset
        lookAheadOffset = Vector3.Lerp(lookAheadOffset, lookDirection * lookAheadDistance, lookAheadSpeed * Time.deltaTime);
    }
    
    /// <summary>
    /// Makes the camera look at the target.
    /// </summary>
    private void LookAtTarget()
    {
        Vector3 lookPosition = target.position;
        
        // Add slight upward offset for better isometric view
        lookPosition.y += 1f;
        
        transform.LookAt(lookPosition);
    }
    
    /// <summary>
    /// Clamps the camera position to defined boundaries.
    /// </summary>
    private Vector3 ClampToBoundaries(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, minX, maxX),
            position.y,
            Mathf.Clamp(position.z, minZ, maxZ)
        );
    }
    
    /// <summary>
    /// Sets a new target for the camera to follow.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    /// <summary>
    /// Updates the camera offset.
    /// </summary>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    /// <summary>
    /// Updates the smooth speed.
    /// </summary>
    public void SetSmoothSpeed(float newSpeed)
    {
        smoothSpeed = newSpeed;
    }
    
    /// <summary>
    /// Draws gizmos in the editor to visualize camera boundaries and offset.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        
        // Draw camera offset
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position + offset, 0.5f);
        Gizmos.DrawLine(target.position, target.position + offset);
        
        // Draw boundaries
        if (useBoundaries)
        {
            Gizmos.color = Color.red;
            Vector3 center = new Vector3((minX + maxX) * 0.5f, target.position.y, (minZ + maxZ) * 0.5f);
            Vector3 size = new Vector3(maxX - minX, 1f, maxZ - minZ);
            Gizmos.DrawWireCube(center, size);
        }
        
        // Draw look-ahead
        if (enableLookAhead)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(target.position + offset + lookAheadOffset, 0.3f);
        }
    }
} 