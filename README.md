Benchmark made with 50000 rows
``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows version 10.0.14393
Processor=Intel(R) Core(TM) i5-7200U CPU 2.50GHz, ProcessorCount=4
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
dotnet cli version=1.0.1
  [Host]     : .NET Core 4.6.25009.03, 64bit RyuJITDEBUG [AttachedDebugger]
  DefaultJob : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |                                        Method |        Mean |    StdDev |
 |---------------------------------------------- |------------ |---------- |
 |                  LoadStoredProcedureHardCoded | 190.4755 ms | 2.7444 ms |
 |                 LoadStoredProcedureReflective | 176.7193 ms | 8.9337 ms |
 | LoadStoredProcedureReflectiveWithOptimisation | 175.6267 ms | 5.2395 ms |
