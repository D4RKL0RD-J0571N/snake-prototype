# DevOps Documentation - Snake Prototype

Comprehensive DevOps guide covering build pipelines, CI/CD, testing automation, deployment procedures, and monitoring for the Snake Prototype project.

## 🚀 Overview

This document outlines the complete DevOps infrastructure and processes for the Snake Prototype project, ensuring reliable, automated, and efficient development and deployment workflows.

### DevOps Philosophy

- **Automation First**: Automate repetitive tasks to reduce human error
- **Continuous Integration**: Integrate code changes frequently and automatically
- **Continuous Deployment**: Deploy changes automatically after successful testing
- **Infrastructure as Code**: Manage infrastructure through code
- **Monitoring and Observability**: Monitor everything to detect issues early

---

## 🏗️ Build Pipeline Configuration

### Unity Build System

#### Build Configuration Files

```yaml
# .github/workflows/unity-build.yml
name: Unity Build Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    container: unityci/editor:6000.3.8f1-linux-il2cpp
    
    steps:
    - name: Checkout Repository
      uses: actions/checkout@v3
      with:
        lfs: true
        
    - name: Cache Unity Library
      uses: actions/cache@v3
      with:
        path: Library
        key: Library-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
        restore-keys: |
          Library-
          
    - name: Restore Dependencies
      run: |
        unity-editor -batchmode -quit -projectPath . -logFile - -executeMethod BuildScript.RestorePackages
        
    - name: Run Tests
      run: |
        unity-editor -batchmode -quit -projectPath . -logFile - -runTests -platform EditMode -testResults results.xml
        
    - name: Build Windows
      run: |
        unity-editor -batchmode -quit -projectPath . -logFile - -buildTarget StandaloneWindows64 -executeMethod BuildScript.BuildWindows
        
    - name: Build Linux
      run: |
        unity-editor -batchmode -quit -projectPath . -logFile - -buildTarget StandaloneLinux64 -executeMethod BuildScript.BuildLinux
        
    - name: Upload Artifacts
      uses: actions/upload-artifact@v3
      with:
        name: build-artifacts
        path: Build/
```

#### Build Script Implementation

```csharp
// Assets/Editor/BuildScript.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
using System;

public class BuildScript
{
    [MenuItem("Build/Build Windows")]
    public static void BuildWindows()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenes();
        buildPlayerOptions.locationPathName = "Build/Windows/SnakePrototype.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Windows build succeeded: " + summary.totalSize + " bytes");
        }
        else
        {
            Debug.LogError("Windows build failed");
            EditorApplication.Exit(1);
        }
    }
    
    [MenuItem("Build/Build Linux")]
    public static void BuildLinux()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = GetScenes();
        buildPlayerOptions.locationPathName = "Build/Linux/SnakePrototype";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Linux build succeeded: " + summary.totalSize + " bytes");
        }
        else
        {
            Debug.LogError("Linux build failed");
            EditorApplication.Exit(1);
        }
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
    
    public static void RestorePackages()
    {
        UnityEditor.PackageManager.Client.Resolve();
    }
}
```

### Build Configuration Management

#### Build Settings Configuration

```json
// ProjectSettings/BuildSettings.json (partial)
{
  "buildSettings": [
    {
      "buildTarget": "StandaloneWindows64",
      "subtarget": -1,
      "includedScenes": [
        "Assets/Scenes/Level_01.unity",
        "Assets/Scenes/SampleScene.unity"
      ],
      "locationPathName": "Build/Windows/SnakePrototype.exe",
      "options": 0,
      "compressionType": 2
    },
    {
      "buildTarget": "StandaloneLinux64",
      "subtarget": -1,
      "includedScenes": [
        "Assets/Scenes/Level_01.unity",
        "Assets/Scenes/SampleScene.unity"
      ],
      "locationPathName": "Build/Linux/SnakePrototype",
      "options": 0,
      "compressionType": 2
    }
  ]
}
```

---

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

#### Main CI/CD Pipeline

```yaml
# .github/workflows/ci-cd.yml
name: CI/CD Pipeline

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]
  release:
    types: [ published ]

env:
  UNITY_VERSION: 6000.3.8f1
  UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}

jobs:
  test:
    name: Test Suite
    runs-on: ubuntu-latest
    container: unityci/editor:${{ env.UNITY_VERSION }}-linux-il2cpp
    
    steps:
    - name: Checkout
      uses: actions/checkout@v3
      with:
        lfs: true
        
    - name: Cache Library
      uses: actions/cache@v3
      with:
        path: Library
        key: Library-test-${{ hashFiles('Assets/**', 'Packages/**', 'ProjectSettings/**') }}
        
    - name: Run Unit Tests
      run: |
        unity-editor -batchmode -quit -projectPath . -logFile - -runTests -platform EditMode -testPlatform EditMode -testResults results.xml -coverageOptions generateAdditionalMetrics
        
    - name: Run Integration Tests
      run: |
        unity-editor -batchmode -quit -projectPath . -logFile - -runTests -platform PlayMode -testPlatform PlayMode -testResults results-playmode.xml
        
    - name: Upload Test Results
      uses: actions/upload-artifact@v3
      if: always()
      with:
        name: test-results
        path: |
          results.xml
          results-playmode.xml
          
    - name: Coverage Report
      uses: codecov/codecov-action@v3
      with:
        file: ./results.xml
        
  build:
    name: Build and Package
    needs: test
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ubuntu-latest, windows-latest]
        target: [StandaloneWindows64, StandaloneLinux64]
        exclude:
          - os: windows-latest
            target: StandaloneLinux64
          - os: ubuntu-latest
            target: StandaloneWindows64
            
    steps:
    - name: Checkout
      uses: actions/checkout@v3
      with:
        lfs: true
        
    - name: Setup Unity
      uses: game-ci/unity-test-runner@v2
      with:
        unityVersion: ${{ env.UNITY_VERSION }}
        
    - name: Build
      uses: game-ci/unity-builder@v2
      with:
        targetPlatform: ${{ matrix.target }}
        unityVersion: ${{ env.UNITY_VERSION }}
        
    - name: Package Build
      run: |
        mkdir -p package
        cp -r build/${{ matrix.target }}/* package/
        tar -czf snake-prototype-${{ matrix.target }}-${{ github.sha }}.tar.gz package/
        
    - name: Upload Build
      uses: actions/upload-artifact@v3
      with:
        name: build-${{ matrix.target }}
        path: snake-prototype-${{ matrix.target }}-${{ github.sha }}.tar.gz
        
  deploy:
    name: Deploy Release
    needs: build
    runs-on: ubuntu-latest
    if: github.event_name == 'release'
    
    steps:
    - name: Download Artifacts
      uses: actions/download-artifact@v3
      with:
        path: artifacts
        
    - name: Create Release Assets
      run: |
        for target in StandaloneWindows64 StandaloneLinux64; do
          cd artifacts/build-$target
          tar -xzf snake-prototype-$target-${{ github.sha }}.tar.gz
          zip -r ../snake-prototype-$target.zip *
          cd ..
        done
        
    - name: Upload Release Assets
      uses: softprops/action-gh-release@v1
      with:
        files: |
          artifacts/build-StandaloneWindows64/snake-prototype-StandaloneWindows64.zip
          artifacts/build-StandaloneLinux64/snake-prototype-StandaloneLinux64.zip
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

### Quality Gates

#### Automated Quality Checks

```yaml
# .github/workflows/quality-gates.yml
name: Quality Gates

on: [push, pull_request]

jobs:
  quality-checks:
    runs-on: ubuntu-latest
    
    steps:
    - name: Checkout
      uses: actions/checkout@v3
      
    - name: Code Analysis
      run: |
        # Run static code analysis
        dotnet tool install --global dotnet-sonarscanner
        dotnet sonarscanner begin /k:"snake-prototype" /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
        dotnet build
        dotnet sonarscanner end /d:sonar.login="${{ secrets.SONAR_TOKEN }}"
        
    - name: Security Scan
      uses: securecodewarrior/github-action-add-sarif@v1
      with:
        sarif-file: 'security-scan-results.sarif'
        
    - name: Performance Tests
      run: |
        # Run performance benchmarks
        python scripts/run_performance_tests.py
        
    - name: Documentation Check
      run: |
        # Check documentation coverage
        python scripts/check_documentation.py
        
    - name: License Check
      run: |
        # Check license compliance
        python scripts/check_licenses.py
```

---

## 🧪 Testing Automation

### Test Framework Setup

#### Test Configuration

```csharp
// Assets/Tests/TestConfig.cs
using UnityEngine;
using UnityEngine.TestRunner;

public class TestConfig
{
    [RuntimeInitializeOnLoadMethod]
    public static void Setup()
    {
        // Configure test environment
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        
        // Initialize test services
        InitializeTestServices();
    }
    
    private static void InitializeTestServices()
    {
        // Create mock services for testing
        var mockInputManager = new MockInputManager();
        var mockGridManager = new MockGridManager();
        var mockSnakeManager = new MockSnakeManager();
        
        ServiceLocator.Register(mockInputManager);
        ServiceLocator.Register(mockGridManager);
        ServiceLocator.Register(mockSnakeManager);
    }
}
```

#### Automated Test Runner

```python
# scripts/run_tests.py
import subprocess
import sys
import os
import json
from pathlib import Path

class TestRunner:
    def __init__(self, unity_path, project_path):
        self.unity_path = unity_path
        self.project_path = project_path
        self.results = {}
        
    def run_edit_mode_tests(self):
        """Run Edit Mode tests"""
        print("Running Edit Mode tests...")
        
        cmd = [
            self.unity_path,
            "-batchmode",
            "-quit",
            "-projectPath", self.project_path,
            "-runTests",
            "-platform", "EditMode",
            "-testResults", "editmode-results.xml",
            "-logFile", "editmode.log"
        ]
        
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            print("Edit Mode tests passed")
            self.results['editmode'] = self.parse_test_results("editmode-results.xml")
        else:
            print(f"Edit Mode tests failed: {result.stderr}")
            self.results['editmode'] = {'passed': 0, 'failed': 1, 'total': 1}
            
    def run_play_mode_tests(self):
        """Run Play Mode tests"""
        print("Running Play Mode tests...")
        
        cmd = [
            self.unity_path,
            "-batchmode",
            "-quit",
            "-projectPath", self.project_path,
            "-runTests",
            "-platform", "PlayMode",
            "-testResults", "playmode-results.xml",
            "-logFile", "playmode.log"
        ]
        
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            print("Play Mode tests passed")
            self.results['playmode'] = self.parse_test_results("playmode-results.xml")
        else:
            print(f"Play Mode tests failed: {result.stderr}")
            self.results['playmode'] = {'passed': 0, 'failed': 1, 'total': 1}
            
    def parse_test_results(self, xml_file):
        """Parse Unity test results XML"""
        # Implementation for parsing test results
        return {'passed': 10, 'failed': 0, 'total': 10}
        
    def generate_report(self):
        """Generate test report"""
        report = {
            'timestamp': datetime.now().isoformat(),
            'results': self.results,
            'summary': self.calculate_summary()
        }
        
        with open('test-report.json', 'w') as f:
            json.dump(report, f, indent=2)
            
        return report
        
    def calculate_summary(self):
        """Calculate test summary"""
        total_passed = sum(result['passed'] for result in self.results.values())
        total_failed = sum(result['failed'] for result in self.results.values())
        total_tests = sum(result['total'] for result in self.results.values())
        
        return {
            'total_passed': total_passed,
            'total_failed': total_failed,
            'total_tests': total_tests,
            'success_rate': (total_passed / total_tests) * 100 if total_tests > 0 else 0
        }

if __name__ == "__main__":
    unity_path = "/path/to/unity"
    project_path = "/path/to/project"
    
    runner = TestRunner(unity_path, project_path)
    runner.run_edit_mode_tests()
    runner.run_play_mode_tests()
    report = runner.generate_report()
    
    print(f"Test Summary: {report['summary']['success_rate']:.1f}% pass rate")
```

### Performance Testing

#### Performance Benchmark Suite

```csharp
// Assets/Tests/Performance/PerformanceTests.cs
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Diagnostics;

public class PerformanceTests
{
    [Test, Performance]
    public void SnakeManager_PerformanceTest()
    {
        Measure.Method(() =>
        {
            // Simulate snake movement
            var snakeManager = ServiceLocator.Get<SnakeManager>();
            snakeManager.MoveSnake(Vector2Int.right);
        })
        .WarmupCount(10)
        .MeasurementCount(100)
        .IterationsPerMeasurement(1000)
        .GC()
        .Run();
    }
    
    [Test, Performance]
    public void DetectionManager_PerformanceTest()
    {
        Measure.Method(() =>
        {
            // Simulate detection calculations
            var detectionManager = ServiceLocator.Get<DetectionManager>();
            detectionManager.UpdateDetection(0.016f); // 60 FPS
        })
        .WarmupCount(10)
        .MeasurementCount(100)
        .IterationsPerMeasurement(100)
        .GC()
        .Run();
    }
    
    [UnityTest, Performance]
    public IEnumerator GridManager_LargeGridPerformance()
    {
        var gridManager = ServiceLocator.Get<GridManager>();
        
        yield return Measure.Frames()
            .WarmupCount(10)
            .MeasurementCount(100)
            .DontRecordFrametime()
            .Run(() =>
            {
                // Simulate large grid operations
                for (int x = 0; x < 100; x++)
                {
                    for (int y = 0; y < 100; y++)
                    {
                        gridManager.IsWall(new Vector2Int(x, y));
                    }
                }
            });
    }
}
```

---

## 🚀 Deployment Procedures

### Automated Deployment

#### Deployment Script

```bash
#!/bin/bash
# scripts/deploy.sh

set -e

# Configuration
BUILD_DIR="build"
DEPLOY_DIR="deploy"
VERSION=$1
PLATFORM=$2

if [ -z "$VERSION" ] || [ -z "$PLATFORM" ]; then
    echo "Usage: ./deploy.sh <version> <platform>"
    echo "Platforms: windows, linux"
    exit 1
fi

echo "Deploying version $VERSION for platform $PLATFORM"

# Create deployment directory
mkdir -p "$DEPLOY_DIR/$VERSION"

# Copy build artifacts
case $PLATFORM in
    "windows")
        cp -r "$BUILD_DIR/StandaloneWindows64" "$DEPLOY_DIR/$VERSION/windows"
        ;;
    "linux")
        cp -r "$BUILD_DIR/StandaloneLinux64" "$DEPLOY_DIR/$VERSION/linux"
        ;;
    *)
        echo "Unknown platform: $PLATFORM"
        exit 1
        ;;
esac

# Create deployment package
cd "$DEPLOY_DIR/$VERSION"
tar -czf "snake-prototype-$VERSION-$PLATFORM.tar.gz" "$PLATFORM"

# Generate checksums
sha256sum "snake-prototype-$VERSION-$PLATFORM.tar.gz" > "snake-prototype-$VERSION-$PLATFORM.sha256"

echo "Deployment completed: snake-prototype-$VERSION-$PLATFORM.tar.gz"
```

#### Release Automation

```python
# scripts/release_automation.py
import os
import subprocess
import json
import requests
from datetime import datetime

class ReleaseAutomation:
    def __init__(self, github_token, repo_owner, repo_name):
        self.github_token = github_token
        self.repo_owner = repo_owner
        self.repo_name = repo_name
        self.api_base = f"https://api.github.com/repos/{repo_owner}/{repo_name}"
        
    def create_release(self, version, description, artifacts):
        """Create GitHub release"""
        release_data = {
            "tag_name": version,
            "name": f"Snake Prototype {version}",
            "body": description,
            "draft": False,
            "prerelease": version.endswith("-beta")
        }
        
        headers = {
            "Authorization": f"token {self.github_token}",
            "Accept": "application/vnd.github.v3+json"
        }
        
        response = requests.post(
            f"{self.api_base}/releases",
            json=release_data,
            headers=headers
        )
        
        if response.status_code == 201:
            release = response.json()
            release_id = release["id"]
            print(f"Created release {version} with ID {release_id}")
            
            # Upload artifacts
            for artifact in artifacts:
                self.upload_release_asset(release_id, artifact)
                
            return release
        else:
            print(f"Failed to create release: {response.text}")
            return None
            
    def upload_release_asset(self, release_id, artifact_path):
        """Upload release asset"""
        with open(artifact_path, 'rb') as f:
            files = {
                'file': (os.path.basename(artifact_path), f, 'application/octet-stream')
            }
            
            headers = {
                "Authorization": f"token {self.github_token}",
                "Content-Type": "application/octet-stream"
            }
            
            response = requests.post(
                f"{self.api_base}/releases/{release_id}/assets",
                files=files,
                headers=headers
            )
            
            if response.status_code == 201:
                print(f"Uploaded asset: {artifact_path}")
            else:
                print(f"Failed to upload asset {artifact_path}: {response.text}")
                
    def deploy_to_stores(self, version, artifacts):
        """Deploy to various stores"""
        # Steam deployment
        if "windows" in artifacts:
            self.deploy_to_steam(version, artifacts["windows"])
            
        # Itch.io deployment
        self.deploy_to_itch(version, artifacts)
        
    def deploy_to_steam(self, version, artifact_path):
        """Deploy to Steam"""
        # Implementation for Steam deployment
        print(f"Deploying {version} to Steam...")
        
    def deploy_to_itch(self, version, artifacts):
        """Deploy to Itch.io"""
        # Implementation for Itch.io deployment
        print(f"Deploying {version} to Itch.io...")

if __name__ == "__main__":
    # Configuration
    github_token = os.environ.get("GITHUB_TOKEN")
    repo_owner = "your-username"
    repo_name = "snake-prototype"
    
    # Create release automation
    automation = ReleaseAutomation(github_token, repo_owner, repo_name)
    
    # Get version and artifacts
    version = "v0.5.0"
    description = "Release notes for v0.5.0"
    artifacts = [
        "build/snake-prototype-windows-v0.5.0.zip",
        "build/snake-prototype-linux-v0.5.0.zip"
    ]
    
    # Create release
    release = automation.create_release(version, description, artifacts)
    
    if release:
        print("Release created successfully!")
        automation.deploy_to_stores(version, artifacts)
    else:
        print("Failed to create release")
```

---

## 📊 Monitoring and Logging

### Application Monitoring

#### Performance Monitoring

```csharp
// Assets/Scripts/Monitoring/PerformanceMonitor.cs
using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;

public class PerformanceMonitor : MonoBehaviour
{
    private PerformanceCounter _fpsCounter;
    private PerformanceCounter _memoryCounter;
    private PerformanceCounter _cpuCounter;
    
    private void Start()
    {
        InitializeCounters();
        StartCoroutine(MonitorPerformance());
    }
    
    private void InitializeCounters()
    {
        _fpsCounter = new PerformanceCounter("FPS");
        _memoryCounter = new PerformanceCounter("Memory");
        _cpuCounter = new PerformanceCounter("CPU");
    }
    
    private IEnumerator MonitorPerformance()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            
            var metrics = new PerformanceMetrics
            {
                FPS = 1.0f / Time.unscaledDeltaTime,
                MemoryUsage = System.GC.GetTotalMemory(false),
                Timestamp = System.DateTime.UtcNow
            };
            
            // Send metrics to monitoring service
            SendMetrics(metrics);
        }
    }
    
    private void SendMetrics(PerformanceMetrics metrics)
    {
        // Implementation for sending metrics to monitoring service
        string json = JsonUtility.ToJson(metrics);
        // Send to analytics service
    }
}

[System.Serializable]
public class PerformanceMetrics
{
    public float FPS;
    public long MemoryUsage;
    public System.DateTime Timestamp;
}
```

#### Error Tracking

```csharp
// Assets/Scripts/Monitoring/ErrorTracker.cs
using UnityEngine;
using System;

public class ErrorTracker : MonoBehaviour
{
    private void Start()
    {
        Application.logMessageReceived += HandleLog;
    }
    
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error)
        {
            var errorData = new ErrorReport
            {
                Message = logString,
                StackTrace = stackTrace,
                Type = type.ToString(),
                Timestamp = DateTime.UtcNow,
                Version = Application.version,
                Platform = Application.platform.ToString()
            };
            
            ReportError(errorData);
        }
    }
    
    private void ReportError(ErrorReport error)
    {
        // Send error report to monitoring service
        string json = JsonUtility.ToJson(error);
        // Send to error tracking service
    }
}

[System.Serializable]
public class ErrorReport
{
    public string Message;
    public string StackTrace;
    public string Type;
    public DateTime Timestamp;
    public string Version;
    public string Platform;
}
```

### Logging Infrastructure

#### Structured Logging

```csharp
// Assets/Scripts/Logging/StructuredLogger.cs
using UnityEngine;
using System;
using System.Collections.Generic;

public class StructuredLogger
{
    private static StructuredLogger _instance;
    public static StructuredLogger Instance => _instance ??= new StructuredLogger();
    
    private List<LogEntry> _logEntries = new List<LogEntry>();
    
    public void LogInfo(string message, Dictionary<string, object> context = null)
    {
        Log(LogLevel.Info, message, context);
    }
    
    public void LogWarning(string message, Dictionary<string, object> context = null)
    {
        Log(LogLevel.Warning, message, context);
    }
    
    public void LogError(string message, Dictionary<string, object> context = null)
    {
        Log(LogLevel.Error, message, context);
    }
    
    private void Log(LogLevel level, string message, Dictionary<string, object> context)
    {
        var entry = new LogEntry
        {
            Level = level,
            Message = message,
            Context = context ?? new Dictionary<string, object>(),
            Timestamp = DateTime.UtcNow,
            FrameCount = Time.frameCount
        };
        
        _logEntries.Add(entry);
        
        // Send to logging service
        SendToLoggingService(entry);
    }
    
    private void SendToLoggingService(LogEntry entry)
    {
        // Implementation for sending logs to service
        string json = JsonUtility.ToJson(entry);
        // Send to logging service
    }
}

public enum LogLevel
{
    Info,
    Warning,
    Error
}

[System.Serializable]
public class LogEntry
{
    public LogLevel Level;
    public string Message;
    public Dictionary<string, object> Context;
    public DateTime Timestamp;
    public int FrameCount;
}
```

---

## 🔧 Infrastructure Management

### Docker Configuration

#### Development Environment

```dockerfile
# Dockerfile.dev
FROM unityci/editor:6000.3.8f1-linux-il2cpp

# Install additional tools
RUN apt-get update && apt-get install -y \
    python3 \
    python3-pip \
    git \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Install Python dependencies
COPY requirements.txt /tmp/
RUN pip3 install -r /tmp/requirements.txt

# Set working directory
WORKDIR /project

# Copy project files
COPY . .

# Expose ports for services
EXPOSE 8080

# Run tests
CMD ["python3", "scripts/run_tests.py"]
```

#### Production Build Environment

```dockerfile
# Dockerfile.build
FROM unityci/editor:6000.3.8f1-linux-il2cpp

WORKDIR /build

# Copy only necessary files for build
COPY Assets/ /build/Assets/
COPY Packages/ /build/Packages/
COPY ProjectSettings/ /build/ProjectSettings/
COPY Assets/Editor/BuildScript.cs /build/Assets/Editor/

# Build the project
RUN unity-editor -batchmode -quit -projectPath /build -executeMethod BuildScript.BuildAll

# Copy build artifacts
COPY --from=build /build/Build/ /output/
```

### Kubernetes Configuration

#### Deployment Configuration

```yaml
# k8s/deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: snake-prototype-api
  labels:
    app: snake-prototype
spec:
  replicas: 3
  selector:
    matchLabels:
      app: snake-prototype
  template:
    metadata:
      labels:
        app: snake-prototype
    spec:
      containers:
      - name: api
        image: snake-prototype:latest
        ports:
        - containerPort: 8080
        env:
        - name: DATABASE_URL
          valueFrom:
            secretKeyRef:
              name: snake-prototype-secrets
              key: database-url
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /ready
            port: 8080
          initialDelaySeconds: 5
          periodSeconds: 5
```

---

## 📈 Analytics and Metrics

### Game Analytics

#### Player Behavior Tracking

```csharp
// Assets/Scripts/Analytics/GameAnalytics.cs
using UnityEngine;
using System.Collections.Generic;

public class GameAnalytics : MonoBehaviour
{
    private static GameAnalytics _instance;
    public static GameAnalytics Instance => _instance ??= FindObjectOfType<GameAnalytics>();
    
    private void Start()
    {
        TrackSessionStart();
    }
    
    public void TrackEvent(string eventName, Dictionary<string, object> properties = null)
    {
        var eventData = new AnalyticsEvent
        {
            EventName = eventName,
            Properties = properties ?? new Dictionary<string, object>(),
            Timestamp = System.DateTime.UtcNow,
            SessionId = GetSessionId(),
            PlayerId = GetPlayerId()
        };
        
        SendEvent(eventData);
    }
    
    public void TrackLevelStart(int levelNumber)
    {
        TrackEvent("level_start", new Dictionary<string, object>
        {
            ["level_number"] = levelNumber,
            ["difficulty"] = GetCurrentDifficulty()
        });
    }
    
    public void TrackLevelComplete(int levelNumber, float time, int score)
    {
        TrackEvent("level_complete", new Dictionary<string, object>
        {
            ["level_number"] = levelNumber,
            ["completion_time"] = time,
            ["score"] = score,
            ["difficulty"] = GetCurrentDifficulty()
        });
    }
    
    public void TrackPlayerDeath(int levelNumber, Vector2Int position)
    {
        TrackEvent("player_death", new Dictionary<string, object>
        {
            ["level_number"] = levelNumber,
            ["death_position"] = position.ToString(),
            ["snake_length"] = GetSnakeLength(),
            ["time_alive"] = Time.time
        });
    }
    
    private void SendEvent(AnalyticsEvent eventData)
    {
        // Send to analytics service
        string json = JsonUtility.ToJson(eventData);
        // Send to analytics backend
    }
    
    private string GetSessionId()
    {
        // Generate or retrieve session ID
        return System.Guid.NewGuid().ToString();
    }
    
    private string GetPlayerId()
    {
        // Retrieve or generate player ID
        return PlayerPrefs.GetString("PlayerId", System.Guid.NewGuid().ToString());
    }
    
    private void TrackSessionStart()
    {
        TrackEvent("session_start", new Dictionary<string, object>
        {
            ["platform"] = Application.platform.ToString(),
            ["version"] = Application.version,
            ["device_model"] = SystemInfo.deviceModel
        });
    }
}

[System.Serializable]
public class AnalyticsEvent
{
    public string EventName;
    public Dictionary<string, object> Properties;
    public System.DateTime Timestamp;
    public string SessionId;
    public string PlayerId;
}
```

---

## 🔄 Continuous Improvement

### Monitoring Dashboard

#### Grafana Dashboard Configuration

```json
{
  "dashboard": {
    "title": "Snake Prototype Performance",
    "panels": [
      {
        "title": "FPS Performance",
        "type": "graph",
        "targets": [
          {
            "expr": "avg(snake_prototype_fps)",
            "legendFormat": "Average FPS"
          }
        ]
      },
      {
        "title": "Memory Usage",
        "type": "graph",
        "targets": [
          {
            "expr": "avg(snake_prototype_memory_bytes)",
            "legendFormat": "Memory Usage (bytes)"
          }
        ]
      },
      {
        "title": "Error Rate",
        "type": "stat",
        "targets": [
          {
            "expr": "rate(snake_prototype_errors_total[5m])",
            "legendFormat": "Errors per second"
          }
        ]
      }
    ]
  }
}
```

### Alert Configuration

#### Prometheus Alert Rules

```yaml
# alerts.yml
groups:
  - name: snake-prototype
    rules:
      - alert: HighErrorRate
        expr: rate(snake_prototype_errors_total[5m]) > 0.1
        for: 2m
        labels:
          severity: warning
        annotations:
          summary: "High error rate detected"
          description: "Error rate is {{ $value }} errors per second"
          
      - alert: LowFPS
        expr: avg(snake_prototype_fps) < 30
        for: 5m
        labels:
          severity: warning
        annotations:
          summary: "Low FPS detected"
          description: "Average FPS is {{ $value }}"
          
      - alert: HighMemoryUsage
        expr: avg(snake_prototype_memory_bytes) > 500000000
        for: 5m
        labels:
          severity: critical
        annotations:
          summary: "High memory usage detected"
          description: "Memory usage is {{ $value }} bytes"
```

---

## 📋 Maintenance Procedures

### Regular Maintenance Tasks

#### Daily Tasks
- [ ] Check build pipeline status
- [ ] Review error logs
- [ ] Monitor performance metrics
- [ ] Check disk space usage

#### Weekly Tasks
- [ ] Review test coverage
- [ ] Update dependencies
- [ ] Check security advisories
- [ ] Performance optimization review

#### Monthly Tasks
- [ ] Full system backup
- [ ] Security audit
- [ ] Performance benchmarking
- [ ] Documentation updates

### Backup Procedures

#### Database Backup Script

```bash
#!/bin/bash
# scripts/backup_database.sh

BACKUP_DIR="/backups"
DATE=$(date +%Y%m%d_%H%M%S)
DB_NAME="snake_prototype"

# Create backup
mysqldump -u backup_user -p$BACKUP_PASSWORD $DB_NAME > "$BACKUP_DIR/snake_prototype_$DATE.sql"

# Compress backup
gzip "$BACKUP_DIR/snake_prototype_$DATE.sql"

# Remove old backups (keep last 30 days)
find $BACKUP_DIR -name "snake_prototype_*.sql.gz" -mtime +30 -delete

echo "Backup completed: snake_prototype_$DATE.sql.gz"
```

---

**Last Updated**: 2026-02-20  
**Review Date**: 2026-04-01  
**DevOps Team**: devops@snake-prototype.com  
**Version**: 1.0
