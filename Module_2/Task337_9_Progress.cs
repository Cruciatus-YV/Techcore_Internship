namespace Techcore_Internship.Module_2;

public static class Task337_9_Progress
{
    public static async Task Run()
    {
        Console.WriteLine("\nTask337.9 - Progress Reporting (Progress Bar)\n");
        var progress = new Progress<int>();
        progress.ProgressChanged += (s, e) =>
        {
            Console.Write($"\rПрогресс - {e}%");
        };


        await LongRunningOperationAsync(progress);

        Console.WriteLine("\nОперация завершена!");
        Console.WriteLine(new string('-', 30));

    }

    public static async Task LongRunningOperationAsync(IProgress<int> progress)
    {
        for (int i = 1; i <= 100; i++)
        {
            await Task.Delay(50);
            progress?.Report(i);
        }
    }
}
