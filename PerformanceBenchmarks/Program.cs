using BenchmarkDotNet.Running;
using PerformanceBenchmarks;

BenchmarkRunner.Run<PathfinderBenchmarks>();
BenchmarkRunner.Run<GameUpdateBenchmarks>();