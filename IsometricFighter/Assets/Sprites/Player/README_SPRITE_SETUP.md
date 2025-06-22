# Sprite Sheet Setup Guide

## Adding Sprite Sheets to Player Character

### 1. Import Sprite Sheets
- Place your sprite sheet files in this folder
- Supported formats: PNG, JPG, TGA

### 2. Configure Import Settings
1. Select your sprite sheet in Unity
2. In Inspector, set:
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Multiple
   - **Pixels Per Unit**: 16 or 32 (based on sprite size)
   - **Filter Mode**: Point (no filter) for pixel art
   - **Compression**: None for pixel art

### 3. Slice Sprite Sheet
1. Click "Sprite Editor" button
2. Click "Slice" → "Grid By Cell Size" or "Grid By Cell Count"
3. Set dimensions based on your sprite sheet
4. Click "Apply"

### 4. Create Animation Clips
1. Select sliced sprites
2. Right-click → Create → Animation → Animation Clip
3. Name them: "PlayerIdle", "PlayerWalk", "PlayerJump", "PlayerPunch"
4. Drag sprites into Animation window in order

### 5. Create Animator Controller
1. Go to Assets/Animations/Player/
2. Right-click → Create → Animator Controller
3. Name it "PlayerAnimator"
4. Add animation clips as states
5. Set up transitions between states

### 6. Setup Player GameObject
1. Add SpriteRenderer component to Player
2. Assign the PlayerAnimator to the Animator component
3. The PlayerController script will auto-find components

### Animation States Needed:
- **Idle**: When player is not moving
- **Walk**: When player is moving
- **Jump**: When player is in air
- **Punch**: When player attacks

### Transition Rules:
- Idle ↔ Walk (when movement starts/stops)
- Any state → Jump (when jumping)
- Any state → Punch (when punching)
- Jump → Idle/Walk (when landing)
- Punch → Idle/Walk (when punch ends) 