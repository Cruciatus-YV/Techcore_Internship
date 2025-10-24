namespace Techcore_Internship.Module_1;

public class Task335_10_Record
{
    public static void Run()
    {
        Console.WriteLine("\nTask335.10 - Records\n");

        var book1 = new Book("451° по Фаренгейту", "Рэй Брэдбери", 1953);
        var book2 = new Book("Мастер и Маргарита", "Михаил Булгаков", 1967);
        var book3 = new Book("Анна Каренина", "Лев Толстой", 1877);

        Console.WriteLine("Созданные объекты Book (Records):");

        book1.DisplayInfo();
        book2.DisplayInfo();
        book3.DisplayInfo();

        Console.WriteLine(new string('-', 30));
    }

    public record Book(string Title, string Author, int Year)
    {
        public string Title { get; init; } =
            !string.IsNullOrWhiteSpace(Title)
                ? Title
                : throw new ArgumentException("Название книги не может быть пустым");

        public string Author { get; init; } =
            !string.IsNullOrWhiteSpace(Author)
                ? Author
                : throw new ArgumentException("Автор не может быть пустым");

        public int Year { get; init; } =
            (Year >= 1450 && Year <= DateTime.Now.Year + 1)
                ? Year
                : throw new ArgumentException($"Год издания должен быть между 1450 и {DateTime.UtcNow.Year + 1}");

        public void DisplayInfo()
        {
            Console.WriteLine($"\"{Title}\" - {Author} ({Year}г.)");
        }
    }
}