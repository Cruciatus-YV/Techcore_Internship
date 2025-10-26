namespace Techcore_Internship.ConsoleApp.Module_2;

public static class Task337_4_TaskWhenAll
{
    public static async Task Run()
    {
        Console.WriteLine("\nTask337.4 - Task.WhenAll\n");

        await Task.WhenAll(DelayAndPrint3(), DelayAndPrint2(), DelayAndPrint1());

        Console.WriteLine(new string('-', 30));
    }
    public static async Task<int> DelayAndPrint1()
    {
        Console.WriteLine($"Задача с задержкой 1000 стартовала.");
        await Task.Delay(1000);
        Console.WriteLine($"Задача с задержкой 1000 мс выполнена.");
        return 1;
    }
    public static async Task<int> DelayAndPrint2()
    {
        Console.WriteLine($"Задача с задержкой 1500 стартовала.");
        await Task.Delay(1500);
        Console.WriteLine($"Задача с задержкой 1500 мс выполнена.");
        return 1;
    }
    public static async Task<int> DelayAndPrint3()
    {
        Console.WriteLine($"Задача с задержкой 2000 стартовала.");
        await Task.Delay(2000);
        Console.WriteLine($"Задача с задержкой 2000 мс выполнена.");
        return 1;
    }
}
