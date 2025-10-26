namespace Techcore_Internship.ConsoleApp.Module_1;

public static class Task335_4_ArraysCollections
{
    public static void Run()
    {
        Console.WriteLine("\nTask 335.4 - Работа с массивами и коллекциями");
        int[] numbers = [12, 45, 7, 23, 56, 89, 3, 67, 34, 91];
        Console.WriteLine("\nИсходный массив чисел: ");
        PrintArray(numbers);

        int maxNumber = FindMaxElement(numbers);
        Console.WriteLine($"Максимальный элемент: {maxNumber}");


        List<string> strings = ["apple", "cat", "elephant", "dog", "butterfly", "hi", "programming"];
        Console.WriteLine("Исходный массив строк: ");
        PrintList(strings);

        var filteredStrings = FilterStringsByLength(strings, 5).ToArray();
        Console.WriteLine("Отфильтрованные строки:");
        PrintList(filteredStrings);
        Console.WriteLine(new string('-', 30));
    }

    static void PrintArray(int[] array)
    {
        Console.WriteLine(string.Join(", ", array));
    }

    static void PrintList(IReadOnlyCollection<string> list)
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

    static int FindMaxElement(int[] array) => array.Max();

    static IEnumerable<string> FilterStringsByLength(List<string> stringList, int minLength)
    {
        foreach (var item in stringList)
            if (item.Length > minLength)
                yield return item;
    }
}