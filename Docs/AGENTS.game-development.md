# Game Development Agent - Specialized Documentation

## Overview

The `game-development` agent provides high-level game development guidance, platform-specific considerations, and cross-platform development strategies for the Snake Prototype project.

## Core Responsibilities

### Platform Selection and Strategy
- Evaluate platform requirements and constraints
- Recommend optimal platform combinations
- Assess platform-specific performance implications
- Guide platform expansion strategies

### Development Architecture
- High-level architectural decision making
- Technology stack recommendations
- Cross-platform compatibility planning
- Scalability and performance planning

### Project Management
- Development workflow optimization
- Team coordination strategies
- Risk assessment and mitigation
- Quality assurance planning

## Platform-Specific Guidance

### Desktop Platforms (Windows/Mac/Linux)

#### Performance Targets
- **Frame Rate**: 60 FPS minimum, 120 FPS target
- **Memory Usage**: < 200MB runtime, < 500MB peak
- **Load Times**: < 3 seconds initial, < 1 second level loads
- **Disk Space**: < 500MB installed size

#### Optimization Strategies
```csharp
// Desktop-specific optimizations
public class DesktopOptimizations
{
    // Use full resolution and quality settings
    public void ApplyDesktopSettings()
    {
        QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
        Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, true);
        
        // Enable advanced features
        QualitySettings.shadowDistance = 100f;
        QualitySettings.lodBias = 2f;
        QualitySettings.maximumLODLevel = 0;
    }
    
    // Desktop input handling
    public void ConfigureDesktopInput()
    {
        // Keyboard and mouse input
        InputSystem.RegisterLayout<KeyboardMouseLayout>();
        
        // Gamepad support
        InputSystem.RegisterLayout<XboxControllerLayout>();
        InputSystem.RegisterLayout<PlayStationControllerLayout>();
    }
}
```

#### Platform-Specific Features
- **Windows**: Xbox Live integration, Windows Store support
- **Mac**: iCloud sync, Retina display optimization
- **Linux**: Steam integration, various desktop environments

### Mobile Platforms (iOS/Android)

#### Performance Targets
- **Frame Rate**: 30 FPS minimum, 60 FPS target
- **Memory Usage**: < 100MB runtime, < 200MB peak
- **Battery Life**: > 2 hours continuous play
- **Storage**: < 200MB installed size

#### Mobile Optimization
```csharp
// Mobile-specific optimizations
public class MobileOptimizations
{
    // Reduce quality for mobile
    public void ApplyMobileSettings()
    {
        QualitySettings.SetQualityLevel(2); // Medium quality
        QualitySettings.shadowDistance = 20f;
        QualitySettings.lodBias = 1f;
        QualitySettings.maximumLODLevel = 2;
        
        // Mobile-specific settings
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
    }
    
    // Touch input handling
    public void ConfigureMobileInput()
    {
        InputSystem.RegisterLayout<TouchLayout>();
        
        // Configure touch sensitivity
        InputSettings.defaultDeadzoneMin = 0.1f;
        InputSettings.defaultDeadzoneMax = 0.9f;
    }
    
    // Battery optimization
    public void OptimizeForBattery()
    {
        // Reduce update frequency when not focused
        Application.runInBackground = false;
        
        // Lower quality when battery is low
        if (SystemInfo.batteryLevel < 0.2f)
        {
            QualitySettings.SetQualityLevel(1);
        }
    }
}
```

#### Mobile-Specific Features
- **iOS**: Game Center integration, Touch ID support
- **Android**: Google Play Games, various screen densities
- **Cross-Platform**: Cloud saves, social sharing

### Console Platforms (PlayStation/Xbox/Switch)

#### Console Requirements
- **Frame Rate**: Stable 60 FPS, no drops below 30 FPS
- **Memory**: Strict memory budgets per platform
- **Certification**: Platform-specific certification requirements
- **Input**: Full controller support, proper button mapping

#### Console Integration
```csharp
// Console-specific implementations
public class ConsoleIntegration
{
    // Platform-specific initialization
    public void InitializeConsoleFeatures()
    {
        #if UNITY_PS5
            InitializePlayStationFeatures();
        #elif UNITY_XBOX
            InitializeXboxFeatures();
        #elif UNITY_SWITCH
            InitializeSwitchFeatures();
        #endif
    }
    
    private void InitializePlayStationFeatures()
    {
        // PlayStation Network integration
        PSNIntegration.Initialize();
        
        // Trophy support
        TrophyManager.Initialize();
        
        // DualSense haptics
        DualSenseController.Initialize();
    }
    
    private void InitializeXboxFeatures()
    {
        // Xbox Live integration
        XboxLive.Initialize();
        
        // Achievement system
        AchievementManager.Initialize();
        
        // Xbox controller features
        XboxController.Initialize();
    }
}
```

## Testing Strategies

### Cross-Platform Testing

#### Automated Testing
```python
# Cross-platform test automation
class CrossPlatformTester:
    def __init__(self):
        self.platforms = ["Windows", "Mac", "Linux", "iOS", "Android"]
        self.test_suites = ["unit_tests", "integration_tests", "performance_tests"]
        
    def run_cross_platform_tests(self):
        results = {}
        
        for platform in self.platforms:
            platform_results = {}
            
            for test_suite in self.test_suites:
                result = self.run_test_suite(platform, test_suite)
                platform_results[test_suite] = result
                
            results[platform] = platform_results
            
        return self.analyze_results(results)
        
    def run_test_suite(self, platform, test_suite):
        # Platform-specific test execution
        if platform in ["Windows", "Mac", "Linux"]:
            return self.run_desktop_tests(platform, test_suite)
        elif platform in ["iOS", "Android"]:
            return self.run_mobile_tests(platform, test_suite)
            
    def analyze_results(self, results):
        # Analyze cross-platform compatibility
        issues = []
        
        for platform, platform_results in results.items():
            for test_suite, result in platform_results.items():
                if not result.passed:
                    issues.append(f"{platform}: {test_suite} failed")
                    
        return {
            "overall_status": "PASS" if len(issues) == 0 else "FAIL",
            "issues": issues,
            "recommendations": self.generate_recommendations(issues)
        }
```

#### Manual Testing Checklist
- [ ] **Input Testing**: All input methods work correctly
- [ ] **Performance**: Frame rates meet platform targets
- [ ] **UI Scaling**: UI scales properly on all screen sizes
- [ ] **Platform Features**: Platform-specific features work
- [ ] **Certification**: Meets platform certification requirements

### Performance Testing

#### Benchmark Suite
```csharp
// Performance benchmarking
public class PerformanceBenchmark
{
    private struct BenchmarkResult
    {
        public float AverageFPS;
        public float MinimumFPS;
        public float MaximumFPS;
        public long MemoryUsage;
        public float LoadTime;
    }
    
    public BenchmarkResult RunBenchmark(string platform)
    {
        var result = new BenchmarkResult();
        
        // FPS benchmark
        result = RunFPSBenchmark(result);
        
        // Memory benchmark
        result = RunMemoryBenchmark(result);
        
        // Load time benchmark
        result = RunLoadTimeBenchmark(result);
        
        return result;
    }
    
    private BenchmarkResult RunFPSBenchmark(BenchmarkResult result)
    {
        var fpsValues = new List<float>();
        var startTime = Time.time;
        
        // Run for 60 seconds
        while (Time.time - startTime < 60f)
        {
            fpsValues.Add(1.0f / Time.deltaTime);
            yield return null;
        }
        
        result.AverageFPS = fpsValues.Average();
        result.MinimumFPS = fpsValues.Min();
        result.MaximumFPS = fpsValues.Max();
        
        return result;
    }
    
    private BenchmarkResult RunMemoryBenchmark(BenchmarkResult result)
    {
        // Force garbage collection
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
        System.GC.Collect();
        
        result.MemoryUsage = System.GC.GetTotalMemory(false);
        return result;
    }
}
```

## Development Workflows

### Continuous Integration

#### Multi-Platform CI/CD
```yaml
# Multi-platform CI/CD pipeline
name: Multi-Platform Build

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build-desktop:
    strategy:
      matrix:
        platform: [Windows, Linux, macOS]
        unity-version: [6000.3.8f1]
        
    runs-on: ${{ matrix.platform }}
    
    steps:
    - name: Checkout
      uses: actions/checkout@v3
      
    - name: Setup Unity
      uses: game-ci/unity-test-runner@v2
      with:
        unityVersion: ${{ matrix.unity-version }}
        
    - name: Build Desktop
      uses: game-ci/unity-builder@v2
      with:
        targetPlatform: ${{ matrix.platform }}
        
  build-mobile:
    strategy:
      matrix:
        platform: [iOS, Android]
        
    runs-on: macos-latest
    
    steps:
    - name: Checkout
      uses: actions/checkout@v3
      
    - name: Setup Unity
      uses: game-ci/unity-test-runner@v2
      with:
        unityVersion: 6000.3.8f1
        
    - name: Build Mobile
      uses: game-ci/unity-builder@v2
      with:
        targetPlatform: ${{ matrix.platform }}
```

### Deployment Strategies

#### Platform-Specific Deployment
```python
# Platform deployment automation
class PlatformDeployment:
    def __init__(self):
        self.platforms = {
            "Windows": self.deploy_windows,
            "Mac": self.deploy_mac,
            "Linux": self.deploy_linux,
            "iOS": self.deploy_ios,
            "Android": self.deploy_android
        }
        
    def deploy_all_platforms(self, version):
        results = {}
        
        for platform, deploy_func in self.platforms.items():
            try:
                result = deploy_func(version)
                results[platform] = {"status": "SUCCESS", "result": result}
            except Exception as e:
                results[platform] = {"status": "FAILED", "error": str(e)}
                
        return results
        
    def deploy_windows(self, version):
        # Windows Store deployment
        return self.deploy_to_windows_store(version)
        
    def deploy_ios(self, version):
        # App Store deployment
        return self.deploy_to_app_store(version)
        
    def deploy_android(self, version):
        # Google Play deployment
        return self.deploy_to_google_play(version)
```

## Best Practices

### Code Organization

#### Platform Abstraction
```csharp
// Platform abstraction layer
public interface IPlatformService
{
    void Initialize();
    string GetPlatformName();
    void ShowAchievements();
    void ShowLeaderboards();
    void ShareScore(int score);
}

public class PlatformService : IPlatformService
{
    private IPlatformService _platformImplementation;
    
    public void Initialize()
    {
        #if UNITY_IOS
            _platformImplementation = new iOSService();
        #elif UNITY_ANDROID
            _platformImplementation = new AndroidService();
        #elif UNITY_STANDALONE
            _platformImplementation = new DesktopService();
        #endif
        
        _platformImplementation.Initialize();
    }
    
    // Delegate to platform-specific implementation
    public string GetPlatformName() => _platformImplementation.GetPlatformName();
    public void ShowAchievements() => _platformImplementation.ShowAchievements();
    public void ShowLeaderboards() => _platformImplementation.ShowLeaderboards();
    public void ShareScore(int score) => _platformImplementation.ShareScore(score);
}
```

### Performance Optimization

#### Platform-Specific Optimizations
```csharp
// Platform-specific performance manager
public class PlatformPerformanceManager
{
    public void OptimizeForPlatform()
    {
        var platform = Application.platform;
        
        switch (platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxPlayer:
                OptimizeForDesktop();
                break;
                
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                OptimizeForMobile();
                break;
                
            case RuntimePlatform.WebGLPlayer:
                OptimizeForWebGL();
                break;
        }
    }
    
    private void OptimizeForDesktop()
    {
        // Desktop optimizations
        QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
        Application.targetFrameRate = -1; // Unlimited
        QualitySettings.vSyncCount = 1;
    }
    
    private void OptimizeForMobile()
    {
        // Mobile optimizations
        QualitySettings.SetQualityLevel(2);
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 1;
        
        // Reduce texture quality
        QualitySettings.masterTextureLimit = 1;
        
        // Disable expensive features
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = ShadowResolution.Low;
    }
    
    private void OptimizeForWebGL()
    {
        // WebGL optimizations
        QualitySettings.SetQualityLevel(1);
        Application.targetFrameRate = 30;
        QualitySettings.vSyncCount = 0;
        
        // WebGL-specific settings
        QualitySettings.particleRaycastBudget = 64;
        QualitySettings.asyncUploadTimeSlice = 2;
    }
}
```

## Troubleshooting

### Common Platform Issues

#### Performance Issues
- **Problem**: Low frame rate on mobile
- **Solution**: Reduce quality settings, optimize shaders, use object pooling
- **Prevention**: Profile regularly, set performance budgets

#### Memory Issues
- **Problem**: Memory leaks on mobile
- **Solution**: Check for circular references, proper cleanup, memory profiling
- **Prevention**: Regular memory profiling, proper object lifecycle management

#### Input Issues
- **Problem**: Input not working on certain platforms
- **Solution**: Check input system setup, platform-specific input handling
- **Prevention**: Test on all target platforms early and often

### Debugging Tools

#### Platform Debugging
```csharp
// Platform debugging utilities
public class PlatformDebugger
{
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogPlatformInfo()
    {
        Debug.Log($"Platform: {Application.platform}");
        Debug.Log($"System Memory: {SystemInfo.systemMemorySize}MB");
        Debug.Log($"Graphics Memory: {SystemInfo.graphicsMemorySize}MB");
        Debug.Log($"Processor: {SystemInfo.processorType}");
        Debug.Log($"Graphics Device: {SystemInfo.graphicsDeviceName}");
    }
    
    [Conditional("DEVELOPMENT_BUILD")]
    public static void LogPerformanceMetrics()
    {
        Debug.Log($"Current FPS: {1.0f / Time.deltaTime:F1}");
        Debug.Log($"Memory Usage: {System.GC.GetTotalMemory(false) / 1024 / 1024}MB");
        Debug.Log($"Draw Calls: {UnityEngine.Profiling.Profiler.GetTotalDrawCalls()}");
    }
}
```

---

**Last Updated**: 2026-02-20  
**Skill Version**: 1.0  
**Maintainer**: Game Development Team
