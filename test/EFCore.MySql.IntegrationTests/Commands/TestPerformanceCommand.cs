using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pomelo.EntityFrameworkCore.MySql.IntegrationTests.Commands
{

    public class TestPerformanceCommand : ITestPerformanceCommand
    {

        private AppDb _db;

        public TestPerformanceCommand(AppDb db)
        {
            _db = db;
        }

        private Lazy<ServiceProvider> _serviceProvider = new Lazy<ServiceProvider>(() =>
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection
                .AddLogging()
                .AddScoped<ITestPerformanceRunner, TestPerformanceRunner>();
            Startup.ConfigureEntityFramework(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(AppConfig.Config.GetSection("Logging"));
            return serviceProvider;
        });

        public void Run(int iterations, int concurrency, int ops)
        {
            Console.WriteLine("Testing with EF_BATCH_SIZE=" + AppConfig.EfBatchSize);
            Console.WriteLine();

            var recordNum = 0;
            async Task Insert1(AppDb db)
            {
                var blog = new Blog
                {
                    Title = "test " + Interlocked.Increment(ref recordNum)
                };
                db.Blogs.Add(blog);
                await db.SaveChangesAsync();
            }

            var selected = new ConcurrentQueue<string>();
            async Task Select1(AppDb db)
            {
                var blog = await db.Blogs.Skip(selected.Count).Take(1).OrderBy(m => m.Id).FirstOrDefaultAsync();
                if (blog != null)
                {
                    selected.Enqueue(blog.Title);
                }
            }

            var updatedNum = 0;
            async Task Update1(AppDb db)
            {
                var blog = await db.Blogs.Skip(selected.Count).Take(1).OrderBy(m => m.Id).FirstOrDefaultAsync();
                if (blog != null)
                {
                    blog.Title = "updated " + Interlocked.Increment(ref updatedNum);
                }
                await db.SaveChangesAsync();
            }

            var sleepNum = 0;
            async Task SleepMillisecond(AppDb db)
            {
                await db.Database.ExecuteSqlCommandAsync("SELECT SLEEP(0.001)");
                db.Database.CloseConnection();
                Interlocked.Increment(ref sleepNum);
            }

            async Task Insert100(AppDb db)
            {
                for (var i = 0; i < 100; i++)
                {
                    var blog = new Blog
                    {
                        Title = "test " + Interlocked.Increment(ref recordNum)
                    };
                    db.Blogs.Add(blog);
                }
                await db.SaveChangesAsync();
            }

            async Task Update100(AppDb db)
            {
                var blogs = await db.Blogs.Skip(updatedNum).Take(100).OrderBy(m => m.Id).ToListAsync();
                foreach (var blog in blogs)
                {
                    blog.Title = "updated " + Interlocked.Increment(ref updatedNum);
                }
                await db.SaveChangesAsync();
            }

            async Task Select100(AppDb db)
            {
                var blogs = await db.Blogs.Skip(selected.Count).Take(100).OrderBy(m => m.Id).ToListAsync();
                foreach (var blog in blogs)
                {
                    selected.Enqueue(blog.Title);
                }
            }

            _db.Database.ExecuteSqlCommand("DELETE FROM `" + _db.Model.FindEntityType(typeof(Blog)).Relational().TableName + "`");

            PerfTest(Insert1, "Insert 1", iterations, concurrency, ops).GetAwaiter().GetResult();
            var insertCount = _db.Blogs.Count();
            Console.WriteLine("Records Inserted: " + insertCount);
            Console.WriteLine();

            PerfTest(Update1, "Update 1", iterations, concurrency, ops).GetAwaiter().GetResult();
            var updateCount = updatedNum;
            Console.WriteLine("Records Updated: " + updateCount);
            Console.WriteLine();

            PerfTest(Select1, "Select 1", iterations, concurrency, ops).GetAwaiter().GetResult();
            var selectCount = selected.Count;
            Console.WriteLine("Records Selected: " + selectCount);
            string firstRecord;
            if (selected.TryDequeue(out firstRecord))
                Console.WriteLine("First Record: " + firstRecord);
            Console.WriteLine();

            PerfTest(SleepMillisecond, "Sleep 1ms", iterations, concurrency, ops).GetAwaiter().GetResult();
            Console.WriteLine("Total Sleep Commands: " + sleepNum);
            Console.WriteLine();

            PerfTest(Insert100, "Insert 100", iterations, concurrency, 1).GetAwaiter().GetResult();
            insertCount = _db.Blogs.Count() - insertCount;
            Console.WriteLine("Records Inserted: " + insertCount);
            Console.WriteLine();

            PerfTest(Update100, "Update 100", iterations, concurrency, 1).GetAwaiter().GetResult();
            updateCount = updatedNum - updateCount;
            Console.WriteLine("Records Updated: " + updateCount);
            Console.WriteLine();

            PerfTest(Select100, "Select 100", iterations, concurrency, 1).GetAwaiter().GetResult();
            selectCount = selected.Count - selectCount;
            Console.WriteLine("Records Selected: " + selectCount);
            Console.WriteLine();
        }

        public async Task PerfTest(Func<AppDb, Task> test, string testName, int iterations, int concurrency, int ops)
        {
            var timers = new List<TimeSpan>();
            var memoryResults = new List<long>();

            for (var iteration = 0; iteration < iterations; iteration++)
            {
                GC.Collect();
                // run the timed tests
                var tasks = new List<Task>();
                var start = DateTime.UtcNow;
                for (var connection = 0; connection < concurrency; connection++)
                {
                    var scope = _serviceProvider.Value.CreateScope();
                    var testPerformanceRunner = scope.ServiceProvider.GetService<ITestPerformanceRunner>();
                    async Task callable()
                    {
                        await testPerformanceRunner.ConnectionTask(test, ops);
                        scope.Dispose();
                    };
                    tasks.Add(callable());
                }
                await Task.WhenAll(tasks);

                var managedMemory = GC.GetTotalMemory(false);
                memoryResults.Add(managedMemory);
                timers.Add(DateTime.UtcNow - start);
            }

            Console.WriteLine("Test:                     " + testName);
            Console.WriteLine("Iterations:               " + iterations);
            Console.WriteLine("Concurrency:              " + concurrency);
            Console.WriteLine("Operations:               " + ops);
            Console.WriteLine("Times (Min, Average, Max) "
                              + timers.Min() + ", "
                              + TimeSpan.FromTicks(timers.Sum(timer => timer.Ticks) / timers.Count) + ", "
                              + timers.Max());

            const double bytesPerMib = 1024 * 1024;
            var memoryMin = memoryResults.Min() / bytesPerMib;
            var memoryMax = memoryResults.Max() / bytesPerMib;
            var memoryAvg = memoryResults.Sum() / bytesPerMib / memoryResults.Count;
            Console.WriteLine($"Memory (Min, Avg, Max)    {memoryMin:F2}, {memoryAvg:F2}, {memoryMax:F2} MiB");
            Console.WriteLine();
        }

    }

}
