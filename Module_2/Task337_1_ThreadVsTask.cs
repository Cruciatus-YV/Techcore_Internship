namespace Techcore_Internship.ConsoleApp.Module_2;

public static class Task337_1_ThreadVsTask
{
    public static void Run()
    {
        Console.WriteLine("\nTask337.1 - Thread vs Task\n");
        Console.WriteLine("Запуск через new Thread()");
        var sw = System.Diagnostics.Stopwatch.StartNew();

        var threads = new Thread[1000];

        for (int i = 0; i < threads.Length; i++)
        {
            threads[i] = new Thread(() => Thread.Sleep(10));
            threads[i].Start();
        }

        foreach (var t in threads)
            t.Join();

        sw.Stop();
        Console.WriteLine($"Время выполнения (Thread): {sw.ElapsedMilliseconds} мс\n");


        Console.WriteLine("Запуск через Task.Run()");
        sw.Restart();

        var tasks = new Task[1000];

        for (int i = 0; i < tasks.Length; i++)
        {
            tasks[i] = Task.Run(() => Thread.Sleep(10));
        }

        Task.WaitAll(tasks);

        sw.Stop();
        Console.WriteLine($"Время выполнения (Task): {sw.ElapsedMilliseconds} мс");
        Console.WriteLine(new string('-', 30));
    }
}
