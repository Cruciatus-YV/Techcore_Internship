using Techcore_Internship.ConsoleApp.Module_1;

namespace Techcore_Internship.Tests
{
    public class CalculatorTests
    {
        [Fact]
        public void Add_TwoNumbers_Returns_CorrectValue()
        {
            // Arrange
            double a = 2;
            double b = 3;
            double expected = 5;

            // Act
            double actual = Task335_3_Calculator.Add(a, b);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Multiply_TwoNumbers_Returns_CorrectValue()
        {
            // Arrange
            double a = 2;
            double b = 3;
            double expected = 6;

            // Act
            double actual = Task335_3_Calculator.Multiply(a, b);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Divide_TwoNumbers_Returns_CorrectValue()
        {
            // Arrange
            double a = 6;
            double b = 3;
            double expected = 2;

            // Act
            double actual = Task335_3_Calculator.Divide(a, b);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Subtract_TwoNumbers_Returns_CorrectValue()
        {
            // Arrange
            double a = 5;
            double b = 3;
            double expected = 2;

            // Act
            double actual = Task335_3_Calculator.Subtract(a, b);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Divide_ByZero_ThrowsDivideByZeroException()
        {
            // Arrange
            double a = 5;
            double b = 0;

            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => Task335_3_Calculator.Divide(a, b));
        }
    }
}
