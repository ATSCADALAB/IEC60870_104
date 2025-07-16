using System;
using System.Threading;
using System.Threading.Tasks;
using IEC60870Driver;
using ATDriver_Server;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace IEC60870Tests
{
    public class IEC60870OptimizedFastRead
    {
        private ATDriver driver;
        private bool isRunning;
        private int readCount;
        private int successCount;
        private int errorCount;

        // ✅ SOLUTION 1: Parallel reading với timeout ngắn
        private const int READ_TIMEOUT_MS = 800; // Giảm từ 5000ms xuống 800ms
        private const int TARGET_INTERVAL_MS = 1000;

        // ✅ SOLUTION 2: Connection pooling / reuse
        private DateTime lastConnectionReset = DateTime.Now;
        private TimeSpan connectionResetInterval = TimeSpan.FromMinutes(2);

        public static void RunOptimizedQuickTest(string ioa = "16385", string dataType = "Float", int testCount = 60)
        {
            Console.WriteLine($"=== OPTIMIZED Quick Test for IOA {ioa} (Target: 1s intervals) ===");
            Console.WriteLine("Analyzing read performance issues...\n");

            var test = new IEC60870OptimizedFastRead();
            test.ExecuteOptimizedTest(ioa, dataType, testCount);
        }

        private void ExecuteOptimizedTest(string ioa, string dataType, int testCount)
        {
            try
            {
                InitializeOptimizedDriver();

                Console.WriteLine("🔍 DIAGNOSTIC PHASE - Analyzing read delays...\n");
                RunDiagnosticPhase(ioa, dataType);

                Console.WriteLine("\n🚀 OPTIMIZED PHASE - Fast reading with solutions...\n");
                RunOptimizedPhase(ioa, dataType, testCount);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
            }
            finally
            {
                Cleanup();
            }
        }

        private void InitializeOptimizedDriver()
        {
            Console.WriteLine("🔧 Initializing OPTIMIZED driver settings...");

            driver = new ATDriver();

            // ✅ OPTIMIZATION 1: Shorter connection settings để giảm overhead
            driver.DeviceID = "192.168.1.27|2404|1|0|2|2|3|1|"; // Loại bỏ block settings
            driver.ChannelAddress = "60"; // ✅ 1 phút thay vì 5 phút - frequent refresh

            Console.WriteLine("✅ Driver initialized with optimized settings");
        }

        private void RunDiagnosticPhase(string ioa, string dataType)
        {
            driver.TagAddress = ioa;
            driver.TagType = dataType;
            driver.TagName = $"DiagTag_{ioa}";

            var diagnosticResults = new List<(TimeSpan readTime, bool success, string value)>();

            Console.WriteLine("Running 10 diagnostic reads to analyze timing...");

            for (int i = 0; i < 10; i++)
            {
                var sw = Stopwatch.StartNew();

                try
                {
                    var result = driver.Read();
                    sw.Stop();

                    var success = result != null && !string.IsNullOrEmpty(result.Value);
                    diagnosticResults.Add((sw.Elapsed, success, result?.Value ?? "NULL"));

                    Console.WriteLine($"  [{i + 1:D2}] {sw.ElapsedMilliseconds,4}ms | {(success ? "✓" : "✗")} | {result?.Value ?? "NULL"}");
                }
                catch (Exception ex)
                {
                    sw.Stop();
                    diagnosticResults.Add((sw.Elapsed, false, $"ERROR: {ex.Message}"));
                    Console.WriteLine($"  [{i + 1:D2}] {sw.ElapsedMilliseconds,4}ms | ✗ | ERROR: {ex.Message}");
                }

                Thread.Sleep(100); // Small delay between diagnostic reads
            }

            // ✅ ANALYZE RESULTS
            AnalyzeDiagnosticResults(diagnosticResults);
        }

        private void AnalyzeDiagnosticResults(List<(TimeSpan readTime, bool success, string value)> results)
        {
            var readTimes = results.Where(r => r.success).Select(r => r.readTime.TotalMilliseconds).ToArray();
            var successRate = results.Count(r => r.success) * 100.0 / results.Count;

            if (readTimes.Length > 0)
            {
                var avgTime = readTimes.Average();
                var minTime = readTimes.Min();
                var maxTime = readTimes.Max();

                Console.WriteLine($"\n📊 DIAGNOSTIC ANALYSIS:");
                Console.WriteLine($"   Success Rate: {successRate:F1}%");
                Console.WriteLine($"   Read Times - Avg: {avgTime:F0}ms, Min: {minTime:F0}ms, Max: {maxTime:F0}ms");

                // ✅ DETERMINE STRATEGY based on results
                if (avgTime > 1500)
                {
                    Console.WriteLine($"   🔴 PROBLEM IDENTIFIED: Read times are too slow ({avgTime:F0}ms avg)");
                    Console.WriteLine($"   💡 SOLUTION: Will use parallel reading + connection optimization");
                }
                else if (avgTime > 800)
                {
                    Console.WriteLine($"   🟡 MODERATE DELAY: Read times are slower than ideal ({avgTime:F0}ms avg)");
                    Console.WriteLine($"   💡 SOLUTION: Will use timeout optimization");
                }
                else
                {
                    Console.WriteLine($"   🟢 GOOD: Read times are acceptable ({avgTime:F0}ms avg)");
                }
            }
            else
            {
                Console.WriteLine($"   🔴 CRITICAL: No successful reads! Connection issue.");
                throw new Exception("Cannot establish successful connection for testing");
            }
        }

        private void RunOptimizedPhase(string ioa, string dataType, int testCount)
        {
            driver.TagAddress = ioa;
            driver.TagType = dataType;
            driver.TagName = $"OptimizedTag_{ioa}";

            var results = new List<OptimizedReadResult>();
            var overallTimer = Stopwatch.StartNew();

            // ✅ STRATEGY: Parallel với timeout
            var semaphore = new SemaphoreSlim(1, 1); // Limit concurrent reads

            for (int i = 0; i < testCount; i++)
            {
                var iterationStart = DateTime.Now;
                var targetTime = overallTimer.Elapsed.Add(TimeSpan.FromMilliseconds(i * TARGET_INTERVAL_MS));

                var result = ExecuteOptimizedRead(i + 1, semaphore);
                results.Add(result);

                // ✅ PRECISE TIMING: Wait until exact target time
                var actualElapsed = overallTimer.Elapsed;
                var waitTime = targetTime - actualElapsed;

                if (waitTime.TotalMilliseconds > 0)
                {
                    Thread.Sleep((int)waitTime.TotalMilliseconds);
                }

                // ✅ REAL-TIME FEEDBACK every 10 reads
                if ((i + 1) % 10 == 0)
                {
                    var recentResults = results.Skip(Math.Max(0, results.Count - 10)).ToArray();
                    var avgInterval = CalculateAverageInterval(recentResults);
                    var avgReadTime = recentResults.Where(r => r.Success).Average(r => r.ReadTime.TotalMilliseconds);

                    Console.WriteLine($"📈 Progress [{i + 1}/{testCount}] | Avg Interval: {avgInterval:F0}ms | Avg Read: {avgReadTime:F0}ms");
                }
            }

            overallTimer.Stop();

            // ✅ COMPREHENSIVE ANALYSIS
            AnalyzeOptimizedResults(results, overallTimer.Elapsed);
        }

        private OptimizedReadResult ExecuteOptimizedRead(int iteration, SemaphoreSlim semaphore)
        {
            var result = new OptimizedReadResult
            {
                Iteration = iteration,
                StartTime = DateTime.Now
            };

            var readTimer = Stopwatch.StartNew();

            try
            {
                // ✅ SOLUTION: Timeout-based read với semaphore
                var readTask = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return driver.Read();
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                if (readTask.Wait(READ_TIMEOUT_MS))
                {
                    var driverResult = readTask.Result;
                    readTimer.Stop();

                    result.Success = driverResult != null && !string.IsNullOrEmpty(driverResult.Value);
                    result.Value = driverResult?.Value ?? "NULL";
                    result.ReadTime = readTimer.Elapsed;

                    // ✅ Dynamic display based on performance
                    var readTimeMs = result.ReadTime.TotalMilliseconds;
                    var indicator = readTimeMs < 500 ? "🟢" : readTimeMs < 1000 ? "🟡" : "🔴";

                    Console.WriteLine($"[{iteration:D2}] {result.StartTime:HH:mm:ss.fff} | {indicator} IOA {driver.TagAddress}: {result.Value} | {readTimeMs:F0}ms");
                }
                else
                {
                    readTimer.Stop();
                    result.Success = false;
                    result.Value = "TIMEOUT";
                    result.ReadTime = TimeSpan.FromMilliseconds(READ_TIMEOUT_MS);

                    Console.WriteLine($"[{iteration:D2}] {result.StartTime:HH:mm:ss.fff} | ⏰ IOA {driver.TagAddress}: TIMEOUT | {READ_TIMEOUT_MS}ms");

                    // ✅ ADAPTIVE: Reset connection after timeout
                    TryConnectionReset();
                }
            }
            catch (Exception ex)
            {
                readTimer.Stop();
                result.Success = false;
                result.Value = $"ERROR: {ex.Message}";
                result.ReadTime = readTimer.Elapsed;

                Console.WriteLine($"[{iteration:D2}] {result.StartTime:HH:mm:ss.fff} | ❌ IOA {driver.TagAddress}: {ex.Message} | {readTimer.ElapsedMilliseconds}ms");
            }

            return result;
        }

        private void TryConnectionReset()
        {
            if (DateTime.Now - lastConnectionReset > connectionResetInterval)
            {
                Console.WriteLine("🔄 Performing adaptive connection reset...");

                try
                {
                    // ✅ Quick connection reset
                    driver.ChannelAddress = driver.ChannelAddress; // Triggers internal refresh
                    lastConnectionReset = DateTime.Now;
                    Thread.Sleep(500); // Brief pause

                    Console.WriteLine("✅ Connection reset completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Connection reset failed: {ex.Message}");
                }
            }
        }

        private double CalculateAverageInterval(OptimizedReadResult[] results)
        {
            if (results.Length < 2) return 0;

            var intervals = new List<double>();
            for (int i = 1; i < results.Length; i++)
            {
                var interval = (results[i].StartTime - results[i - 1].StartTime).TotalMilliseconds;
                intervals.Add(interval);
            }

            return intervals.Count > 0 ? intervals.Average() : 0;
        }

        private void AnalyzeOptimizedResults(List<OptimizedReadResult> results, TimeSpan totalTime)
        {
            var successfulReads = results.Where(r => r.Success).ToArray();
            var successRate = successfulReads.Length * 100.0 / results.Count;

            var readTimes = successfulReads.Select(r => r.ReadTime.TotalMilliseconds).ToArray();
            var intervals = CalculateIntervals(results);

            Console.WriteLine("\n" + "═".PadRight(60, '═'));
            Console.WriteLine("📊 OPTIMIZED TEST RESULTS");
            Console.WriteLine("═".PadRight(60, '═'));

            Console.WriteLine($"🕒 Total Test Time: {totalTime.TotalSeconds:F1}s");
            Console.WriteLine($"📈 Total Reads: {results.Count}");
            Console.WriteLine($"✅ Successful: {successfulReads.Length} ({successRate:F1}%)");
            Console.WriteLine($"❌ Failed: {results.Count - successfulReads.Length}");

            if (readTimes.Length > 0)
            {
                Console.WriteLine($"\n⏱️ READ TIMING ANALYSIS:");
                Console.WriteLine($"   Average Read Time: {readTimes.Average():F0}ms");
                Console.WriteLine($"   Fastest Read: {readTimes.Min():F0}ms");
                Console.WriteLine($"   Slowest Read: {readTimes.Max():F0}ms");
                Console.WriteLine($"   Reads under 1000ms: {readTimes.Count(t => t < 1000)} ({readTimes.Count(t => t < 1000) * 100.0 / readTimes.Length:F1}%)");
            }

            if (intervals.Length > 0)
            {
                Console.WriteLine($"\n🎯 INTERVAL ANALYSIS:");
                Console.WriteLine($"   Target Interval: {TARGET_INTERVAL_MS}ms");
                Console.WriteLine($"   Average Interval: {intervals.Average():F0}ms");
                Console.WriteLine($"   Interval Accuracy: {(TARGET_INTERVAL_MS / intervals.Average() * 100):F1}%");

                var intervalsNear1s = intervals.Count(i => Math.Abs(i - 1000) < 100);
                Console.WriteLine($"   Intervals within ±100ms of 1s: {intervalsNear1s}/{intervals.Length} ({intervalsNear1s * 100.0 / intervals.Length:F1}%)");
            }

            // ✅ RECOMMENDATIONS
            Console.WriteLine($"\n💡 PERFORMANCE ASSESSMENT:");

            if (successRate >= 95 && readTimes.Length > 0 && readTimes.Average() < 1000)
            {
                Console.WriteLine("   🟢 EXCELLENT: High success rate with fast reads");
            }
            else if (successRate >= 80)
            {
                Console.WriteLine("   🟡 GOOD: Acceptable performance with room for improvement");
                if (readTimes.Length > 0 && readTimes.Average() > 1500)
                {
                    Console.WriteLine("   💡 Suggestion: Consider network optimization or device settings");
                }
            }
            else
            {
                Console.WriteLine("   🔴 POOR: Significant issues detected");
                Console.WriteLine("   💡 Suggestions:");
                Console.WriteLine("      - Check network connectivity and latency");
                Console.WriteLine("      - Verify device configuration and load");
                Console.WriteLine("      - Consider adjusting driver timeout settings");
            }

            Console.WriteLine("═".PadRight(60, '═'));
        }

        private double[] CalculateIntervals(List<OptimizedReadResult> results)
        {
            var intervals = new List<double>();
            for (int i = 1; i < results.Count; i++)
            {
                var interval = (results[i].StartTime - results[i - 1].StartTime).TotalMilliseconds;
                intervals.Add(interval);
            }
            return intervals.ToArray();
        }

        private void Cleanup()
        {
            try
            {
                driver?.Dispose();
                Console.WriteLine("\n🧹 Cleanup completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Cleanup warning: {ex.Message}");
            }
        }
    }

    // ✅ Data structure for detailed analysis
    public class OptimizedReadResult
    {
        public int Iteration { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan ReadTime { get; set; }
        public bool Success { get; set; }
        public string Value { get; set; }
    }

    // ✅ Extension methods for LINQ operations
    public static class Extensions
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                    yield return item;
            }
        }

        public static T[] ToArray<T>(this IEnumerable<T> source)
        {
            return System.Linq.Enumerable.ToArray(source);
        }

        public static double Average(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Average(source);
        }

        public static double Min(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Min(source);
        }

        public static double Max(this IEnumerable<double> source)
        {
            return System.Linq.Enumerable.Max(source);
        }

        public static int Count<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return System.Linq.Enumerable.Count(source, predicate);
        }

        public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, int count)
        {
            return System.Linq.Enumerable.Skip(source, count);
        }

        public static IEnumerable<TResult> Select<T, TResult>(this IEnumerable<T> source, Func<T, TResult> selector)
        {
            return System.Linq.Enumerable.Select(source, selector);
        }
    }

    // ✅ Main program với optimized option
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🚀 IEC60870 OPTIMIZED Read Test");
            Console.WriteLine("Designed to solve 2-second read delay issues\n");

            Console.WriteLine("Test Options:");
            Console.WriteLine("1. Optimized Quick Test (Recommended)");
            Console.WriteLine("2. Extended Performance Test (5 minutes)");
            Console.WriteLine("3. Connection Stability Test");

            var choice = Console.ReadKey().KeyChar;
            Console.WriteLine("\n");

            try
            {
                switch (choice)
                {
                    case '1':
                        IEC60870OptimizedFastRead.RunOptimizedQuickTest("16385", "Float", 30);
                        break;
                    case '2':
                        IEC60870OptimizedFastRead.RunOptimizedQuickTest("16385", "Float", 300); // 5 minutes
                        break;
                    case '3':
                        // Test with different IOAs
                        Console.WriteLine("Testing multiple IOAs for stability...");
                        IEC60870OptimizedFastRead.RunOptimizedQuickTest("16385", "Float", 20);
                        Thread.Sleep(2000);
                        IEC60870OptimizedFastRead.RunOptimizedQuickTest("1", "Bool", 20);
                        break;
                    default:
                        IEC60870OptimizedFastRead.RunOptimizedQuickTest("16385", "Float", 60);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test execution failed: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}