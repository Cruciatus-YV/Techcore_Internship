using System;

public static class Task335_5_LINQ
{
	public static void Run()
	{
		Console.WriteLine("\nTask335.5 - Введение в LINQ");

		List<int> numbers = [ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 ];

		var processedNumbers = numbers.Where(n => n % 2 == 0).Select(n => n * 2).OrderByDescending(n => n); 

		Console.WriteLine($"\nИсходный массив: {string.Join(", ", numbers)}");
		Console.WriteLine($"Результат: {string.Join(", ", processedNumbers)}");
		Console.WriteLine(new string('-', 30));
	}
}
