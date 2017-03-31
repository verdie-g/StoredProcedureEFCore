Benchmark made with 50000 rows
``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows version 10.0.14393
Processor=Intel(R) Core(TM) i5-7200U CPU 2.50GHz, ProcessorCount=4
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
dotnet cli version=1.0.1
  [Host]     : .NET Core 4.6.25009.03, 64bit RyuJIT [AttachedDebugger]
  DefaultJob : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |                                        Method |        Mean |    StdDev |
 |---------------------------------------------- |------------ |---------- |
 |                  LoadStoredProcedureHardCoded | 188.3435 ms | 1.8572 ms |
 |                 LoadStoredProcedureReflective | 180.0493 ms | 3.4271 ms |
 | LoadStoredProcedureReflectiveWithOptimisation | 167.5520 ms | 1.0534 ms |
