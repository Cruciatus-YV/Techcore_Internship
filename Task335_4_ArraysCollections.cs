using System;

public static class Task335_4_ArraysCollections
{
	public static void Run()
    {
        Console.WriteLine("\nTask 335.4 - Работа с массивами и коллекциями");
        int[] numbers = { 12, 45, 7, 23, 56, 89, 3, 67, 34, 91 };
        Console.WriteLine("\nИсходный массив чисел: ");
        PrintArray(numbers);

        int maxNumber = FindMaxElement(numbers);
        Console.WriteLine($"Максимальный элемент: {maxNumber}");


        List<string> strings = new List<string> { "apple", "cat", "elephant", "dog", "butterfly", "hi", "programming" };
        Console.WriteLine("Исходный массив строк: ");
        PrintList(strings);

        List<string> filteredStrings = FilterStringsByLength(strings, 5);
        Console.WriteLine("Отфильтрованные строки:");
        PrintList(filteredStrings);
        Console.WriteLine(new string('-', 30));
    }

    static void PrintArray(int[] array)
    {
        Console.WriteLine(string.Join(", ", array));
    }

    static void PrintList(List<string> list)
    {
        if (list.Count == 0)
        {
            Console.WriteLine("Список пуст");
            return;
        }

        foreach (string item in list)
        {
            Console.WriteLine($"- \"{item}\" (длина: {item.Length})");
        }
    }

    static int FindMaxElement(int[] array)
    {
        return array.Max();
    }

    static List<string> FilterStringsByLength(List<string> stringList, int minLength)
    {
        if (stringList == null)
            throw new ArgumentException("Список не может быть null");

        List<string> result = new List<string>(stringList.Count);

        for (int i = 0; i < stringList.Count; i++)
        {
            string str = stringList[i];
            if (str != null && str.Length > minLength)
            {
                result.Add(str);
            }
        }

        return result;
    }
}