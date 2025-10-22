using System;

public static class Task335_7_RefValTypes
{
	public static void Run()
	{
		var myClass = new MyClass() { Value = 100 };
		Console.WriteLine($"Исходное myClass.Value: {myClass.Value}");
		Modify(myClass);
        Console.WriteLine($"Значение myClass.Value после вызова метода Modify(myClass): {myClass.Value}");

        var myStruct = new MyStruct() { Value = 100 };
        Console.WriteLine($"\nИсходное myStruct.Value: {myStruct.Value}");
        Modify(myStruct);
        Console.WriteLine($"Значение myStruct.Value после вызова метода Modify(myStruct): {myStruct.Value}");
        Console.WriteLine("\nОбъяснение: класс в C# является ссылочным типом. Это означает, что при создании экземпляра класса, объект размещается в куче, а в переменной хранится ссылка на этот объект. При передаче такой переменной в метод, в параметр метода копируется ссылка на тот же самый объект в куче. Как следствие, любые изменения, внесенные в объект внутри метода, затрагивают оригинальный объект.\nВ случае со структурой - она является значимым типом. Это значит, что при передаче в метод передается не оригинальная структура, а её полная копия. Все изменения внутри метода применяются только к этой копии и не влияют на оригинальную структуру.");
        Console.WriteLine(new string('-', 30));
    }

    public interface IHasValue
    {
        int Value { get; set; }
    }

    public class MyClass : IHasValue
    {
        public int Value { get; set; }
    }

    public struct MyStruct : IHasValue
    {
        public int Value { get; set; }
    }

    public static void Modify<T>(T item) where T : IHasValue
    {
        item.Value += 10;
    }
}
