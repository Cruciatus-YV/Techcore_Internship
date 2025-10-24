namespace Techcore_Internship.Module_1;

public static class Task335_6_OOP
{
    public static void Run()
    {
        Console.WriteLine("\nTask335.6 - Основы ООП: Классы и Объекты");

        try
        {
            Book book1 = new Book("Преступление и наказание", "Фёдор Достоевский", 1866);
            Book book2 = new Book("1984", "Джордж Оруэлл", 1949);
            Book book3 = new Book("Мастер и Маргарита", "Михаил Булгаков", 1967);

            Console.WriteLine("\nСозданные объекты Book:");

            book1.DisplayInfo();
            book2.DisplayInfo();
            book3.DisplayInfo();

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
        Console.WriteLine(new string('-', 30));
    }
}

public class Book
{
    private string _title;
    private string _author;
    private int _year;

    public string Title => _title;
    public string Author => _author;
    public int Year => _year;

    public Book(string title, string author, int year)
    {
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название книги не может быть пустым");
            if (string.IsNullOrWhiteSpace(author))
                throw new ArgumentException("Автор не может быть пустым");
            if (year < 1450 || year > DateTime.Now.Year + 1)
                throw new ArgumentException($"Год издания должен быть между 1450 и {DateTime.Now.Year + 1}");

            _title = title;
            _author = author;
            _year = year;
        }
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"\"{Title}\" - {Author} ({Year}г.)");
    }
}