using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace Student.Management.Tests
{
    public class SimpleClassTests
    {
        public class Class
        {
            public List<string> Students { get; set; } = new List<string>();
            public List<double> Grades { get; set; } = new List<double>();

        }

        [Fact]
        public void TotalStudents_Count_IsCorrect()
        {
            // Arrange
            var myClass = new Class();
            myClass.Students.AddRange(new[] { "Alice", "Bob", "Charlie" });

            // Act
            var total = myClass.Students.Count;

            // Assert
            Assert.Equal(3, total);
        }
        [Fact]
        public void AverageGrade_Calculation_IsCorrect()
        {
            // Arrange
            var myClass = new Class();
            myClass.Grades.AddRange(new[] { 8.0, 7.5, 9.0 });

            // Act: tính điểm trung bình
            var average = myClass.Grades.Average();

            // Assert
            Assert.Equal(8.166666666666666, average, 5); 
        }

    }
}
