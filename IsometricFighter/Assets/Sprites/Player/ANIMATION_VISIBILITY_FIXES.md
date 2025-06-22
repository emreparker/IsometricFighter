# Animation Visibility Fixes

## Common Reasons Animations Aren't Visible

### 1. Missing Components
**Problem**: Player GameObject missing required components
**Solution**: 
- Add **Animator** component
- Add **SpriteRenderer** component
- Ensure both are enabled

### 2. No Sprite Assigned
**Problem**: SpriteRenderer has no sprite assigned
**Solution**:
- Select Player GameObject
- In SpriteRenderer component, assign a sprite to "Sprite" field
- Use any sprite from your sprite sheet

### 3. Wrong Camera Setup
**Problem**: Camera not positioned to see the player
**Solution**:
- Ensure camera is positioned above and behind player
- Check camera's Culling Mask includes the player's layer
- Verify camera is orthographic for 2D sprites

### 4. Player Position Issues
**Problem**: Player is outside camera view or wrong position
**Solution**:
- Set player position to (0, 0, 0) or visible area
- Check player scale isn't too small (should be 1,1,1)
- Ensure player isn't behind other objects

### 5. Sorting Layer Issues
**Problem**: Player sprite behind other objects
**Solution**:
- Set SpriteRenderer "Sorting Layer" to "Default" or higher
- Increase "Order in Layer" value
- Check if other objects are in front

### 6. Animator Controller Issues
**Problem**: No controller assigned or wrong controller
**Solution**:
- Create Animator Controller in Animations/Player/
- Assign it to Animator component
- Add at least one animation state

### 7. Animation Clip Issues
**Problem**: Animation clips not properly set up
**Solution**:
- Create animation clips from your sprite sheets
- Ensure clips have sprites assigned
- Set appropriate frame rate (12-24 fps)

## Quick Fix Checklist

### In Unity Editor:
1. **Select Player GameObject**
2. **Check Components**:
   - [ ] Animator component exists and enabled
   - [ ] SpriteRenderer component exists and enabled
   - [ ] Sprite assigned in SpriteRenderer
3. **Check Position**:
   - [ ] Player position is (0, 0, 0) or visible
   - [ ] Player scale is (1, 1, 1)
4. **Check Camera**:
   - [ ] Camera can see player position
   - [ ] Camera is orthographic (for 2D)
5. **Check Animator**:
   - [ ] Controller assigned
   - [ ] At least one animation state exists

### Debug Information:
Run the game and check Console for:
- "=== ANIMATION DIAGNOSTICS ===" messages
- "=== VISIBILITY CHECK ===" messages
- Any error messages about missing components

## Most Common Fix:
1. **Add SpriteRenderer** to Player GameObject
2. **Assign any sprite** to the SpriteRenderer
3. **Position camera** to see the player
4. **Create basic Animator Controller** with one idle state

## Test Steps:
1. Run the game
2. Check Console for diagnostic messages
3. Look for the player in Scene view
4. Move camera around to find player
5. Check if player is behind other objects 