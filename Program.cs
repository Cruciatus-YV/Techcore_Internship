Console.WriteLine("Task 335.1");
Console.WriteLine("Hello, World!\n");

Console.WriteLine("Task 335.2 - базовые конструкции языка C#");
Console.WriteLine("Введите целое число: ");
Int32.TryParse(Console.ReadLine(), out int numberA);

Console.WriteLine("Введите десятичную дробь (дробная доля через запятую): ");
Double.TryParse(Console.ReadLine(), out double numberB); ;

Console.WriteLine("Сравниваем введённые значения: ");
Thread.Sleep(1000);

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

    default:
        break;
}

Thread.Sleep(500);

Console.WriteLine("Проверим работу цикла for :");
Console.WriteLine("Введите количество итераций от 1 до 10: ");

Int32.TryParse(Console.ReadLine(), out int n);

for (int i = 1; i <= n; i++)
{
    Console.WriteLine($"Цикл for - итерация {i}");
    Thread.Sleep(500);
}

Thread.Sleep(500);

Console.WriteLine("Теперь проверим работу цикла while: ");
Console.WriteLine("Введите количество итераций от 1 до 10: ");

Int32.TryParse(Console.ReadLine(), out int w);

int index = 0;
while (w > 0)
{
    index++;
    Console.WriteLine($"Цикл while - итерация {index}");
    w--;
    Thread.Sleep(500);
}

Console.WriteLine("\nTask 335.3 - консольный калькулятор");
try
{
    Console.WriteLine("Введите первый операнд: ");
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
finally
{
    Console.ReadLine();
}

static void Calculator(double a, double b, char operation)
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

// Статические методы для математических операций
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