using BenchmarkDotNet.Attributes;
using GameLibrary;

namespace PerformanceBenchmarks;

[MemoryDiagnoser(true), ShortRunJob]
public class PathfinderBenchmarks
{
    private static readonly Entity _entity = new()
    {
        Id = "test",
        Sprite = "example",
        MapPosition = new(0, 0)
    };

    [Benchmark]
    public bool From_0_0__To_39_39()
    {
        return Pathfinder.TryCalculatePath(_entity.MapPosition, new(39, 39), out var path);
    }


    [Benchmark]
    public bool From_0_0__To_10_10()
    {
        return Pathfinder.TryCalculatePath(_entity.MapPosition, new(10, 10), out var path);
    }
}
