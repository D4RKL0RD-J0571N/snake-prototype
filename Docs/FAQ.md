# Frequently Asked Questions (FAQ)

## Setup & Installation

### Q: The game won't start or shows errors in Unity Console
**A**: Check the following:
1. Ensure you're using Unity 6000.3.8f1 or later
2. Verify URP package is installed (v17.3.0+)
3. Make sure all ScriptableObject configurations are assigned in the GameBootstrapper
4. Check that the snake segment prefab is assigned to SnakeView

### Q: Grid is invisible or shows wrong colors
**A**: This is usually a material/shader issue:
1. Ensure the GridMaterial in GridConfig uses the `SnakePrototype/GridDynamic` shader
2. Check that the material isn't set to 0 scale
3. Verify the grid GameObject has the correct renderer component
4. Make sure the GridManager is properly initialized

### Q: Snake doesn't appear or is invisible
**A**: Common causes:
1. **Missing Segment Prefab**: The SnakeView component MUST have a segment prefab assigned
2. **Wrong Shader**: Ensure the segment prefab uses `SnakePrototype/SnakeBody` shader
3. **Camera Position**: The camera should follow the snake's head position
4. **Scale Issues**: Check that the segment prefab isn't scaled to 0

### Q: Audio is not playing or too quiet
**A**: Audio system troubleshooting:
1. Check that all 5 AudioSources are assigned to GameBootstrapper
2. Verify audio clips are assigned in LevelConfig
3. AudioManager sets music layers to 0 volume by default - they fade in with detection
4. Check the Base Volume setting in AudioManager

## Performance & Optimization

### Q: Game runs slowly or has frame drops
**A**: Performance optimization tips:
1. Check Unity Profiler for bottlenecks
2. Reduce grid size in GridConfig for testing
3. Disable VFX in VFXConfiguration temporarily
4. Ensure you're using URP with the correct renderer settings
5. Check that particle systems aren't emitting too frequently

### Q: Memory usage is too high
**A**: Memory optimization:
1. Verify object pooling is working for VFX
2. Check for memory leaks in event subscriptions
3. Ensure textures are compressed appropriately
4. Monitor garbage collection in the Profiler

### Q: Build size is too large
**A**: Build optimization:
1. Exclude unused assets from build
2. Compress audio files
3. Optimize texture compression settings
4. Remove debug symbols from release builds

## Gameplay & Mechanics

### Q: Detection system seems inconsistent
**A**: Detection system troubleshooting:
1. Check that DetectionSource components are properly configured
2. Verify DetectionManager stress threshold settings
3. Ensure multiple detection sources are weighted correctly
4. Check that DetectionConeView renderers are working

### Q: Snake movement feels unresponsive
**A**: Input and movement issues:
1. Check InputManager settings for move interval
2. Verify Input System package is properly configured
3. Check for input conflicts in Unity Input settings
4. Ensure GameBootstrapper is registering InputManager first

### Q: Score isn't updating correctly
**A**: Score system issues:
1. Verify ScoreManager is registered as a service
2. Check that EnergyCoreManager publishes score events
3. Ensure UIManager is listening to score change events
4. Check that ScoreManager properly resets on level start

## Architecture & Development

### Q: How do I add a new game system?
**A**: Adding new systems:
1. Create Manager class implementing `IGameService`
2. Create corresponding View class if needed
3. Register the service in `GameBootstrapper.Awake()`
4. Use `GameEventManager` for communication with other systems
5. Follow the Manager/View separation pattern

### Q: Events aren't being received
**A**: Event system debugging:
1. Ensure `GameEventManager.Publish()` is being called
2. Check that listeners are added before events are published
3. Verify event types match exactly
4. Check for null reference exceptions in event handlers
5. Ensure services are registered in the correct order

### Q: ServiceLocator returns null
**A**: Service registration issues:
1. Check that services are registered in `GameBootstrapper.Awake()`
2. Verify service classes implement `IGameService`
3. Ensure you're calling `ServiceLocator.Get<T>()` with the correct type
4. Check for initialization order dependencies

## Visual Effects & Shaders

### Q: Custom shaders aren't working
**A**: Shader troubleshooting:
1. Ensure shaders are included in the build
2. Check that URP renderer includes the custom shader passes
3. Verify material properties are set correctly
4. Check for shader compilation errors in Console

### Q: VFX aren't playing
**A**: VFX system issues:
1. Check VFXConfiguration for missing prefab assignments
2. Verify VFXManager is registered and initialized
3. Ensure particle systems are properly configured
4. Check that VFXAnimateProperties are set up correctly

### Q: UI elements aren't styling correctly
**A**: UI Toolkit issues:
1. Verify USS files are assigned to UIDocument
2. Check that UI elements have correct class names
3. Ensure UIManager is applying palette colors
4. Check for USS syntax errors

## Platform-Specific Issues

### Q: Build fails on specific platforms
**A**: Platform-specific troubleshooting:
1. Check platform-specific requirements in ProjectSettings
2. Verify all dependencies support the target platform
3. Check for platform-specific API usage
4. Ensure build settings are configured correctly

### Q: Input doesn't work on certain platforms
**A**: Input system issues:
1. Verify Input System actions are defined correctly
2. Check platform-specific input configurations
3. Ensure InputManager is handling platform differences
4. Test with different input devices

## Debugging & Troubleshooting

### Q: How do I debug the ServiceLocator system?
**A**: ServiceLocator debugging:
1. Check Unity Console for service registration logs
2. Use `Debug.Log` in service Initialize() methods
3. Verify service order in GameBootstrapper
4. Check for circular dependencies

### Q: How do I profile the game?
**A**: Profiling tips:
1. Use Unity Profiler to identify bottlenecks
2. Check memory usage with Memory Profiler
3. Monitor frame time and garbage collection
4. Profile both in Editor and in builds

### Q: How do I test individual systems?
**A**: Testing strategies:
1. Use existing EditMode tests as templates
2. Create isolated test scenes for specific systems
3. Use Unity Test Runner for automated testing
4. Test services independently before integration

## Common Unity Issues

### Q: Prefab references are missing
**A**: Prefab reference issues:
1. Check that prefabs exist in the correct folders
2. Verify prefab names match exactly
3. Check for case sensitivity in file names
4. Ensure prefabs are not deleted or moved

### Q: Script compilation errors
**A**: Compilation issues:
1. Check for missing using statements
2. Verify namespace declarations
3. Check for circular references
4. Ensure all dependencies are available

### Q: Package manager issues
**A**: Package troubleshooting:
1. Verify manifest.json is correctly formatted
2. Check package versions are compatible
3. Resolve package conflicts
4. Clear package cache if needed

## Getting Help

### Community Resources
- **Unity Forums**: https://forum.unity.com/
- **Unity Documentation**: https://docs.unity3d.com/
- **URP Documentation**: https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest/

### Project-Specific Help
- **GitHub Issues**: Report bugs and request features
- **Documentation**: Check project docs for detailed information
- **Code Comments**: Read inline documentation in source files

### Best Practices
1. **Read the logs**: Always check Unity Console first
2. **Isolate the problem**: Test systems independently
3. **Check the basics**: Verify configurations and references
4. **Use version control**: Commit working states frequently
5. **Document issues**: Keep track of recurring problems

---

**Still having issues?** Check the [Setup Walkthrough](Walkthrough_Setup.md) for detailed setup instructions or open an issue on GitHub with:
- Unity version and platform
- Steps to reproduce the issue
- Console errors (if any)
- Expected vs. actual behavior
