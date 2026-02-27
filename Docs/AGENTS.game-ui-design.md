# Game UI Design Agent - Specialized Documentation

## Overview

The `game-ui-design` agent provides expertise in user interface design, user experience optimization, and UI Toolkit implementation for the Snake Prototype project, combining Nintendo's UI clarity philosophy with modern responsive design principles.

## Core Responsibilities

### UI/UX Design Principles
- Nintendo-inspired clarity and simplicity
- Diegetic interface design principles
- Competitive gaming UI readability
- Accessibility and inclusivity considerations

### UI Toolkit Implementation
- Unity UI Toolkit best practices
- Responsive design for multiple platforms
- Performance optimization for UI rendering
- Visual hierarchy and information architecture

### User Experience Optimization
- Flow state preservation through UI
- Minimal cognitive load design
- Intuitive navigation and interaction
- Cross-platform UI adaptation

## Design Philosophy

### Nintendo UI Principles

#### Clarity and Simplicity
```csharp
// Nintendo-inspired UI design principles
public class NintendoUIDesigner
{
    public struct UIPrinciples
    {
        public float ClarityScore;      // How clear the UI is
        public float SimplicityScore;  // How simple the interface is
        public float IntuitivenessScore; // How intuitive the navigation is
        public float AccessibilityScore; // How accessible the UI is
    }
    
    public UIPrinciples AnalyzeUIDesign(UIElement rootElement)
    {
        return new UIPrinciples
        {
            ClarityScore = CalculateClarityScore(rootElement),
            SimplicityScore = CalculateSimplicityScore(rootElement),
            IntuitivenessScore = CalculateIntuitivenessScore(rootElement),
            AccessibilityScore = CalculateAccessibilityScore(rootElement)
        };
    }
    
    private float CalculateClarityScore(UIElement element)
    {
        float score = 0f;
        
        // Visual hierarchy
        score += EvaluateVisualHierarchy(element) * 0.3f;
        
        // Text readability
        score += EvaluateTextReadability(element) * 0.3f;
        
        // Icon clarity
        score += EvaluateIconClarity(element) * 0.2f;
        
        // Color contrast
        score += EvaluateColorContrast(element) * 0.2f;
        
        return Mathf.Clamp01(score);
    }
    
    private float EvaluateVisualHierarchy(UIElement element)
    {
        // Check for clear visual hierarchy
        var hierarchyScore = 0f;
        var childCount = element.childCount;
        
        for (int i = 0; i < childCount; i++)
        {
            var child = element.ElementAt(i);
            
            // Check size hierarchy
            if (i == 0) // Primary element
            {
                hierarchyScore += child.style.fontSize.value.value > 16f ? 1f : 0.5f;
            }
            else // Secondary elements
            {
                hierarchyScore += child.style.fontSize.value.value <= 16f ? 1f : 0.5f;
            }
        }
        
        return hierarchyScore / childCount;
    }
    
    private float EvaluateTextReadability(UIElement element)
    {
        float readabilityScore = 0f;
        var textElements = element.Query<TextElement>().ToList();
        
        foreach (var textElement in textElements)
        {
            var fontSize = textElement.style.fontSize.value.value;
            var color = textElement.style.color.value;
            
            // Font size should be at least 12pt for readability
            if (fontSize >= 12f)
                readabilityScore += 0.5f;
            else
                readabilityScore += 0.2f;
                
            // Color contrast should meet WCAG standards
            if (ColorContrast.MeetsWCAG(color))
                readabilityScore += 0.5f;
            else
                readabilityScore += 0.2f;
        }
        
        return textElements.Count > 0 ? readabilityScore / textElements.Count : 1f;
    }
}
```

#### Minimalist Design Approach
```csharp
// Minimalist UI design implementation
public class MinimalistUIDesigner
{
    public struct MinimalistMetrics
    {
        public int ElementCount;        // Total UI elements
        public float VisualDensity;     // Visual density score
        public float InformationRatio;  // Information vs decoration ratio
        public float WhiteSpaceRatio;   // White space utilization
    }
    
    public MinimalistMetrics AnalyzeMinimalism(UIElement rootElement)
    {
        var allElements = GetAllUIElements(rootElement);
        
        return new MinimalistMetrics
        {
            ElementCount = allElements.Count,
            VisualDensity = CalculateVisualDensity(allElements),
            InformationRatio = CalculateInformationRatio(allElements),
            WhiteSpaceRatio = CalculateWhiteSpaceRatio(rootElement)
        };
    }
    
    private float CalculateVisualDensity(List<UIElement> elements)
    {
        float totalArea = 0f;
        float occupiedArea = 0f;
        
        foreach (var element in elements)
        {
            var rect = element.worldBound;
            totalArea += rect.width * rect.height;
            
            // Only count visible elements
            if (element.style.display.value != DisplayStyle.None)
            {
                occupiedArea += rect.width * rect.height;
            }
        }
        
        return totalArea > 0 ? occupiedArea / totalArea : 0f;
    }
    
    private float CalculateInformationRatio(List<UIElement> elements)
    {
        int informationalElements = 0;
        int decorativeElements = 0;
        
        foreach (var element in elements)
        {
            if (IsInformationalElement(element))
                informationalElements++;
            else if (IsDecorativeElement(element))
                decorativeElements++;
        }
        
        int totalElements = informationalElements + decorativeElements;
        return totalElements > 0 ? (float)informationalElements / totalElements : 1f;
    }
    
    private bool IsInformationalElement(UIElement element)
    {
        return element is TextElement || 
               element.name.Contains("Score") || 
               element.name.Contains("Health") || 
               element.name.Contains("Timer") ||
               element.name.Contains("Button");
    }
    
    private bool IsDecorativeElement(UIElement element)
    {
        return element.name.Contains("Decoration") || 
               element.name.Contains("Background") ||
               element.name.Contains("Border");
    }
}
```

## Diegetic Interface Design

### In-World UI Integration

#### Diegetic HUD Elements
```csharp
// Diegetic interface design for Snake Prototype
public class DiegeticInterfaceDesigner
{
    public struct DiegeticElement
    {
        public string Name;
        public Vector3 WorldPosition;
        public Quaternion WorldRotation;
        public float InteractionDistance;
        public bool IsAlwaysVisible;
        public DiegeticType Type;
    }
    
    public enum DiegeticType
    {
        WorldSpace,      // UI exists in 3D world space
        Surface,         // UI projected onto surfaces
        Object,          // UI attached to objects
        Environmental    // UI integrated into environment
    }
    
    public DiegeticElement[] CreateDiegeticElements()
    {
        return new DiegeticElement[]
        {
            new DiegeticElement
            {
                Name = "Health Display",
                WorldPosition = Vector3.forward * 2f,
                WorldRotation = Quaternion.LookRotation(Vector3.back),
                InteractionDistance = 5f,
                IsAlwaysVisible = true,
                Type = DiegeticType.WorldSpace
            },
            
            new DiegeticElement
            {
                Name = "Score Counter",
                WorldPosition = Vector3.up * 3f,
                WorldRotation = Quaternion.LookRotation(Vector3.down),
                InteractionDistance = 10f,
                IsAlwaysVisible = true,
                Type = DiegeticType.WorldSpace
            },
            
            new DiegeticElement
            {
                Name = "Detection Indicator",
                WorldPosition = Vector3.zero,
                WorldRotation = Quaternion.identity,
                InteractionDistance = 0f,
                IsAlwaysVisible = false,
                Type = DiegeticType.Object
            }
        };
    }
    
    public void UpdateDiegeticElements(Camera playerCamera, DiegeticElement[] elements)
    {
        foreach (var element in elements)
        {
            UpdateDiegeticElement(playerCamera, element);
        }
    }
    
    private void UpdateDiegeticElement(Camera camera, DiegeticElement element)
    {
        // Calculate distance to player
        float distance = Vector3.Distance(camera.transform.position, element.WorldPosition);
        
        // Determine visibility based on distance and type
        bool shouldBeVisible = false;
        
        switch (element.Type)
        {
            case DiegeticType.WorldSpace:
                shouldBeVisible = element.IsAlwaysVisible || distance <= element.InteractionDistance;
                break;
                
            case DiegeticType.Object:
                shouldBeVisible = distance <= element.InteractionDistance;
                break;
                
            case DiegeticType.Surface:
                shouldBeVisible = distance <= element.InteractionDistance && IsInCameraView(camera, element.WorldPosition);
                break;
        }
        
        // Update element visibility
        SetDiegeticElementVisibility(element, shouldBeVisible);
        
        // Update element orientation for readability
        if (shouldBeVisible && element.Type == DiegeticType.WorldSpace)
        {
            OrientElementToCamera(element, camera);
        }
    }
    
    private void OrientElementToCamera(DiegeticElement element, Camera camera)
    {
        // Make the element face the camera for readability
        Vector3 lookDirection = camera.transform.position - element.WorldPosition;
        lookDirection.y = 0; // Keep upright
        
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        
        // Smooth rotation
        SetDiegeticElementRotation(element, Quaternion.Slerp(
            element.WorldRotation, 
            targetRotation, 
            Time.deltaTime * 5f
        ));
    }
}
```

#### Environmental UI Integration
```csharp
// Environmental UI integration for industrial theme
public class EnvironmentalUIDesigner
{
    public struct EnvironmentalUI
    {
        public string Name;
        public GameObject EnvironmentObject;
        public UIDocument UIDocument;
        public Vector3 LocalOffset;
        public Material ProjectionMaterial;
        public bool IsProjected;
    }
    
    public void CreateEnvironmentalUI()
    {
        // Create UI elements that integrate with the industrial environment
        
        // Project UI onto walls and surfaces
        CreateProjectedUI();
        
        // Integrate UI into machinery and equipment
        CreateMachineryUI();
        
        // Add environmental indicators
        CreateEnvironmentalIndicators();
    }
    
    private void CreateProjectedUI()
    {
        // Project score and detection onto nearby walls
        var wallProjector = new GameObject("WallProjector");
        var projector = wallProjector.AddComponent<Projector>();
        
        // Configure projector for industrial look
        projector.material = CreateIndustrialProjectorMaterial();
        projector.orthographic = true;
        projector.orthographicSize = 2f;
        projector.nearClipPlane = 0.1f;
        projector.farClipPlane = 10f;
        
        // Project UI elements
        projector.material.SetTexture("_MainTex", CreateUITexture());
    }
    
    private Material CreateIndustrialProjectorMaterial()
    {
        var material = new Material(Shader.Find("Projector/Multiply"));
        
        // Industrial green color scheme
        material.SetColor("_Color", new Color(0.2f, 0.8f, 0.3f, 0.8f));
        
        // Add scanline effect for industrial feel
        material.EnableKeyword("_SCANLINE_ON");
        
        return material;
    }
    
    private void CreateMachineryUI()
    {
        // Integrate UI into industrial machinery
        var machineryObjects = GameObject.FindGameObjectsWithTag("Machinery");
        
        foreach (var machinery in machineryObjects)
        {
            var uiPanel = CreateMachineryUIPanel(machinery);
            uiPanel.transform.SetParent(machinery.transform, false);
            uiPanel.transform.localPosition = Vector3.up * 1.5f;
        }
    }
}
```

## Competitive Gaming UI

### Readability and Performance

#### High-Performance HUD Design
```csharp
// Competitive gaming UI optimization
public class CompetitiveUIDesigner
{
    public struct CompetitiveMetrics
    {
        public float ReadabilityScore;    // How readable the UI is during gameplay
        public float DistractionScore;    // How much UI distracts from gameplay
        public float PerformanceScore;    // UI rendering performance
        public float InformationDensity;  // Information per screen area
    }
    
    public CompetitiveMetrics AnalyzeCompetitiveUI(UIElement rootElement)
    {
        return new CompetitiveMetrics
        {
            ReadabilityScore = CalculateReadabilityScore(rootElement),
            DistractionScore = CalculateDistractionScore(rootElement),
            PerformanceScore = CalculatePerformanceScore(rootElement),
            InformationDensity = CalculateInformationDensity(rootElement)
        };
    }
    
    private float CalculateReadabilityScore(UIElement element)
    {
        float score = 0f;
        
        // Text size and contrast
        score += EvaluateTextReadabilityForCompetitive(element) * 0.4f;
        
        // Icon clarity and recognition
        score += EvaluateIconRecognition(element) * 0.3f;
        
        // Color coding consistency
        score += EvaluateColorConsistency(element) * 0.3f;
        
        return Mathf.Clamp01(score);
    }
    
    private float EvaluateTextReadabilityForCompetitive(UIElement element)
    {
        float readabilityScore = 0f;
        var textElements = element.Query<TextElement>().ToList();
        
        foreach (var textElement in textElements)
        {
            // Competitive games need larger text for quick reading
            var fontSize = textElement.style.fontSize.value.value;
            if (fontSize >= 14f)
                readabilityScore += 1f;
            else if (fontSize >= 12f)
                readabilityScore += 0.7f;
            else
                readabilityScore += 0.3f;
                
            // High contrast is essential
            var color = textElement.style.color.value;
            if (ColorContrast.IsHighContrast(color))
                readabilityScore += 1f;
            else
                readabilityScore += 0.5f;
        }
        
        return textElements.Count > 0 ? readabilityScore / (textElements.Count * 2) : 1f;
    }
    
    private float CalculateDistractionScore(UIElement element)
    {
        float distractionScore = 0f;
        
        // Check for distracting animations
        distractionScore += EvaluateAnimationDistraction(element) * 0.3f;
        
        // Check for visual noise
        distractionScore += EvaluateVisualNoise(element) * 0.3f;
        
        // Check for screen coverage
        distractionScore += EvaluateScreenCoverage(element) * 0.4f;
        
        return Mathf.Clamp01(distractionScore);
    }
    
    private float EvaluateAnimationDistraction(UIElement element)
    {
        // Competitive games should minimize distracting animations
        var animatedElements = element.Query().Where(e => e.style.transitionProperty.value.value != "").ToList();
        
        float distractionScore = 1f; // Start with perfect score
        
        foreach (var animatedElement in animatedElements)
        {
            var transitionDuration = animatedElement.style.transitionDuration.value.value;
            
            // Long animations are distracting
            if (transitionDuration > 0.5f)
                distractionScore -= 0.3f;
            else if (transitionDuration > 0.2f)
                distractionScore -= 0.1f;
        }
        
        return Mathf.Clamp01(distractionScore);
    }
}
```

#### Real-Time Information Display
```csharp
// Real-time competitive information display
public class RealTimeInfoDisplay : MonoBehaviour
{
    [SerializeField] private UIDocument _hudDocument;
    [SerializeField] private float _updateRate = 60f; // 60 FPS updates
    
    private VisualElement _root;
    private Label _scoreLabel;
    private Label _detectionLabel;
    private ProgressBar _healthBar;
    private float _lastUpdateTime;
    
    private void Start()
    {
        InitializeHUD();
    }
    
    private void InitializeHUD()
    {
        _root = _hudDocument.rootVisualElement;
        
        // Find critical HUD elements
        _scoreLabel = _root.Q<Label>("ScoreLabel");
        _detectionLabel = _root.Q<Label>("DetectionLabel");
        _healthBar = _root.Q<ProgressBar>("HealthBar");
        
        // Optimize for competitive play
        OptimizeHUDForCompetitive();
    }
    
    private void OptimizeHUDForCompetitive()
    {
        // Disable unnecessary transitions for instant updates
        _scoreLabel.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("none") };
        _detectionLabel.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("none") };
        _healthBar.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("none") };
        
        // Set high contrast colors
        _scoreLabel.style.color = new StyleColor(Color.white);
        _detectionLabel.style.color = new StyleColor(Color.red);
        
        // Use large, readable fonts
        _scoreLabel.style.fontSize = new StyleLength(18f);
        _detectionLabel.style.fontSize = new StyleLength(16f);
    }
    
    private void Update()
    {
        // High-frequency updates for competitive play
        if (Time.time - _lastUpdateTime >= 1f / _updateRate)
        {
            UpdateHUD();
            _lastUpdateTime = Time.time;
        }
    }
    
    private void UpdateHUD()
    {
        // Get game state
        var gameState = ServiceLocator.Get<GameStateManager>();
        
        // Update critical information instantly
        _scoreLabel.text = $"Score: {gameState.Score}";
        _detectionLabel.text = $"Detection: {(int)(gameState.DetectionLevel * 100)}%";
        _healthBar.value = gameState.Health / 100f;
        
        // Color-code detection level for quick recognition
        Color detectionColor = gameState.DetectionLevel switch
        {
            < 0.3f => Color.green,
            < 0.7f => Color.yellow,
            _ => Color.red
        };
        _detectionLabel.style.color = new StyleColor(detectionColor);
    }
}
```

## Responsive Design

### Multi-Platform Adaptation

#### Responsive UI Framework
```csharp
// Responsive UI design for multiple platforms
public class ResponsiveUIDesigner
{
    public struct ResponsiveSettings
    {
        public Platform Platform;
        public ScreenSize ScreenSize;
        public float ScaleFactor;
        public bool IsTouchDevice;
        public bool IsHighDPI;
    }
    
    public enum Platform
    {
        Desktop,
        Mobile,
        Console,
        Web
    }
    
    public enum ScreenSize
    {
        Small,   // < 768px
        Medium,  // 768px - 1024px
        Large,   // 1024px - 1920px
        XLarge   // > 1920px
    }
    
    public ResponsiveSettings GetResponsiveSettings()
    {
        var settings = new ResponsiveSettings();
        
        // Determine platform
        settings.Platform = DeterminePlatform();
        
        // Determine screen size
        settings.ScreenSize = DetermineScreenSize();
        
        // Calculate scale factor
        settings.ScaleFactor = CalculateScaleFactor(settings.ScreenSize);
        
        // Check device capabilities
        settings.IsTouchDevice = Input.touchSupported;
        settings.IsHighDPI = Screen.dpi > 150;
        
        return settings;
    }
    
    private Platform DeterminePlatform()
    {
        if (Application.isMobilePlatform)
            return Platform.Mobile;
        else if (Application.isConsolePlatform)
            return Platform.Console;
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
            return Platform.Web;
        else
            return Platform.Desktop;
    }
    
    private ScreenSize DetermineScreenSize()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float minDimension = Mathf.Min(screenWidth, screenHeight);
        
        return minDimension switch
        {
            < 768f => ScreenSize.Small,
            < 1024f => ScreenSize.Medium,
            < 1920f => ScreenSize.Large,
            _ => ScreenSize.XLarge
        };
    }
    
    private float CalculateScaleFactor(ScreenSize screenSize)
    {
        return screenSize switch
        {
            ScreenSize.Small => 0.8f,
            ScreenSize.Medium => 1.0f,
            ScreenSize.Large => 1.2f,
            ScreenSize.XLarge => 1.4f
        };
    }
    
    public void ApplyResponsiveSettings(UIDocument uiDocument, ResponsiveSettings settings)
    {
        var root = uiDocument.rootVisualElement;
        
        // Apply scale factor
        root.style.scale = new Vector3(settings.ScaleFactor, settings.ScaleFactor, 1f);
        
        // Adjust for touch devices
        if (settings.IsTouchDevice)
        {
            AdjustForTouch(root);
        }
        
        // Adjust for high DPI
        if (settings.IsHighDPI)
        {
            AdjustForHighDPI(root);
        }
        
        // Platform-specific adjustments
        switch (settings.Platform)
        {
            case Platform.Mobile:
                AdjustForMobile(root);
                break;
            case Platform.Console:
                AdjustForConsole(root);
                break;
            case Platform.Web:
                AdjustForWeb(root);
                break;
        }
    }
    
    private void AdjustForTouch(VisualElement root)
    {
        // Increase touch target sizes
        var buttons = root.Query<Button>().ToList();
        foreach (var button in buttons)
        {
            var currentSize = button.style.width.value.value;
            button.style.width = new StyleLength(Mathf.Max(currentSize, 44f)); // Minimum 44px touch target
            button.style.height = new StyleLength(Mathf.Max(button.style.height.value.value, 44f));
        }
        
        // Add touch feedback
        foreach (var button in buttons)
        {
            button.RegisterCallback<PointerDownEvent>(OnTouchDown);
            button.RegisterCallback<PointerUpEvent>(OnTouchUp);
        }
    }
    
    private void AdjustForMobile(VisualElement root)
    {
        // Optimize layout for mobile screens
        root.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Column);
        
        // Reduce padding for smaller screens
        root.style.paddingLeft = new StyleLength(8f);
        root.style.paddingRight = new StyleLength(8f);
        root.style.paddingTop = new StyleLength(4f);
        root.style.paddingBottom = new StyleLength(4f);
    }
    
    private void AdjustForConsole(VisualElement root)
    {
        // Optimize for controller input
        var buttons = root.Query<Button>().ToList();
        foreach (var button in buttons)
        {
            // Add focus indicators
            button.focusable = true;
            button.RegisterCallback<FocusEvent>(OnButtonFocused);
            button.RegisterCallback<BlurEvent>(OnButtonUnfocused);
        }
    }
}
```

### Accessibility Implementation

#### WCAG Compliance
```csharp
// Accessibility implementation for Snake Prototype
public class AccessibilityManager : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private AccessibilitySettings _settings;
    
    private VisualElement _root;
    private Dictionary<string, Color> _originalColors;
    
    private void Start()
    {
        _root = _uiDocument.rootVisualElement;
        _originalColors = new Dictionary<string, Color>();
        
        InitializeAccessibility();
    }
    
    private void InitializeAccessibility()
    {
        // Store original colors for colorblind modes
        StoreOriginalColors();
        
        // Apply accessibility settings
        ApplyAccessibilitySettings();
        
        // Set up keyboard navigation
        SetupKeyboardNavigation();
        
        // Add screen reader support
        SetupScreenReaderSupport();
    }
    
    private void StoreOriginalColors()
    {
        var textElements = _root.Query<TextElement>().ToList();
        foreach (var textElement in textElements)
        {
            var color = textElement.style.color.value;
            _originalColors[textElement.name] = color;
        }
    }
    
    public void ApplyAccessibilitySettings()
    {
        // Apply colorblind mode
        if (_settings.ColorblindMode != ColorblindMode.None)
        {
            ApplyColorblindMode(_settings.ColorblindMode);
        }
        
        // Apply high contrast mode
        if (_settings.HighContrastMode)
        {
            ApplyHighContrastMode();
        }
        
        // Apply large text mode
        if (_settings.LargeTextMode)
        {
            ApplyLargeTextMode();
        }
        
        // Apply reduced motion
        if (_settings.ReducedMotion)
        {
            ApplyReducedMotion();
        }
    }
    
    private void ApplyColorblindMode(ColorblindMode mode)
    {
        var textElements = _root.Query<TextElement>().ToList();
        
        foreach (var textElement in textElements)
        {
            var originalColor = _originalColors.GetValueOrDefault(textElement.name, Color.white);
            var adjustedColor = ColorblindHelper.AdjustColor(originalColor, mode);
            textElement.style.color = new StyleColor(adjustedColor);
        }
    }
    
    private void ApplyHighContrastMode()
    {
        var textElements = _root.Query<TextElement>().ToList();
        
        foreach (var textElement in textElements)
        {
            // High contrast: black text on white background or vice versa
            var currentColor = textElement.style.color.value;
            var backgroundColor = GetBackgroundColor(textElement);
            
            if (ColorContrast.GetLuminance(currentColor) > ColorContrast.GetLuminance(backgroundColor))
            {
                textElement.style.color = new StyleColor(Color.black);
            }
            else
            {
                textElement.style.color = new StyleColor(Color.white);
            }
        }
    }
    
    private void ApplyLargeTextMode()
    {
        var textElements = _root.Query<TextElement>().ToList();
        
        foreach (var textElement in textElements)
        {
            var currentSize = textElement.style.fontSize.value.value;
            textElement.style.fontSize = new StyleLength(currentSize * 1.5f);
        }
    }
    
    private void ApplyReducedMotion()
    {
        // Disable all animations and transitions
        var allElements = _root.Query().ToList();
        
        foreach (var element in allElements)
        {
            element.style.transitionProperty = new List<StylePropertyName> { new StylePropertyName("none") };
            element.style.transitionDuration = new StyleList<TimeValue> { new TimeValue(0f, TimeUnit.Second) };
        }
    }
    
    private void SetupKeyboardNavigation()
    {
        var focusableElements = _root.Query().Where(e => e.focusable).ToList();
        
        foreach (var element in focusableElements)
        {
            element.RegisterCallback<NavigationMoveEvent>(OnNavigationMove);
        }
    }
    
    private void OnNavigationMove(NavigationMoveEvent evt)
    {
        // Handle keyboard navigation
        switch (evt.direction)
        {
            case NavigationMoveEvent.Direction.Up:
                NavigateToNextElement(evt, Vector2.up);
                break;
            case NavigationMoveEvent.Direction.Down:
                NavigateToNextElement(evt, Vector2.down);
                break;
            case NavigationMoveEvent.Direction.Left:
                NavigateToNextElement(evt, Vector2.left);
                break;
            case NavigationMoveEvent.Direction.Right:
                NavigateToNextElement(evt, Vector2.right);
                break;
        }
    }
    
    private void SetupScreenReaderSupport()
    {
        // Add accessibility labels for screen readers
        var buttons = _root.Query<Button>().ToList();
        
        foreach (var button in buttons)
        {
            button.RegisterCallback<GeometryChangedEvent>(OnButtonGeometryChanged);
        }
    }
    
    private void OnButtonGeometryChanged(GeometryChangedEvent evt)
    {
        var button = (Button)evt.target;
        
        // Set accessibility label
        var label = button.text;
        button.SetProperty("accessibility-label", label);
        
        // Set accessibility role
        button.SetProperty("accessibility-role", "button");
    }
}
```

## Performance Optimization

### UI Rendering Optimization

#### Efficient UI Updates
```csharp
// High-performance UI rendering system
public class UIPerformanceOptimizer : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private float _updateRate = 30f; // 30 FPS for UI
    
    private VisualElement _root;
    private Dictionary<string, UIUpdateData> _updateData;
    private float _lastUpdateTime;
    
    private struct UIUpdateData
    {
        public VisualElement Element;
        public string Property;
        public object Value;
        public float LastUpdateTime;
        public float UpdateInterval;
    }
    
    private void Start()
    {
        _root = _uiDocument.rootVisualElement;
        _updateData = new Dictionary<string, UIUpdateData>();
        
        InitializeOptimizedUI();
    }
    
    private void InitializeOptimizedUI()
    {
        // Optimize UI elements for performance
        OptimizeTextElements();
        OptimizeImages();
        OptimizeAnimations();
    }
    
    private void OptimizeTextElements()
    {
        var textElements = _root.Query<TextElement>().ToList();
        
        foreach (var textElement in textElements)
        {
            // Disable text wrapping for better performance
            textElement.style.whiteSpace = new StyleEnum<WhiteSpace>(WhiteSpace.NoWrap);
            
            // Use fixed font sizes to avoid layout recalculations
            textElement.style.fontSize = new StyleLength(Length.Percent(100f));
            
            // Cache text elements for batch updates
            RegisterForBatchUpdate(textElement, "text", 0.1f);
        }
    }
    
    private void OptimizeImages()
    {
        var images = _root.Query<VisualElement>().Where(e => e.style.backgroundImage.value.value != null).ToList();
        
        foreach (var image in images)
        {
            // Use sprite atlases for better performance
            image.style.backgroundImage = new StyleBackground(LoadSpriteAtlas());
            
            // Disable image scaling for better performance
            image.style.scale = new Vector3(1f, 1f, 1f);
        }
    }
    
    private void OptimizeAnimations()
    {
        // Use CSS transitions instead of Unity animations for better performance
        var animatedElements = _root.Query().ToList();
        
        foreach (var element in animatedElements)
        {
            // Use GPU-accelerated properties
            element.style.transitionProperty = new List<StylePropertyName>
            {
                new StylePropertyName("transform"),
                new StylePropertyName("opacity")
            };
        }
    }
    
    private void Update()
    {
        // Batch UI updates for better performance
        if (Time.time - _lastUpdateTime >= 1f / _updateRate)
        {
            BatchUpdateUI();
            _lastUpdateTime = Time.time;
        }
    }
    
    private void BatchUpdateUI()
    {
        var currentTime = Time.time;
        var elementsToUpdate = new List<UIUpdateData>();
        
        // Collect elements that need updating
        foreach (var kvp in _updateData)
        {
            var updateData = kvp.Value;
            if (currentTime - updateData.LastUpdateTime >= updateData.UpdateInterval)
            {
                elementsToUpdate.Add(updateData);
                updateData.LastUpdateTime = currentTime;
                _updateData[kvp.Key] = updateData;
            }
        }
        
        // Batch update all elements
        foreach (var updateData in elementsToUpdate)
        {
            UpdateUIElement(updateData);
        }
    }
    
    private void UpdateUIElement(UIUpdateData updateData)
    {
        switch (updateData.Property)
        {
            case "text":
                if (updateData.Element is TextElement textElement)
                {
                    textElement.text = updateData.Value.ToString();
                }
                break;
            case "color":
                if (updateData.Value is Color color)
                {
                    updateData.Element.style.color = new StyleColor(color);
                }
                break;
            case "opacity":
                if (updateData.Value is float opacity)
                {
                    updateData.Element.style.opacity = new StyleFloat(opacity);
                }
                break;
        }
    }
    
    public void RegisterForBatchUpdate(VisualElement element, string property, float updateInterval)
    {
        var key = $"{element.name}_{property}";
        _updateData[key] = new UIUpdateData
        {
            Element = element,
            Property = property,
            Value = null,
            LastUpdateTime = 0f,
            UpdateInterval = updateInterval
        };
    }
    
    public void QueueUIUpdate(string elementName, string property, object value)
    {
        var key = $"{elementName}_{property}";
        if (_updateData.ContainsKey(key))
        {
            var updateData = _updateData[key];
            updateData.Value = value;
            _updateData[key] = updateData;
        }
    }
}
```

---

**Last Updated**: 2026-02-20  
**Skill Version**: 1.0  
**UI Framework**: Unity UI Toolkit  
**Design Philosophy**: Nintendo + Competitive Gaming  
**Maintainer**: UI/UX Design Team
