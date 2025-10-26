namespace Techcore_Internship.ConsoleApp.Module_1;

public class Task335_8_InheritancePolymorphism
{
    public static void Run()
    {
        Console.WriteLine("\nTask335.8 - Наследование и Полиморфизм\n");

        List<Shape> shapes = new List<Shape>
        {
            new Circle(5),
            new Rectangle(4, 6),
            new Circle(3),
            new Rectangle(2, 8)
        };

        int counter = 1;
        foreach (var shape in shapes)
        {
            Console.WriteLine($"Фигура {counter++} - {shape.Name}, площадь = {shape.GetArea()}");
        }
        Console.WriteLine($"Общая площадь фигур = {shapes.GetTotalArea()}");
        Console.WriteLine(new string('-', 30));
    }

    public abstract class Shape
    {
        public abstract string Name { get; }
        public abstract double GetArea();
    }

    public class Circle : Shape
    {
        public override string Name { get; } = "Circle";
        public double Radius { get; set; }
        public Circle(double radius)
        {
            Radius = radius;
        }
        public override double GetArea()
        {
            return Math.PI * Radius * Radius;
        }
    }

    public class Rectangle : Shape
    {
        public override string Name { get; } = "Rectangle";
        public double Width { get; set; }
        public double Height { get; set; }
        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }
        public override double GetArea()
        {
            return Width * Height;
        }
    }
}

public static class ShapeExtensions
{
    public static double GetTotalArea(this IEnumerable<Task335_8_InheritancePolymorphism.Shape> shapes)
        => shapes.Sum(shape => shape.GetArea());
}