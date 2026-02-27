# Unity ECS Patterns Agent - Specialized Documentation

## Overview

The `unity-ecs-patterns` agent provides expertise in Unity's Entity Component System (ECS), Data-Oriented Technology Stack (DOTS), and high-performance systems for the Snake Prototype project.

## Core Responsibilities

### ECS Architecture Design
- Entity Component System implementation patterns
- Data-oriented design principles
- Component and system organization
- Performance optimization through DOTS

### High-Performance Systems
- Job System utilization
- Burst compiler optimization
- Memory management strategies
- Multi-threading considerations

### Large-Scale Entity Management
- Efficient entity spawning and destruction
- Component data optimization
- System scheduling and dependencies
- Performance profiling and optimization

## ECS Fundamentals

### Core Concepts

#### Entity-Component-System Pattern
```csharp
// Core ECS structure for Snake Prototype
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;

// Components
public struct SnakeSegment : IComponentData
{
    public int SegmentIndex;
    public float2 Direction;
    public float MoveTimer;
}

public struct SnakeHead : IComponentData
{
    public float2 NextDirection;
    public float Speed;
}

public struct GridPosition : IComponentData
{
    public int2 Position;
    public bool IsWall;
}

public struct DetectionSource : IComponentData
{
    public float DetectionRadius;
    public float DetectionAngle;
    public float DetectionStrength;
    public float3 Forward;
}

// Systems
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class SnakeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithAll<SnakeHead>()
            .ForEach((ref SnakeHead head, ref Translation translation, ref SnakeSegment segment) =>
            {
                // Update movement timer
                segment.MoveTimer += deltaTime;
                
                if (segment.MoveTimer >= 1.0f / head.Speed)
                {
                    segment.MoveTimer = 0f;
                    
                    // Update position
                    float2 newPosition = (float2)translation.Value.xy + segment.Direction;
                    translation.Value.xy = new float3(newPosition.x, 0, newPosition.y);
                    
                    // Update direction
                    segment.Direction = head.NextDirection;
                }
            }).ScheduleParallel();
    }
}
```

#### Component Data Optimization
```csharp
// Optimized component structures for cache efficiency
using Unity.Entities;
using Unity.Mathematics;

// Chunk-friendly components (16-byte aligned)
public struct MovementData : IComponentData
{
    public float2 Velocity;
    public float Speed;
    public float Rotation;
}

public struct CollisionData : IComponentData
{
    public float Radius;
    public int Layer;
    public bool IsTrigger;
}

// Tag components for efficient queries
public struct SnakeTag : IComponentData { }
public struct WallTag : IComponentData { }
public struct EnergyCoreTag : IComponentData { }

// Shared components for common data
public struct SharedRenderData : ISharedComponentData
{
    public Mesh Mesh;
    public Material Material;
}

// Buffer components for dynamic data
public struct PathBuffer : IBufferElementData
{
    public float2 Position;
    public float Timestamp;
}

// System state components for singleton data
public struct GameState : ISystemStateComponentData
{
    public int Score;
    public int CurrentLevel;
    public bool IsGameOver;
}
```

## Job System Integration

### Parallel Job Implementation

#### Snake Movement Jobs
```csharp
// High-performance snake movement using Jobs
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
public struct SnakeMovementJob : IJobParallelFor
{
    public NativeArray<float3> Positions;
    public NativeArray<float2> Directions;
    public NativeArray<float> MoveTimers;
    public NativeArray<float2> NextDirections;
    public float DeltaTime;
    public float Speed;
    
    public void Execute(int index)
    {
        MoveTimers[index] += DeltaTime;
        
        if (MoveTimers[index] >= 1.0f / Speed)
        {
            MoveTimers[index] = 0f;
            
            // Update position
            float2 currentPos = new float2(Positions[index].x, Positions[index].z);
            float2 newPos = currentPos + Directions[index];
            Positions[index] = new float3(newPos.x, Positions[index].y, newPos.y);
            
            // Update direction
            Directions[index] = NextDirections[index];
        }
    }
}

// System that schedules the job
public class SnakeMovementJobSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities
            .WithAll<SnakeTag>()
            .ForEach((ref Translation translation, 
                     ref SnakeSegment segment, 
                     ref SnakeHead head) =>
            {
                // Schedule job for each snake entity
                var job = new SnakeMovementJob
                {
                    DeltaTime = deltaTime,
                    Speed = head.Speed
                };
                
                // Job would be scheduled here with proper NativeArrays
            }).WithoutBurst().Run();
    }
}
```

#### Detection System Jobs
```csharp
// Parallel detection calculations
[BurstCompile]
public struct DetectionCalculationJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> SnakePositions;
    [ReadOnly] public NativeArray<DetectionSourceData> DetectionSources;
    [ReadOnly] public NativeArray<float3> GuardPositions;
    public NativeArray<float> DetectionLevels;
    
    public void Execute(int index)
    {
        float3 snakePos = SnakePositions[index];
        float totalDetection = 0f;
        
        // Check all detection sources
        for (int i = 0; i < DetectionSources.Length; i++)
        {
            var source = DetectionSources[i];
            float3 sourcePos = GuardPositions[i];
            
            // Calculate distance-based detection
            float distance = math.distance(snakePos, sourcePos);
            if (distance <= source.DetectionRadius)
            {
                // Calculate angle-based detection
                float3 toSnake = math.normalize(snakePos - sourcePos);
                float angle = math.acos(math.dot(source.Forward, toSnake));
                
                if (angle <= source.DetectionAngle * 0.5f)
                {
                    float detectionStrength = source.DetectionStrength * (1f - distance / source.DetectionRadius);
                    totalDetection += detectionStrength;
                }
            }
        }
        
        DetectionLevels[index] = math.saturate(totalDetection);
    }
}

// Detection system with job scheduling
public class DetectionJobSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Get all relevant entities
        var snakeQuery = GetEntityQuery(typeof(Translation), typeof(SnakeTag));
        var guardQuery = GetEntityQuery(typeof(Translation), typeof(DetectionSource));
        
        var snakePositions = snakeQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var guardPositions = guardQuery.ToComponentDataArray<Translation>(Allocator.TempJob);
        var detectionSources = guardQuery.ToComponentDataArray<DetectionSource>(Allocator.TempJob);
        
        var detectionLevels = new NativeArray<float>(snakePositions.Length, Allocator.TempJob);
        
        // Schedule detection calculation job
        var detectionJob = new DetectionCalculationJob
        {
            SnakePositions = new NativeArray<float3>(snakePositions.Length, Allocator.TempJob),
            DetectionSources = detectionSources,
            GuardPositions = guardPositions,
            DetectionLevels = detectionLevels
        };
        
        // Copy snake positions
        for (int i = 0; i < snakePositions.Length; i++)
        {
            detectionJob.SnakePositions[i] = snakePositions[i].Value;
        }
        
        JobHandle handle = detectionJob.Schedule(snakePositions.Length, 64);
        handle.Complete();
        
        // Apply detection results
        // ... apply detection levels to entities
        
        // Dispose of native arrays
        snakePositions.Dispose();
        guardPositions.Dispose();
        detectionSources.Dispose();
        detectionLevels.Dispose();
    }
}
```

## Burst Compiler Optimization

### Burst-Optimized Systems

#### High-Performance Grid Operations
```csharp
// Burst-optimized grid system
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

[BurstCompile]
public struct GridCollisionJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> Positions;
    [ReadOnly] public NativeArray<int2> GridSize;
    [ReadOnly] public NativeArray<bool> WallData;
    public NativeArray<bool> CollisionResults;
    
    public void Execute(int index)
    {
        float3 pos = Positions[index];
        int2 gridPos = (int2)math.floor(new float2(pos.x, pos.z));
        
        // Check bounds
        if (gridPos.x < 0 || gridPos.x >= GridSize[0].x ||
            gridPos.y < 0 || gridPos.y >= GridSize[0].y)
        {
            CollisionResults[index] = true;
            return;
        }
        
        // Check wall collision
        int gridIndex = gridPos.y * GridSize[0].x + gridPos.x;
        CollisionResults[index] = WallData[gridIndex];
    }
}

// Burst-optimized math utilities
[BurstCompile]
public static class SnakeMath
{
    [BurstCompile]
    public static float2 Rotate90(float2 vector)
    {
        return new float2(-vector.y, vector.x);
    }
    
    [BurstCompile]
    public static float2 RotateMinus90(float2 vector)
    {
        return new float2(vector.y, -vector.x);
    }
    
    [BurstCompile]
    public static bool IsOppositeDirection(float2 dir1, float2 dir2)
    {
        return math.dot(dir1, dir2) < -0.9f;
    }
    
    [BurstCompile]
    public static float2 ClampToGrid(float2 position, int2 gridSize)
    {
        return math.clamp(position, new float2(0, 0), new float2(gridSize.x - 1, gridSize.y - 1));
    }
}
```

#### Memory-Efficient Data Structures
```csharp
// Burst-compatible data structures
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

// Custom NativeList for burst compatibility
[BurstCompile]
public struct BurstNativeList<T> where T : unmanaged
{
    [NativeDisableUnsafePtrRestriction]
    private unsafe T* _ptr;
    private int _capacity;
    private int _length;
    private Allocator _allocator;
    
    public BurstNativeList(int capacity, Allocator allocator)
    {
        _capacity = capacity;
        _length = 0;
        _allocator = allocator;
        unsafe
        {
            _ptr = (T*)UnsafeUtility.Malloc(capacity * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), allocator);
        }
    }
    
    public int Length => _length;
    public int Capacity => _capacity;
    
    [BurstCompile]
    public void Add(T item)
    {
        if (_length >= _capacity)
        {
            Resize(_capacity * 2);
        }
        
        unsafe
        {
            _ptr[_length] = item;
        }
        _length++;
    }
    
    [BurstCompile]
    public T this[int index]
    {
        get
        {
            unsafe
            {
                return _ptr[index];
            }
        }
        set
        {
            unsafe
            {
                _ptr[index] = value;
            }
        }
    }
    
    private void Resize(int newCapacity)
    {
        unsafe
        {
            T* newPtr = (T*)UnsafeUtility.Malloc(newCapacity * UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), _allocator);
            UnsafeUtility.MemCpy(newPtr, _ptr, _length * UnsafeUtility.SizeOf<T>());
            UnsafeUtility.Free(_ptr, _allocator);
            _ptr = newPtr;
        }
        _capacity = newCapacity;
    }
    
    public void Dispose()
    {
        unsafe
        {
            UnsafeUtility.Free(_ptr, _allocator);
        }
    }
}
```

## System Architecture Patterns

### System Group Organization

#### System Dependencies and Scheduling
```csharp
// System group organization for Snake Prototype
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(TransformSystemGroup))]
public partial class SnakeSimulationSystemGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        // Define system execution order
        AddSystemToUpdateList(World.GetOrCreateSystem<InputSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<SnakeMovementSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<CollisionDetectionSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<DetectionSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<GameStateSystem>());
    }
}

[UpdateInGroup(typeof(PresentationSystemGroup))]
[UpdateAfter(typeof(EndFrameTRSystemGroup))]
public partial class SnakePresentationSystemGroup : ComponentSystemGroup
{
    protected override void OnCreate()
    {
        AddSystemToUpdateList(World.GetOrCreateSystem<SnakeRenderingSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<GridRenderingSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<VFXRenderingSystem>());
        AddSystemToUpdateList(World.GetOrCreateSystem<UIRenderingSystem>());
    }
}

// System with explicit dependencies
[UpdateInGroup(typeof(SnakeSimulationSystemGroup))]
[UpdateBefore(typeof(CollisionDetectionSystem))]
public partial class SnakeMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Movement logic here
    }
}

[UpdateInGroup(typeof(SnakeSimulationSystemGroup))]
[UpdateAfter(typeof(SnakeMovementSystem))]
public partial class CollisionDetectionSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Collision detection logic here
    }
}
```

### Component Data Management

#### Efficient Component Queries
```csharp
// Optimized component queries
public class OptimizedSnakeSystem : SystemBase
{
    private EntityQuery _snakeQuery;
    private EntityQuery _gridQuery;
    
    protected override void OnCreate()
    {
        // Pre-configure queries for performance
        var snakeQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<SnakeTag>(),
                ComponentType.ReadOnly<Translation>(),
                ComponentType.ReadWrite<SnakeSegment>(),
                ComponentType.ReadWrite<SnakeHead>()
            },
            Options = EntityQueryOptions.FilterWriteGroup
        };
        
        _snakeQuery = GetEntityQuery(snakeQueryDesc);
        
        var gridQueryDesc = new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<GridPosition>(),
                ComponentType.ReadOnly<WallTag>()
            },
            Options = EntityQueryOptions.FilterWriteGroup
        };
        
        _gridQuery = GetEntityQuery(gridQueryDesc);
    }
    
    protected override void OnUpdate()
    {
        // Use pre-configured queries
        var snakeEntities = _snakeQuery.ToEntityArray(Allocator.TempJob);
        var gridEntities = _gridQuery.ToEntityArray(Allocator.TempJob);
        
        // Process entities efficiently
        ProcessSnakeEntities(snakeEntities);
        ProcessGridEntities(gridEntities);
        
        // Dispose temporary arrays
        snakeEntities.Dispose();
        gridEntities.Dispose();
    }
    
    private void ProcessSnakeEntities(NativeArray<Entity> snakeEntities)
    {
        // Efficient processing logic
    }
    
    private void ProcessGridEntities(NativeArray<Entity> gridEntities)
    {
        // Efficient processing logic
    }
}
```

## Performance Optimization

### Memory Management

#### Efficient Entity Spawning
```csharp
// High-performance entity spawning
public class EntitySpawner : SystemBase
{
    private EntityArchetype _snakeSegmentArchetype;
    private EntityArchetype _energyCoreArchetype;
    
    protected override void OnCreate()
    {
        // Pre-create archetypes for fast spawning
        _snakeSegmentArchetype = EntityManager.CreateArchetype(
            typeof(SnakeTag),
            typeof(Translation),
            typeof(Rotation),
            typeof(SnakeSegment),
            typeof(LocalToWorld)
        );
        
        _energyCoreArchetype = EntityManager.CreateArchetype(
            typeof(EnergyCoreTag),
            typeof(Translation),
            typeof(Rotation),
            typeof(NonUniformScale),
            typeof(LocalToWorld)
        );
    }
    
    public Entity SpawnSnakeSegment(float3 position)
    {
        var entity = EntityManager.CreateEntity(_snakeSegmentArchetype);
        EntityManager.SetComponentData(entity, new Translation { Value = position });
        EntityManager.SetComponentData(entity, new Rotation { Value = quaternion.identity });
        EntityManager.SetComponentData(entity, new SnakeSegment 
        { 
            SegmentIndex = 0,
            Direction = float2.zero,
            MoveTimer = 0f
        });
        
        return entity;
    }
    
    public Entity SpawnEnergyCore(float3 position)
    {
        var entity = EntityManager.CreateEntity(_energyCoreArchetype);
        EntityManager.SetComponentData(entity, new Translation { Value = position });
        EntityManager.SetComponentData(entity, new Rotation { Value = quaternion.identity });
        EntityManager.SetComponentData(entity, new NonUniformScale { Value = new float3(0.5f, 0.5f, 0.5f) });
        
        return entity;
    }
}
```

#### Chunk-Based Iteration
```csharp
// Chunk-based iteration for maximum performance
public class ChunkOptimizedSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var query = GetEntityQuery(typeof(SnakeTag), typeof(Translation), typeof(SnakeSegment));
        
        var chunks = query.CreateArchetypeChunkArray(Allocator.TempJob);
        var snakeType = GetArchetypeChunkComponentType<SnakeTag>(true);
        var translationType = GetArchetypeChunkComponentType<Translation>(false);
        var segmentType = GetArchetypeChunkComponentType<SnakeSegment>(false);
        
        for (int chunkIndex = 0; chunkIndex < chunks.Length; chunkIndex++)
        {
            var chunk = chunks[chunkIndex];
            var snakeComponents = chunk.GetNativeArray(snakeType);
            var translationComponents = chunk.GetNativeArray(translationType);
            var segmentComponents = chunk.GetNativeArray(segmentType);
            
            // Process all entities in chunk
            for (int entityIndex = 0; entityIndex < chunk.Count; entityIndex++)
            {
                // Process entity data
                var translation = translationComponents[entityIndex];
                var segment = segmentComponents[entityIndex];
                
                // Update entity
                translationComponents[entityIndex] = UpdateTranslation(translation, segment);
            }
        }
        
        chunks.Dispose();
    }
    
    private Translation UpdateTranslation(Translation translation, SnakeSegment segment)
    {
        // Update logic here
        return translation;
    }
}
```

## Testing and Profiling

### ECS Performance Testing

#### Benchmark Systems
```csharp
// Performance benchmarking for ECS systems
public class ECSPerformanceTests
{
    [Test, Performance]
    public void SnakeMovementSystem_PerformanceTest()
    {
        var world = new World("TestWorld");
        var system = world.GetOrCreateSystem<SnakeMovementSystem>();
        
        // Create test entities
        var entityManager = world.EntityManager;
        var archetype = entityManager.CreateArchetype(
            typeof(SnakeTag),
            typeof(Translation),
            typeof(SnakeSegment),
            typeof(SnakeHead)
        );
        
        // Spawn 1000 snake entities
        var entities = new NativeArray<Entity>(1000, Allocator.Temp);
        entityManager.CreateEntity(archetype, entities);
        
        // Benchmark system performance
        Measure.Method(() =>
        {
            system.Update();
        })
        .WarmupCount(10)
        .MeasurementCount(100)
        .IterationsPerMeasurement(10)
        .GC()
        .Run();
        
        // Cleanup
        entityManager.DestroyEntity(entities);
        entities.Dispose();
        world.Dispose();
    }
}
```

### Memory Profiling

#### ECS Memory Analysis
```csharp
// Memory profiling utilities for ECS
public static class ECSMemoryProfiler
{
    public static void LogWorldMemoryUsage(World world)
    {
        var entityManager = world.EntityManager;
        
        Debug.Log($"Total Entities: {entityManager.EntityCount}");
        Debug.Log($"Total Chunks: {entityManager.Debug.GetTotalChunkCount()}");
        
        // Log memory usage by component type
        var allComponentTypes = entityManager.Debug.GetAllComponentTypes();
        foreach (var componentType in allComponentTypes)
        {
            var query = entityManager.CreateEntityQuery(componentType);
            var chunkCount = query.CalculateChunkCount();
            var entityCount = query.CalculateEntityCount();
            
            Debug.Log($"{componentType.GetManagedType().Name}: {entityCount} entities, {chunkCount} chunks");
        }
    }
    
    public static void LogSystemPerformance(World world)
    {
        var systemGroups = world.Systems;
        
        foreach (var systemGroup in systemGroups)
        {
            if (systemGroup is ComponentSystemGroup group)
            {
                Debug.Log($"System Group: {group.GetType().Name}");
                
                foreach (var system in group.Systems)
                {
                    if (system is ComponentSystemBase componentSystem)
                    {
                        Debug.Log($"  - {componentSystem.GetType().Name}");
                    }
                }
            }
        }
    }
}
```

## Integration with Existing Systems

### Hybrid ECS Approach

#### Combining ECS with MonoBehaviour
```csharp
// Hybrid system for gradual ECS migration
public class HybridSnakeManager : MonoBehaviour
{
    private World _ecsWorld;
    private EntityManager _entityManager;
    private Entity _snakeEntity;
    
    private void Start()
    {
        // Create ECS world
        _ecsWorld = new World("SnakeWorld");
        _entityManager = _ecsWorld.EntityManager;
        
        // Create snake entity in ECS
        CreateECSsnake();
        
        // Initialize ECS systems
        InitializeECSSystems();
    }
    
    private void CreateECSsnake()
    {
        var archetype = _entityManager.CreateArchetype(
            typeof(SnakeTag),
            typeof(Translation),
            typeof(SnakeSegment),
            typeof(SnakeHead)
        );
        
        _snakeEntity = _entityManager.CreateEntity(archetype);
        _entityManager.SetComponentData(_snakeEntity, new Translation { Value = transform.position });
        _entityManager.SetComponentData(_snakeEntity, new SnakeSegment 
        { 
            SegmentIndex = 0,
            Direction = new float2(1, 0),
            MoveTimer = 0f
        });
        _entityManager.SetComponentData(_snakeEntity, new SnakeHead 
        { 
            NextDirection = new float2(1, 0),
            Speed = 5f
        });
    }
    
    private void Update()
    {
        // Update ECS world
        _ecsWorld.Update();
        
        // Sync ECS position to MonoBehaviour
        var translation = _entityManager.GetComponentData<Translation>(_snakeEntity);
        transform.position = translation.Value;
    }
    
    private void OnDestroy()
    {
        _ecsWorld.Dispose();
    }
}
```

---

**Last Updated**: 2026-02-20  
**Skill Version**: 1.0  
**DOTS Version**: 1.0+  
**Maintainer**: ECS Development Team
