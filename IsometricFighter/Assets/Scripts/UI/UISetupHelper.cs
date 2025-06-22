using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper script for setting up the UI system in Unity.
/// Creates a clean, minimal UI setup with only essential elements.
/// </summary>
public class UISetupHelper : MonoBehaviour
{
    [Header("Quick Setup")]
    [SerializeField] private bool autoSetupOnStart = false;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            CleanSetupUI();
        }
    }
    
    /// <summary>
    /// Creates a clean, minimal UI setup
    /// </summary>
    [ContextMenu("Clean Setup UI")]
    public void CleanSetupUI()
    {
        // Find or create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }
        
        // Add GameUI script if not present
        GameUI gameUI = canvas.GetComponent<GameUI>();
        if (gameUI == null)
        {
            gameUI = canvas.gameObject.AddComponent<GameUI>();
        }
        
        // Clear existing UI elements
        ClearExistingUI(canvas);
        
        // Create essential UI elements
        CreateEssentialUI(canvas, gameUI);
        
        // Connect GameUI to all enemies
        ConnectGameUIToEnemies(gameUI);
        
        Debug.Log("Clean UI Setup Complete! Check the Canvas for essential elements only.");
    }
    
    /// <summary>
    /// Clears existing UI elements to avoid duplicates
    /// </summary>
    private void ClearExistingUI(Canvas canvas)
    {
        // Remove all child objects except the Canvas components
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(canvas.transform.GetChild(i).gameObject);
        }
    }
    
    /// <summary>
    /// Creates only essential UI elements
    /// </summary>
    private void CreateEssentialUI(Canvas canvas, GameUI gameUI)
    {
        // Player Health Bar (top-left)
        GameObject playerHealthGO = CreateSimpleSlider("PlayerHealthBar", canvas.transform, new Vector2(200, 20));
        RectTransform playerHealthRect = playerHealthGO.GetComponent<RectTransform>();
        playerHealthRect.anchorMin = new Vector2(0, 1);
        playerHealthRect.anchorMax = new Vector2(0, 1);
        playerHealthRect.anchoredPosition = new Vector2(120, -30);
        
        // Player Health Text
        GameObject playerHealthTextGO = CreateSimpleText("PlayerHealthText", canvas.transform, "Player: 100/100");
        RectTransform playerHealthTextRect = playerHealthTextGO.GetComponent<RectTransform>();
        playerHealthTextRect.anchorMin = new Vector2(0, 1);
        playerHealthTextRect.anchorMax = new Vector2(0, 1);
        playerHealthTextRect.anchoredPosition = new Vector2(120, -50);
        
        // Enemy Health Bar (top-right)
        GameObject enemyHealthGO = CreateSimpleSlider("EnemyHealthBar", canvas.transform, new Vector2(200, 20));
        RectTransform enemyHealthRect = enemyHealthGO.GetComponent<RectTransform>();
        enemyHealthRect.anchorMin = new Vector2(1, 1);
        enemyHealthRect.anchorMax = new Vector2(1, 1);
        enemyHealthRect.anchoredPosition = new Vector2(-120, -30);
        
        // Enemy Health Text
        GameObject enemyHealthTextGO = CreateSimpleText("EnemyHealthText", canvas.transform, "Enemy: 50/50");
        RectTransform enemyHealthTextRect = enemyHealthTextGO.GetComponent<RectTransform>();
        enemyHealthTextRect.anchorMin = new Vector2(1, 1);
        enemyHealthTextRect.anchorMax = new Vector2(1, 1);
        enemyHealthTextRect.anchoredPosition = new Vector2(-120, -50);
        
        // Controls Text (bottom-left, small)
        GameObject controlsTextGO = CreateSimpleText("ControlsText", canvas.transform, "WASD: Move | SPACE: Jump | V: Punch");
        RectTransform controlsTextRect = controlsTextGO.GetComponent<RectTransform>();
        controlsTextRect.anchorMin = new Vector2(0, 0);
        controlsTextRect.anchorMax = new Vector2(0, 0);
        controlsTextRect.anchoredPosition = new Vector2(10, 10);
        controlsTextRect.sizeDelta = new Vector2(300, 30);
        
        // Make controls text smaller
        TextMeshProUGUI controlsText = controlsTextGO.GetComponent<TextMeshProUGUI>();
        controlsText.fontSize = 14;
        controlsText.color = new Color(1, 1, 1, 0.7f);
        
        // Game Status Text (top-center)
        GameObject statusTextGO = CreateSimpleText("GameStatusText", canvas.transform, "Fight!");
        RectTransform statusRect = statusTextGO.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 1);
        statusRect.anchorMax = new Vector2(0.5f, 1);
        statusRect.anchoredPosition = new Vector2(0, -80);
        
        // Assign to GameUI script (you'll need to do this manually)
        Debug.Log("Essential UI created! Assign the elements to GameUI script in Inspector:");
        Debug.Log("- Player Health Bar: " + playerHealthGO.name);
        Debug.Log("- Player Health Text: " + playerHealthTextGO.name);
        Debug.Log("- Enemy Health Bar: " + enemyHealthGO.name);
        Debug.Log("- Enemy Health Text: " + enemyHealthTextGO.name);
        Debug.Log("- Game Status Text: " + statusTextGO.name);
    }
    
    /// <summary>
    /// Creates a simple slider without complex hierarchy
    /// </summary>
    private GameObject CreateSimpleSlider(string name, Transform parent, Vector2 size)
    {
        GameObject sliderGO = new GameObject(name);
        sliderGO.transform.SetParent(parent, false);
        
        Slider slider = sliderGO.AddComponent<Slider>();
        RectTransform rect = sliderGO.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        
        // Simple background
        GameObject backgroundGO = new GameObject("Background");
        backgroundGO.transform.SetParent(sliderGO.transform, false);
        Image backgroundImage = backgroundGO.AddComponent<Image>();
        backgroundImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
        RectTransform backgroundRect = backgroundGO.GetComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.sizeDelta = Vector2.zero;
        
        // Simple fill
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(sliderGO.transform, false);
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.color = Color.green;
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        
        slider.fillRect = fillRect;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;
        
        return sliderGO;
    }
    
    /// <summary>
    /// Creates a simple text element
    /// </summary>
    private GameObject CreateSimpleText(string name, Transform parent, string text)
    {
        GameObject textGO = new GameObject(name);
        textGO.transform.SetParent(parent, false);
        
        TextMeshProUGUI tmpText = textGO.AddComponent<TextMeshProUGUI>();
        tmpText.text = text;
        tmpText.fontSize = 18;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Center;
        
        RectTransform rect = textGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 30);
        
        return textGO;
    }
    
    /// <summary>
    /// Connects GameUI to all enemies in the scene
    /// </summary>
    private void ConnectGameUIToEnemies(GameUI gameUI)
    {
        // Find all enemies with EnemyHealth components
        EnemyHealth[] enemies = FindObjectsOfType<EnemyHealth>();
        
        foreach (EnemyHealth enemy in enemies)
        {
            enemy.gameUI = gameUI;
        }
        
        // Also connect to PlayerController and EnemyAI
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            player.gameUI = gameUI;
        }
        
        EnemyAI[] enemyAIs = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemyAI in enemyAIs)
        {
            enemyAI.gameUI = gameUI;
        }
        
        Debug.Log($"Connected GameUI to {enemies.Length} enemies, {players.Length} players, and {enemyAIs.Length} enemy AIs");
    }
} 