# Unity Developer Agent - Specialized Documentation

## Overview

The `unity-developer` agent provides Unity-specific expertise, optimization guidance, and best practices for the Snake Prototype project using Unity 6000.3.8f1 with Universal Render Pipeline (URP).

## Core Responsibilities

### Unity Engine Expertise
- Unity 6000.3.8f1 feature utilization
- URP (Universal Render Pipeline) optimization
- ScriptableObject architecture patterns
- Unity-specific performance optimization
- Build pipeline management

### Asset Pipeline Management
- Asset bundle optimization
- Texture and audio compression
- Shader optimization for URP
- Prefab organization and optimization
- Memory-efficient asset loading

### Platform-Specific Unity Features
- Unity platform settings optimization
- Platform-specific build configurations
- Unity Analytics integration
- Unity Cloud Build management
- Unity Services integration

## Unity 6000.3.8f1 Specific Features

### New Features Utilization

#### ScriptableRenderPipeline Improvements
```csharp
// URP 17.3.0 specific optimizations
public class URPOptimizations : MonoBehaviour
{
    [SerializeField] private UniversalRenderPipelineAsset _urpAsset;
    
    private void Start()
    {
        OptimizeURPSettings();
    }
    
    private void OptimizeURPSettings()
    {
        // Enable URP-specific optimizations
        _urpAsset.renderScale = 1.0f; // Adjust for performance
        _urpAsset.shadowDistance = 50f; // Optimize shadow distance
        _urpAsset.shadowCascadeCount = 2; // Reduce shadow cascades for performance
        
        // MSAA configuration
        _urpAsset.msaaSampleCount = 2; // 2x MSAA for balance
        
        // Post-processing settings
        _urpAsset.supportsHDR = true;
        _urpApp.colorGradingMode = ColorGradingMode.HighDynamicRange;
    }
}
```

#### Input System 1.18.0 Integration
```csharp
// Unity Input System advanced usage
public class AdvancedInputManager : MonoBehaviour
{
    private InputActions _inputActions;
    private Vector2 _moveInput;
    private bool _isPaused;
    
    private void Awake()
    {
        _inputActions = new InputActions();
        
        // Configure input actions
        _inputActions.Gameplay.Move.performed += OnMovePerformed;
        _inputActions.Gameplay.Move.canceled += OnMoveCanceled;
        _inputActions.Gameplay.Pause.performed += OnPausePerformed;
        
        // Enable enhanced input features
        InputSystem.settings.SetDefaultDeviceStateChangeHandler("Keyboard", OnKeyboardChange);
        InputSystem.settings.SetDefaultDeviceStateChangeHandler("Gamepad", OnGamepadChange);
    }
    
    private void OnEnable()
    {
        _inputActions.Enable();
        
        // Enable enhanced touch support for mobile
        if (Application.isMobilePlatform)
        {
            InputSystem.EnableDevice(Touchscreen.current);
        }
    }
    
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>();
        
        // Normalize diagonal movement
        if (_moveInput.magnitude > 1f)
        {
            _moveInput = _moveInput.normalized;
        }
        
        // Apply deadzone
        _moveInput = ApplyDeadzone(_moveInput, 0.1f);
    }
    
    private Vector2 ApplyDeadzone(Vector2 input, float deadzone)
    {
        if (input.magnitude < deadzone)
            return Vector2.zero;
            
        return input.normalized * ((input.magnitude - deadzone) / (1f - deadzone));
    }
}
```

### URP Optimization Strategies

#### Custom URP Renderer Configuration
```csharp
// Custom URP renderer for Snake Prototype
public class SnakePrototypeRenderer : ScriptableRendererFeature
{
    [SerializeField] private Shader _snakeShader;
    [SerializeField] private Shader _gridShader;
    [SerializeField] private Shader _vfxShader;
    
    private class CustomRenderPass : ScriptableRenderPass
    {
        private Material _snakeMaterial;
        private Material _gridMaterial;
        private Material _vfxMaterial;
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Custom snake rendering with optimized material
            if (_snakeMaterial != null)
            {
                DrawSnakeGeometry(context, renderingData);
            }
            
            // Grid rendering with dynamic properties
            if (_gridMaterial != null)
            {
                DrawGridWithDetection(context, renderingData);
            }
            
            // VFX rendering with performance optimization
            if (_vfxMaterial != null)
            {
                DrawOptimizedVFX(context, renderingData);
            }
        }
        
        private void DrawSnakeGeometry(ScriptableRenderContext context, RenderingData renderingData)
        {
            // Optimized snake rendering
            CommandBuffer cmd = CommandBufferPool.Get("SnakeRendering");
            
            // Set material properties
            _snakeMaterial.SetFloat("_Time", Time.time);
            _snakeMaterial.SetVector("_HeadPosition", GetSnakeHeadPosition());
            _snakeMaterial.SetInt("_SegmentCount", GetSnakeSegmentCount());
            
            // Draw with instancing for performance
            cmd.DrawMeshInstanced(GetSnakeMesh(), Matrix4x4.identity, _snakeMaterial, 0, GetSnakeInstances());
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}
```

#### URP Shader Optimization
```hlsl
// Optimized URP shader for Snake Prototype
Shader "SnakePrototype/OptimizedSnakeBody"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GradientRatio ("Gradient Ratio", Range(0,1)) = 0.5
        _FresnelPower ("Fresnel Power", Range(1,10)) = 3
        _Dissolve ("Dissolve", Range(0,1)) = 0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDirWS : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _GradientRatio;
                float _FresnelPower;
                float _Dissolve;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.viewDirWS = GetWorldSpaceViewDir(TransformObjectToWorld(input.positionOS.xyz));
                
                return output;
            }
            
            float4 frag(Varyings input) : SV_Target
            {
                // Optimized fragment shader
                float2 uv = input.uv;
                
                // Sample texture with mipmaps
                float4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
                
                // Fresnel effect for edge highlighting
                float fresnel = pow(1.0 - saturate(dot(input.normalWS, normalize(input.viewDirWS))), _FresnelPower);
                
                // Gradient based on UV position
                float gradient = saturate(uv.y * _GradientRatio);
                
                // Dissolve effect
                float dissolveNoise = texColor.a;
                float dissolveAlpha = saturate((dissolveNoise - _Dissolve) / 0.1);
                
                // Combine effects
                float3 finalColor = lerp(texColor.rgb, _Color.rgb, gradient);
                finalColor += fresnel * 0.2;
                
                return float4(finalColor, dissolveAlpha);
            }
            ENDHLSL
        }
    }
}
```

## Performance Optimization

### Unity Profiler Integration

#### Custom Profiler Markers
```csharp
// Custom profiling for Snake systems
public static class SnakeProfiler
{
    private static readonly ProfilerMarker SnakeMovementMarker = new ProfilerMarker("Snake.Movement");
    private static readonly ProfilerMarker DetectionUpdateMarker = new ProfilerMarker("Detection.Update");
    private static readonly ProfilerMarker VFXUpdateMarker = new ProfilerMarker("VFX.Update");
    
    public static void ProfileSnakeMovement(System.Action action)
    {
        using (SnakeMovementMarker.Auto())
        {
            action();
        }
    }
    
    public static void ProfileDetectionUpdate(System.Action action)
    {
        using (DetectionUpdateMarker.Auto())
        {
            action();
        }
    }
    
    public static void ProfileVFXUpdate(System.Action action)
    {
        using (VFXUpdateMarker.Auto())
        {
            action();
        }
    }
}

// Usage in SnakeManager
public class SnakeManager : IGameService
{
    public void Update(float deltaTime)
    {
        SnakeProfiler.ProfileSnakeMovement(() =>
        {
            UpdateMovement(deltaTime);
            CheckCollisions();
            UpdateVisuals();
        });
    }
}
```

#### Memory Optimization
```csharp
// Memory optimization utilities
public class MemoryOptimizer
{
    private static readonly Dictionary<string, Object> _objectPool = new Dictionary<string, Object>();
    
    public static T GetPooledObject<T>(string key, Func<T> createFunc) where T : Object
    {
        if (_objectPool.TryGetValue(key, out var pooledObject) && pooledObject != null)
        {
            _objectPool.Remove(key);
            return (T)pooledObject;
        }
        
        return createFunc();
    }
    
    public static void ReturnToPool<T>(string key, T obj) where T : Object
    {
        if (obj == null) return;
        
        // Reset object state if possible
        if (obj is Component component)
        {
            component.gameObject.SetActive(false);
        }
        
        _objectPool[key] = obj;
    }
    
    public static void ClearPool()
    {
        foreach (var kvp in _objectPool)
        {
            if (kvp.Value != null)
            {
                Object.DestroyImmediate(kvp.Value);
            }
        }
        _objectPool.Clear();
        
        // Force garbage collection
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
    }
}

// Optimized VFX system
public class OptimizedVFXManager : MonoBehaviour
{
    [SerializeField] private GameObject _vfxPrefab;
    [SerializeField] private int _poolSize = 10;
    
    private Queue<GameObject> _vfxPool = new Queue<GameObject>();
    
    private void Start()
    {
        // Pre-warm pool
        for (int i = 0; i < _poolSize; i++)
        {
            var vfx = Instantiate(_vfxPrefab);
            vfx.SetActive(false);
            _vfxPool.Enqueue(vfx);
        }
    }
    
    public void PlayVFX(Vector3 position)
    {
        if (_vfxPool.Count == 0)
        {
            // Create new VFX if pool is empty
            var vfx = Instantiate(_vfxPrefab);
            vfx.transform.position = position;
            StartCoroutine(ReturnToPoolAfterDelay(vfx, 2f));
            return;
        }
        
        var pooledVFX = _vfxPool.Dequeue();
        pooledVFX.transform.position = position;
        pooledVFX.SetActive(true);
        
        StartCoroutine(ReturnToPoolAfterDelay(pooledVFX, 2f));
    }
    
    private IEnumerator ReturnToPoolAfterDelay(GameObject vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        vfx.SetActive(false);
        _vfxPool.Enqueue(vfx);
    }
}
```

## Build Pipeline Management

### Automated Build Configuration

#### Custom Build Script
```csharp
// Advanced build script for multiple platforms
public class AdvancedBuildScript
{
    [MenuItem("Build/Build All Platforms")]
    public static void BuildAllPlatforms()
    {
        BuildWindows();
        BuildLinux();
        BuildMac();
        BuildAndroid();
        BuildiOS();
    }
    
    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "Builds/Windows/SnakePrototype.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };
        
        // Windows-specific settings
        PlayerSettings.companyName = "SnakePrototype";
        PlayerSettings.productName = "SNEK: Labyrinth Protocol";
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        
        // Optimize for Windows
        EditorUserBuildSettings.standaloneBuildSubtarget = StandaloneBuildSubtarget.Player;
        
        BuildPipeline.BuildPlayer(buildOptions);
    }
    
    [MenuItem("Build/Build Android")]
    public static void BuildAndroid()
    {
        BuildPlayerOptions buildOptions = new BuildPlayerOptions
        {
            scenes = GetScenes(),
            locationPathName = "Builds/Android/SnakePrototype.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };
        
        // Android-specific settings
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
        PlayerSettings.Android.bundleVersionCode = 1;
        
        // Optimize for Android
        PlayerSettings.stripEngineCode = true;
        PlayerSettings.stripUnusedMeshComponents = true;
        
        BuildPipeline.BuildPlayer(buildOptions);
    }
    
    private static string[] GetScenes()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }
}
```

#### Build Optimization
```csharp
// Build optimization utilities
public class BuildOptimizer
{
    [MenuItem("Build/Optimize Build Settings")]
    public static void OptimizeBuildSettings()
    {
        // Enable stripping
        PlayerSettings.stripEngineCode = true;
        PlayerSettings.stripUnusedMeshComponents = true;
        PlayerSettings.optimizeMeshData = true;
        
        // Optimize for size
        EditorUserBuildSettings.il2CppCodeGeneration = Il2CppCodeGeneration.OptimizeSize;
        
        // Compression settings
        EditorUserBuildSettings.compressFilesInPackage = true;
        EditorUserBuildSettings.development = false;
        
        // Script optimization
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Standalone, ApiCompatibilityLevel.NET_Standard_2_1);
        
        Debug.Log("Build settings optimized for production");
    }
    
    [MenuItem("Build/Analyze Build Size")]
    public static void AnalyzeBuildSize()
    {
        // Analyze build size and provide recommendations
        var buildSize = GetBuildSize();
        var textureSize = GetTextureSize();
        var audioSize = GetAudioSize();
        
        Debug.Log($"Total Build Size: {buildSize / 1024 / 1024:F1} MB");
        Debug.Log($"Texture Size: {textureSize / 1024 / 1024:F1} MB");
        Debug.Log($"Audio Size: {audioSize / 1024 / 1024:F1} MB");
        
        // Provide optimization recommendations
        if (textureSize > 50 * 1024 * 1024) // 50MB
        {
            Debug.LogWarning("Consider compressing textures further");
        }
        
        if (audioSize > 20 * 1024 * 1024) // 20MB
        {
            Debug.LogWarning("Consider compressing audio files");
        }
    }
    
    private static long GetBuildSize()
    {
        // Calculate build size
        string buildPath = "Builds";
        if (Directory.Exists(buildPath))
        {
            return Directory.GetFiles(buildPath, "*", SearchOption.AllDirectories)
                .Sum(file => new FileInfo(file).Length);
        }
        return 0;
    }
}
```

## Asset Pipeline Optimization

### Texture Optimization

#### Automatic Texture Compression
```csharp
// Texture optimization utilities
public class TextureOptimizer
{
    [MenuItem("Assets/Optimize All Textures")]
    public static void OptimizeAllTextures()
    {
        string[] texturePaths = AssetDatabase.FindAssets("t:Texture")
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();
            
        foreach (string path in texturePaths)
        {
            OptimizeTexture(path);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private static void OptimizeTexture(string path)
    {
        var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
        if (textureImporter == null) return;
        
        // Optimize based on platform
        textureImporter.textureCompression = TextureImporterCompression.Compressed;
        textureImporter.compressionQuality = 50; // Balance quality and size
        
        // Set appropriate size based on usage
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture != null)
        {
            if (texture.width > 1024 || texture.height > 1024)
            {
                textureImporter.maxTextureSize = 1024;
            }
        }
        
        // Generate mipmaps for performance
        textureImporter.mipmapEnabled = true;
        textureImporter.filterMode = FilterMode.Bilinear;
        
        // Save changes
        EditorUtility.SetDirty(textureImporter);
        textureImporter.SaveAndReimport();
    }
}
```

### Audio Optimization

#### Audio Compression Settings
```csharp
// Audio optimization utilities
public class AudioOptimizer
{
    [MenuItem("Assets/Optimize All Audio")]
    public static void OptimizeAllAudio()
    {
        string[] audioPaths = AssetDatabase.FindAssets("t:AudioClip")
            .Select(AssetDatabase.GUIDToAssetPath)
            .ToArray();
            
        foreach (string path in audioPaths)
        {
            OptimizeAudio(path);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    private static void OptimizeAudio(string path)
    {
        var audioImporter = AssetImporter.GetAtPath(path) as AudioImporter;
        if (audioImporter == null) return;
        
        var settings = audioImporter.defaultSampleSettings;
        
        // Optimize for mobile
        settings.compressionFormat = AudioCompressionFormat.Vorbis;
        settings.quality = 0.7f; // Good quality with reasonable size
        
        // Set sample rate
        settings.sampleRateSetting = AudioSampleRateSetting._22050Hz;
        
        // Optimize for memory
        audioImporter.loadInBackground = true;
        
        audioImporter.defaultSampleSettings = settings;
        EditorUtility.SetDirty(audioImporter);
        audioImporter.SaveAndReimport();
    }
}
```

## Testing and Quality Assurance

### Unity Test Framework Integration

#### Automated Testing Setup
```csharp
// Custom test utilities for Unity
public class SnakeTestUtilities
{
    public static GameObject CreateTestSnake()
    {
        var snake = new GameObject("TestSnake");
        snake.AddComponent<SnakeManager>();
        snake.AddComponent<SnakeView>();
        
        // Configure test data
        var config = ScriptableObject.CreateInstance<SnakeConfiguration>();
        config.MoveInterval = 0.1f;
        config.MaxLength = 10;
        
        var snakeManager = snake.GetComponent<SnakeManager>();
        snakeManager.Initialize(config);
        
        return snake;
    }
    
    public static GridManager CreateTestGrid(int width = 20, int height = 20)
    {
        var config = ScriptableObject.CreateInstance<GridConfiguration>();
        config.Width = width;
        config.Height = height;
        config.CellSize = 1f;
        
        return new GridManager(config);
    }
    
    public static void AssertVector2IntEqual(Vector2Int expected, Vector2Int actual, string message = "")
    {
        if (expected != actual)
        {
            throw new AssertionException($"Vector2Int not equal. Expected: {expected}, Actual: {actual}. {message}");
        }
    }
}

// Performance testing
public class PerformanceTests
{
    [Test, Performance]
    public void SnakeMovement_PerformanceTest()
    {
        var snake = SnakeTestUtilities.CreateTestSnake();
        var snakeManager = snake.GetComponent<SnakeManager>();
        
        Measure.Method(() =>
        {
            snakeManager.MoveSnake(Vector2Int.right);
        })
        .WarmupCount(10)
        .MeasurementCount(100)
        .IterationsPerMeasurement(1000)
        .GC()
        .Run();
    }
}
```

## Troubleshooting

### Common Unity Issues

#### Build Issues
- **Problem**: Build fails with IL2CPP errors
- **Solution**: Check for unsupported .NET features, reduce code complexity
- **Prevention**: Regular build testing, code review for IL2CPP compatibility

#### Performance Issues
- **Problem**: Frame drops on mobile
- **Solution**: Profile with Unity Profiler, optimize shaders, reduce draw calls
- **Prevention**: Regular performance profiling, set performance budgets

#### Memory Issues
- **Problem**: Memory leaks in builds
- **Solution**: Check for circular references, proper cleanup, memory profiling
- **Prevention**: Regular memory profiling, proper object lifecycle management

### Debugging Tools

#### Unity Console Extensions
```csharp
// Enhanced debugging utilities
public static class UnityDebugExtensions
{
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogWithStack(string message, Object context = null)
    {
        Debug.Log($"{message}\n{StackTraceUtility.ExtractStackTrace()}", context);
    }
    
    [Conditional("DEVELOPMENT_BUILD")]
    public static void DrawDebugGrid(GridManager gridManager)
    {
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                var position = new Vector3(x, 0, y);
                var color = gridManager.IsWall(new Vector2Int(x, y)) ? Color.red : Color.green;
                Debug.DrawWireCube(position, Vector3.one * 0.9f, color, 1f);
            }
        }
    }
    
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogSnakePath(SnakeManager snakeManager)
    {
        var path = snakeManager.BodyParts;
        for (int i = 0; i < path.Count - 1; i++)
        {
            var start = new Vector3(path[i].x, 0, path[i].y);
            var end = new Vector3(path[i + 1].x, 0, path[i + 1].y);
            Debug.DrawLine(start, end, Color.blue, 1f);
        }
    }
}
```

---

**Last Updated**: 2026-02-20  
**Skill Version**: 1.0  
**Unity Version**: 6000.3.8f1  
**Maintainer**: Unity Development Team
