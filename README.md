Benchmark made with 50000 rows
``` ini

BenchmarkDotNet=v0.10.3.0, OS=Microsoft Windows version 10.0.14393
Processor=Intel(R) Core(TM) i5-7200U CPU 2.50GHz, ProcessorCount=4
Frequency=2648438 Hz, Resolution=377.5810 ns, Timer=TSC
dotnet cli version=1.0.1
  [Host]     : .NET Core 4.6.25009.03, 64bit RyuJIT [AttachedDebugger]
  DefaultJob : .NET Core 4.6.25009.03, 64bit RyuJIT


```
 |                                        Method |        Mean |     StdDev |      Median |
 |---------------------------------------------- |------------ |----------- |------------ |
 |                  LoadStoredProcedureHardCoded | 194.2672 ms | 12.8725 ms | 187.1605 ms |
 |                 LoadStoredProcedureReflective | 168.6564 ms |  0.6452 ms | 168.8288 ms |
 | LoadStoredProcedureReflectiveWithOptimisation | 160.1283 ms |  1.2191 ms | 160.0825 ms |

 The third method will also read the Field attribute to get the database field name.
 It is useful when the name contains an underscore in the database but not in the c# code.
 Ex: creator_id => CreatorId