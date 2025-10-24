namespace Techcore_Internship.Module_2;

public static class Task337_6_AsyncError
{
    public static async Task Run()
    {
        Console.WriteLine("\nTask337.6 - Async Error Handling\n");
        try
        {
            await ExceptionTask();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
    public static async Task ExceptionTask()
    {
        for (int i = 1; i <= 20; i++)
        {
            Console.WriteLine(i);
            await Task.Delay(500);
            if (i == 5)
                throw new InvalidOperationException("Произошла ошибка в асинхронной задаче на итерации 10.");
        }
    }
}
