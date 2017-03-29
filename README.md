``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows version 10.0.14393
Processor=Intel(R) Core(TM) i5-7200U CPU 2.50GHz, ProcessorCount=4
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
dotnet cli version=1.0.1
  [Host]     : .NET Core 4.6.25009.03, 64bit RyuJITDEBUG [AttachedDebugger]
  DefaultJob : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |                                        Method |        Mean |    StdErr |    StdDev |
 |---------------------------------------------- |------------ |---------- |---------- |
 |                  LoadStoredProcedureHardCoded | 201.7669 ms | 2.3002 ms | 9.4840 ms |
 |                 LoadStoredProcedureReflective | 177.9699 ms | 2.0260 ms | 7.8467 ms |
 | LoadStoredProcedureReflectiveWithOptimisation | 172.3568 ms | 0.4118 ms | 1.4847 ms |
