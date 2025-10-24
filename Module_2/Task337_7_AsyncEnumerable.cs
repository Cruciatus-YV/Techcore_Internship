namespace Techcore_Internship.Module_2;

public static class Task337_7_AsyncEnumerable
{
    public static async Task Run()
    {
        Console.WriteLine("\nTask337.7 - Async Enumerable\n");
        await foreach (var number in GetNumbersAsync())
        {
            Console.WriteLine(number);
        }
        Console.WriteLine(new string('-', 30));
    }
    public static async IAsyncEnumerable<int> GetNumbersAsync()
    {
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(500);
            yield return i;
        }
    }
}
