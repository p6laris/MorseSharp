using BenchmarkDotNet.Running;
using Benchmark;

BenchmarkSwitcher.FromAssembly(typeof(MorseSharpBenchmarks).Assembly).Run(args);
