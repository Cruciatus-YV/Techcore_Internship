namespace Techcore_Internship.ConsoleApp.Module_2;

public static class Task337_2_AsyncAwait
{
    public static async Task Run()
    {
        Console.WriteLine("\nTask337.2 - Async/Await\n");
        var taskResult = await DownloadDataAsync();
        Console.WriteLine(taskResult);
        Console.WriteLine(new string('-', 30));
    }
    public static async Task<string> DownloadDataAsync()
    {
        await Task.Delay(2000);
        
        return "Задача была выполнена";
    }
}
