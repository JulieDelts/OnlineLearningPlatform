using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Tests.TestCases
{
    public static class CoursesServiceTestCases
    {
        public static IEnumerable<object[]> CourseToCreate()
        {
            var courseModel = new CreateCourseModel()
            {
                Name = "TestCourse",
                Description = "This is a test course.",
                NumberOfLessons = 10,
                TeacherId = Guid.NewGuid()
            };

            yield return new object[] { courseModel };
        }

        public static IEnumerable<object[]> CourseWithFullInfo()
        {
            var courseDTO = new Course()
            {
                Id = Guid.NewGuid(),
                Name = "TestCourse",
                Description = "This is a test course.",
                NumberOfLessons = 20,
                TeacherId = Guid.NewGuid(),
                IsDeactivated = false,
                Teacher = new User()
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "test1@gmail.com"
                }
            };

            yield return new object[] { courseDTO };
        }

        public static IEnumerable<object[]> CourseToUpdate()
        {
            var courseModel = new UpdateCourseModel()
            {
                Name = "NewCourseName",
                Description = "This is a new course description.",
                NumberOfLessons = 10
            };

            yield return new object[] { courseModel };
        }

        public static IEnumerable<object[]> ActiveCourses()
        {
            var courseDTOs = new List<Course>()
            {
                new Course { Name = "FirstTestCourse", Description = "This is the first test course." },
                new Course { Name = "SecondTestCourse", Description = "This is the second test course." },
                new Course { Name = "ThirdTestCourse", Description = "This is the third test course." },
                new Course { Name = "FourthTestCourse", Description = "This is the fourth test course." }
            };

            yield return new object[] { courseDTOs };
        }

        public static IEnumerable<object[]> EnrollmentsByCourseId()
        {
            var enrollmentDTOs = new List<Enrollment>()
            {
                new Enrollment()
                {
                    User = new User()
                    {
                        FirstName = "John",
                        LastName = "Doe",
                        Email = "test1@gmail.com"
                    },
                    Attendance = 1,
                    Grade = null
                },
                new Enrollment()
                {
                    User = new User()
                    {
                        FirstName = "Kate",
                        LastName = "Beck",
                        Email = "test2@gmail.com"
                    },
                    Attendance = null,
                    Grade = 5
                }
            };

            yield return new object[] { enrollmentDTOs };
        }
    }
}
