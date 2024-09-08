using BenchmarkDotNet.Attributes;
using GameLibrary;

namespace PerformanceBenchmarks;

[MemoryDiagnoser(true), ShortRunJob]
public class GameUpdateBenchmarks
{
    [Benchmark]
    public void Update()
    {
        GameState.Update();
    }
}
