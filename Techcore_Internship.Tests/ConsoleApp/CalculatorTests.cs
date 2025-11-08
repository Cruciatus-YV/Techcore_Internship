using Techcore_Internship.ConsoleApp.Module_1;

namespace Techcore_Internship.UnitTests.ConsoleApp
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

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(0, 0, 0)]
        [InlineData(-1, 5, 4)]
        [InlineData(2.5, 3.5, 6)]
        public void Add_MultipleNumbers_Returns_CorrectValues(double a, double b, double expected)
        {
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

        [Theory]
        [InlineData(2, 3, 6)]
        [InlineData(0, 5, 0)]
        [InlineData(-2, 4, -8)]
        [InlineData(2.5, 4, 10)]
        public void Multiply_MultipleNumbers_Returns_CorrectValues(double a, double b, double expected)
        {
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

        [Theory]
        [InlineData(6, 3, 2)]
        [InlineData(7.5, 2.5, 3)]
        [InlineData(-10, 2, -5)]
        public void Divide_MultipleNumbers_Returns_CorrectValues(double a, double b, double expected)
        {
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

        [Theory]
        [InlineData(5, 3, 2)]
        [InlineData(0, 0, 0)]
        [InlineData(-3, -1, -2)]
        [InlineData(7.5, 2.5, 5)]
        public void Subtract_MultipleNumbers_Returns_CorrectValues(double a, double b, double expected)
        {
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

        [Theory]
        [InlineData(10, 0)]
        [InlineData(-3, 0)]
        [InlineData(0, 0)]
        public void Divide_ByMultipleZeros_ThrowsDivideByZeroException(double a, double b)
        {
            // Act & Assert
            Assert.Throws<DivideByZeroException>(() => Task335_3_Calculator.Divide(a, b));
        }
    }
}