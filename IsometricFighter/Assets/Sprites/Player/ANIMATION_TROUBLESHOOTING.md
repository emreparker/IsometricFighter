# Animation Troubleshooting Guide

## Common Animation Issues and Solutions

### 1. Animator Component Issues

**Problem**: Animator is null or not found
**Solution**: 
- Ensure Player GameObject has Animator component
- Check if Animator component is enabled
- Verify Animator Controller is assigned

**Check in Inspector**:
- Select Player GameObject
- Look for Animator component
- Ensure "Controller" field has your PlayerAnimator assigned
- Ensure "Enabled" checkbox is checked

### 2. Animator Controller Issues

**Problem**: No controller assigned or wrong controller
**Solution**:
- Create Animator Controller in Assets/Animations/Player/
- Name it "PlayerAnimator"
- Assign it to the Animator component
- Add animation clips as states

**Steps**:
1. Right-click in Animations/Player folder
2. Create → Animator Controller
3. Name it "PlayerAnimator"
4. Double-click to open Animator window
5. Add animation clips as states

### 3. Animation Parameter Issues

**Problem**: Animation parameters don't match
**Solution**: Ensure these parameters exist in your Animator Controller:
- **IsMoving** (Bool)
- **IsJumping** (Bool) 
- **Punch** (Trigger)
- **Speed** (Float)

**To add parameters**:
1. Open Animator window
2. Click "Parameters" tab
3. Click "+" to add parameters
4. Set correct types and names

### 4. Animation Clip Issues

**Problem**: Animation clips not playing
**Solution**:
- Ensure clips are properly created
- Check clip length and frame rate
- Verify sprites are assigned to clips

**To create clips**:
1. Select sliced sprites
2. Right-click → Create → Animation → Animation Clip
3. Name them: "PlayerIdle", "PlayerWalk", "PlayerJump", "PlayerPunch"
4. Drag sprites into Animation window in correct order

### 5. Sprite Renderer Issues

**Problem**: Sprites not visible
**Solution**:
- Ensure SpriteRenderer component exists
- Check if sprite is assigned
- Verify sprite import settings

**Check in Inspector**:
- Select Player GameObject
- Look for SpriteRenderer component
- Ensure "Sprite" field has a sprite assigned
- Check "Enabled" checkbox

### 6. Import Settings Issues

**Problem**: Sprites look wrong or don't animate
**Solution**:
- Set Texture Type to "Sprite (2D and UI)"
- Set Sprite Mode to "Multiple"
- Use Point filter for pixel art
- Set appropriate Pixels Per Unit

### 7. Debug Information

The script now provides detailed debug logs. Check Console for:
- "Auto-found Animator: SUCCESS/FAILED"
- "Auto-found SpriteRenderer: SUCCESS/FAILED"
- "=== ANIMATION DEBUG ===" messages
- Warning messages about missing components

### 8. Quick Fix Checklist

- [ ] Player GameObject has Animator component
- [ ] Animator Controller is assigned
- [ ] Animation parameters exist (IsMoving, IsJumping, Punch, Speed)
- [ ] Animation clips are created and named correctly
- [ ] SpriteRenderer component exists
- [ ] Sprites are properly imported and sliced
- [ ] No error messages in Console

### 9. Testing Animation

To test if animations work:
1. Move the player (WASD) - should trigger walk animation
2. Jump (Spacebar) - should trigger jump animation
3. Punch (V) - should trigger punch animation
4. Stand still - should play idle animation

### 10. Common Error Messages

- **"Animator is null"** → Add Animator component
- **"No controller assigned"** → Create and assign Animator Controller
- **"Animator disabled"** → Enable Animator component
- **"SpriteRenderer is null"** → Add SpriteRenderer component 