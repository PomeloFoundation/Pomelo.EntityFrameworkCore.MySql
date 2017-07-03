using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Threading.Tasks;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Models;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Commands{

    public static class TestPerformanceCommand{

        public static void Run(int iterations, int concurrency, int ops)
        {

            var recordNum = 0;
            async Task InsertOne(AppDb db)
            {
                var blog = new Blog
                {
                    Title = "test " + Interlocked.Increment(ref recordNum)
                };
                db.Blogs.Add(blog);
                await db.SaveChangesAsync();
            }

            var selected = new ConcurrentQueue<string>();
            async Task SelectOne(AppDb db)
            {
                var blog = await db.Blogs.OrderBy(m => m.Id).FirstOrDefaultAsync();
                selected.Enqueue(blog.Title);
            }

            var sleepNum = 0;
            async Task SleepMillisecond(AppDb db)
            {
                await db.Database.ExecuteSqlCommandAsync("SELECT SLEEP(0.001)");
                Interlocked.Increment(ref sleepNum);
            }

            using (var db = new AppDb())
            {
                db.Database.ExecuteSqlCommand($"DELETE FROM `{db.Model.FindEntityType(typeof(Blog)).Relational().TableName}`");
            }

            PerfTest(InsertOne, "Insert One", iterations, concurrency, ops).GetAwaiter().GetResult();
            using (var db = new AppDb())
            {
                Console.WriteLine("Records Inserted: " + db.Blogs.Count());
                Console.WriteLine();
            }

            PerfTest(SelectOne, "Select One", iterations, concurrency, ops).GetAwaiter().GetResult();
            Console.WriteLine("Records Selected: " +selected.Count);
            string firstRecord;
            if (selected.TryDequeue(out firstRecord))
                Console.WriteLine("First Record: " + firstRecord);
            Console.WriteLine();

            PerfTest(SleepMillisecond, "Sleep 1ms", iterations, concurrency, ops).GetAwaiter().GetResult();
            Console.WriteLine("Total Sleep Commands: " + sleepNum);
            Console.WriteLine();
        }

        public static async Task PerfTest(Func<AppDb, Task> test, string testName, int iterations, int concurrency, int ops)
        {
            var timers = new List<TimeSpan>();
            for (var iteration = 0; iteration < iterations; iteration++)
            {
                var tasks = new List<Task>();
                var start = DateTime.UtcNow;
                for (var connection = 0; connection < concurrency; connection++)
                {
                    tasks.Add(ConnectionTask(test, ops));
                }
                await Task.WhenAll(tasks);
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
            Console.WriteLine();
        }

        private static async Task ConnectionTask(Func<AppDb, Task> cb, int ops)
        {
            using (var db = new AppDb())
            {
                for (var op = 0; op < ops; op++)
                {
                    await cb(db);
                }
            }
        }

    }

}
