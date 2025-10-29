namespace Techcore_Internship.ConsoleApp.Module_1;


public static class Task335_2_BasicConstructs
{
    public static void Run()
    {
        Console.WriteLine("\nTask 335.2 - базовые типы и конструкции языка C#");
        Console.WriteLine("\nВведите целое число: ");
        Int32.TryParse(Console.ReadLine(), out int numberA);

        Console.WriteLine("Введите десятичную дробь (дробная доля через запятую): ");
        Double.TryParse(Console.ReadLine(), out double numberB); ;

        Console.WriteLine("Сравниваем введённые значения: ");

        bool isEquals = false;
        string result = string.Empty;
        double winner = 0;

        if (numberA > numberB)
        {
            result = "Первое число больше второго.";
            winner = numberA;
        }
        else if (numberA < numberB)
        {
            result = "Второе число больше первого.";
            winner = numberB;
        }
        else
        {
            isEquals = true;
            result = "Числа равны.";
        }
        switch (isEquals)
        {
            case true:
                Console.WriteLine(result + " Победитель не определён."); ;
                break;

            case false:
                Console.WriteLine(result + $" Победитель - число {winner}.");
                break;
        }

        Console.WriteLine("Проверим работу цикла for :");
        Console.WriteLine("Введите количество итераций от 1 до 10: ");

        Int32.TryParse(Console.ReadLine(), out int n);

        for (int i = 1; i <= n; i++)
        {
            Console.WriteLine($"Цикл for - итерация {i}");
        }

        Console.WriteLine("Теперь проверим работу цикла while: ");
        Console.WriteLine("Введите количество итераций от 1 до 10: ");

        Int32.TryParse(Console.ReadLine(), out int w);

        int index = 0;
        while (w > 0)
        {
            index++;
            Console.WriteLine($"Цикл while - итерация {index}");
            w--;
        }
        Console.WriteLine(new string('-', 30));
    }
}