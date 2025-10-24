namespace Techcore_Internship.Module_1;

public class Task335_9_Intarfaces
{
    public static void Run()
    {
        Console.WriteLine("\nTask335.9 - Интерфейсы\n");

        ConsoleLogger consoleLogger = new ConsoleLogger();
        FileLogger fileLogger = new FileLogger();

        consoleLogger.Log("Это сообщение для консоли.");
        fileLogger.Log("Это сообщение для \"файла\".");
        Console.WriteLine(new string('-', 30));
    }

    public interface ILoggable
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILoggable
    {
        public void Log(string message)
        {
            Console.WriteLine($"[LOG]: {message}");
        }
    }

    public class FileLogger : ILoggable
    {
        public void Log(string message)
        {
            // Представим, что это файл, мы же интерфейс тренируемся использовать xD 
            Console.WriteLine($"[FILE LOG]: {message}");
        }
    }
}