namespace Techcore_Internship.ConsoleApp.Module_2;

public static class Task337_5_CancellationToken
{
    public static async Task Run()
    {
        Console.WriteLine("\nTask337.5 - CancellationToken\n");
        using var cts = new CancellationTokenSource();
        var token = cts.Token;
        var task = Task.Run(() => DoWork(token), token);
        Console.WriteLine("Нажмите любую клавишу для отмены задачи...");
        Console.ReadKey();
        Console.WriteLine();
        cts.Cancel();
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Задача была отменена.");
        }
        Console.WriteLine(new string('-', 30));
    }
    public static void DoWork(CancellationToken token)
    {
        for (int i = 0; i < 100; i++)
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("Была запрошена отмена");
                token.ThrowIfCancellationRequested();
            }
            Console.WriteLine($"Выполнение работы... {i + 1}/10");
            Thread.Sleep(500);
        }
    }
}
