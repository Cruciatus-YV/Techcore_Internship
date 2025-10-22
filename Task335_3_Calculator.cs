public static class Task335_3_Calculator
{
    public static void Run()
    {
        Console.WriteLine("\nTask 335.3 - консольный калькулятор");
        try
        {
            Console.WriteLine("\nВведите первый операнд: ");
            if (!Double.TryParse(Console.ReadLine(), out double operand1))
            {
                throw new ArgumentException("Ошибка: введено некорректное число для первого операнда.");
            }

            Console.WriteLine("Введите второй операнд: ");
            if (!Double.TryParse(Console.ReadLine(), out double operand2))
            {
                throw new ArgumentException("Ошибка: введено некорректное число для второго операнда.");
            }

            Console.WriteLine("Введите операцию (+, -, *, /): ");
            char operation = Console.ReadKey().KeyChar;
            Console.WriteLine();

            Calculator(operand1, operand2, operation);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        Console.WriteLine(new string('-', 30));
    }

    public static void Calculator(double a, double b, char operation)
    {
        try
        {
            double result = operation switch
            {
                '+' => Add(a, b),
                '-' => Subtract(a, b),
                '*' => Multiply(a, b),
                '/' => Divide(a, b),
                _ => throw new ArgumentException("Ошибка: недопустимая операция.")
            };

            Console.WriteLine($"{a} {operation} {b} = {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static double Add(double a, double b)
    {
        return a + b;
    }

    static double Subtract(double a, double b)
    {
        return a - b;
    }

    static double Multiply(double a, double b)
    {
        return a * b;
    }

    static double Divide(double a, double b)
    {
        if (b == 0)
            throw new DivideByZeroException("Ошибка: деление на ноль невозможно.");

        return a / b;
    }
}