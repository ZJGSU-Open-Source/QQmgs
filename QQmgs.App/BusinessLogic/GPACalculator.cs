using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Twitter.App.BusinessLogic
{
    public class GPACalculator
    {
        private readonly List<Course> _courses = new List<Course>();

        public void AddCourse(Course course)
        {
            _courses.Add(course);
        }

        public void AddCourse(List<Course> courses)
        {
            _courses.AddRange(courses);
        }

        public List<Course> GetOptimization(int skipCourseNumber = 0)
        {
            switch (skipCourseNumber)
            {
                case 1:
                    return SkipOneCourseNumber(_courses);
                case 2:
                    return SkipTwoCourseNumber(_courses);
                case 3:
                    return SkipThreeCourseNumber(_courses);
                case 4:
                    return SkipFourCourseNumber(_courses);
                case 5:
                    return SkipFiveCourseNumber(_courses);
                default:
                    return SkipZeroCourseNumber(_courses);
            }
        }

        private List<Course> SkipZeroCourseNumber(List<Course> courses)
        {
            return null;
        }
        
        private List<Course> SkipOneCourseNumber(List<Course> courses)
        {
            return new List<Course> {GetMinGPAFromCourse(courses)};
        }

        private List<Course> SkipTwoCourseNumber(List<Course> courses)
        {
            var firstMin = GetMinGPAFromCourse(courses);
            courses.Remove(firstMin);

            var secondMin = GetMinGPAFromCourse(courses);
            return new List<Course> {firstMin, secondMin};
        }

        private List<Course> SkipThreeCourseNumber(List<Course> courses)
        {
            var firstMin = GetMinGPAFromCourse(courses);
            courses.Remove(firstMin);

            var secondMin = GetMinGPAFromCourse(courses);
            courses.Remove(secondMin);

            var thirdMin = GetMinGPAFromCourse(courses);

            return new List<Course> { firstMin, secondMin, thirdMin };
        }

        private List<Course> SkipFourCourseNumber(List<Course> courses)
        {
            var firstMin = GetMinGPAFromCourse(courses);
            courses.Remove(firstMin);

            var secondMin = GetMinGPAFromCourse(courses);
            courses.Remove(secondMin);

            var thirdMin = GetMinGPAFromCourse(courses);
            courses.Remove(thirdMin);

            var forthMin = GetMinGPAFromCourse(courses);

            return new List<Course> { firstMin, secondMin, thirdMin, forthMin };
        }
        private List<Course> SkipFiveCourseNumber(List<Course> courses)
        {
            var firstMin = GetMinGPAFromCourse(courses);
            courses.Remove(firstMin);

            var secondMin = GetMinGPAFromCourse(courses);
            courses.Remove(secondMin);

            var thirdMin = GetMinGPAFromCourse(courses);
            courses.Remove(thirdMin);

            var forthMin = GetMinGPAFromCourse(courses);
            courses.Remove(forthMin);

            var fifthMin = GetMinGPAFromCourse(courses);

            return new List<Course> { firstMin, secondMin, thirdMin, forthMin, fifthMin };
        }

        private static Course GetMinGPAFromCourse(List<Course> courses)
        {
            double averageScore = 0.0;
            var index = -1;

            for (int i = 0; i < courses.Count(); ++i)
            {
                double totalCredit = 0.0;
                double sum = 0;
                for (int j = 0; j < courses.Count(); ++j)
                {
                    if (i == j) continue;

                    sum += courses[j].Credit*courses[j].Score;
                    totalCredit += courses[j].Credit;
                }

                double currentAverageScore = sum/totalCredit;
                if (currentAverageScore > averageScore)
                {
                    averageScore = currentAverageScore;
                    index = i;
                }
            }

            return courses[index];
        }

        public class Course
        {
            public string Name { get; set; }

            public double Score { get; set; }

            public double Credit { get; set; }
        }

    }
}