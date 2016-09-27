using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitter.App.BusinessLogic;

namespace CalculateGPA
{
    class Program
    {
        static void Main(string[] args)
        {
            var gpaCalculator = new GPACalculator();

            var courses = new List<GPACalculator.Course>
            {
                new GPACalculator.Course
                {
                    Score = 82,
                    Credit = 2.0
                },
                new GPACalculator.Course
                {
                    Score = 68,
                    Credit = 1.0
                },
                new GPACalculator.Course
                {
                    Score = 76,
                    Credit = 4.0
                },
                new GPACalculator.Course
                {
                    Score = 86,
                    Credit = 6.0
                },
                new GPACalculator.Course
                {
                    Score = 88,
                    Credit = 4.0
                },
                new GPACalculator.Course
                {
                    Score = 77,
                    Credit = 1.0
                },
                new GPACalculator.Course
                {
                    Score = 80,
                    Credit = 1.0
                },
                new GPACalculator.Course
                {
                    Score = 84,
                    Credit = 1.0
                },
                new GPACalculator.Course
                {
                    Score = 84,
                    Credit = 3.0
                },
                new GPACalculator.Course
                {
                    Score = 75,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 92,
                    Credit = 2
                },
                new GPACalculator.Course
                {
                    Score = 69,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 66,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 69,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 80,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 71,
                    Credit = 4
                },
                new GPACalculator.Course
                {
                    Score = 68,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 86,
                    Credit = 4
                },
                new GPACalculator.Course
                {
                    Score = 92,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 89,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 68,
                    Credit = 2
                },
                new GPACalculator.Course
                {
                    Score = 83,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 88,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 76,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 90,
                    Credit = 0.5
                },
                new GPACalculator.Course
                {
                    Score = 71,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 66,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 75,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 73,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 77,
                    Credit = 2
                },
                new GPACalculator.Course
                {
                    Score = 86,
                    Credit = 0.5
                },
                new GPACalculator.Course
                {
                    Score = 75,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 89,
                    Credit = 2
                },
                new GPACalculator.Course
                {
                    Score = 83,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 76,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 88,
                    Credit = 2
                },
                new GPACalculator.Course
                {
                    Score = 72,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 68,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 89,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 75,
                    Credit = 2
                },
                new GPACalculator.Course
                {
                    Score = 80,
                    Credit = 6
                },
                new GPACalculator.Course
                {
                    Score = 89,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 94,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 81,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 86,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 87,
                    Credit = 2
                },
                new GPACalculator.Course
                {
                    Score = 71,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 76,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 79,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 85,
                    Credit = 1
                },
                new GPACalculator.Course
                {
                    Score = 68,
                    Credit = 3
                },
                new GPACalculator.Course
                {
                    Score = 79,
                    Credit = 2
                }
            };

            gpaCalculator.AddCourse(courses);

            var result = gpaCalculator.GetOptimization(5);

            foreach (var item in result)
            {
                Console.WriteLine($"Grade: {item.Score}  Credit: {item.Credit}");
            }
        }
    }
}
