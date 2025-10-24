using System.Data;

namespace Techcore_Internship.Module_1;

public static class Task335_3_Calculator
{
    public static void Run()
    {
        Console.WriteLine("\nTask 335.3 - консольный калькулятор");

        bool aIsValid = false;
        bool bIsValid = false; 
        bool operationIsValid = false;

        double a = 0;
        double b = 0;
        char operation = ' ';

        while (!aIsValid)
        {
            Console.WriteLine("\nВведите первый операнд: ");
            if(Double.TryParse(Console.ReadLine(), out a))
                aIsValid = true;
        }

        while (!operationIsValid)
        {
            Console.WriteLine("\nВведите операцию (+ - / *): ");

            char? o = Console.ReadLine() switch
            {
                "+" => '+',
                "-" => '-',
                "*" => '*',
                "/" => '/',
                _ => null
            };
            if (o != null)
            {
                operation = o.Value;
                operationIsValid = true;
            }
        }

        while (!bIsValid)
        {
            Console.WriteLine("\nВведите второй операнд: ");
            if (Double.TryParse(Console.ReadLine(), out b))
                bIsValid = true;
        }

        var result = Calculator(a, b, operation);
        Console.WriteLine($"\n{a} {operation} {b} = {result}");
        Console.WriteLine(new string('-', 30));
    }

    public static double Calculator(double a, double b, char operation)
    {
        try
        {
            return operation switch
            {
                '+' => Add(a, b),
                '-' => Subtract(a, b),
                '*' => Multiply(a, b),
                '/' => Divide(a, b),
                _ => throw new ArgumentException("\nОшибка: недопустимая операция.")
            };
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("\nОшибка: деление на ноль.");
            return double.NaN;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return double.NaN;
        }
        
    }

    static double Add(double a, double b) => a + b;

    static double Subtract(double a, double b) => a - b;

    static double Multiply(double a, double b) => a * b;

    static double Divide(double a, double b)
    {
        if (b == 0)
            throw new DivideByZeroException();
        return a / b;
    }

    public static void Run2()
    {
        Console.WriteLine("\nВведите выражение: ");
        var input = Console.ReadLine();

        try
        {
            var result = new DataTable().Compute(input, null);
            Console.WriteLine($"{input} = {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        Console.WriteLine(new string('-', 30));
    }
}