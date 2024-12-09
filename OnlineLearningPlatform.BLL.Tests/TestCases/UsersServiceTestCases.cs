
using OnlineLearningPlatform.BLL.BusinessModels;
using OnlineLearningPlatform.Core;
using OnlineLearningPlatform.DAL.DTOs;

namespace OnlineLearningPlatform.BLL.Tests.TestCases
{
    public static class UsersServiceTestCases
    {
        public static IEnumerable<object[]> UserToRegister()
        {
            var userModel = new UserRegistrationModel()
            {
                FirstName = "John",
                LastName = "Doe",
                Login = "JohnDoeTest",
                Password = "Password",
                Email = "JohnDoeTest@gmail.com",
                Phone = "89121234567"
            };

            yield return new object[] { userModel };
        }

        public static IEnumerable<object[]> UserFullInfo()
        {
            var userDTO = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = "TestFirstName",
                LastName = "TestSecondName",
                Email = "EmailTest",
                Phone = "78901234567",
                Role = Role.Admin,
                IsDeactivated = false
            };

            yield return new object[] { userDTO };
        }

        public static IEnumerable<object[]> UserToUpdate()
        {
            var userProfileModel = new UpdateUserProfileModel()
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "JohnDoeTest@gmail.com",
                Phone = "89121234567"
            };

            yield return new object[] { userProfileModel };
        }

        public static IEnumerable<object[]> ActiveUsers()
        {
            var usersDTOs = new List<User>()
            {
                new User { FirstName = "FirstTestName1", LastName = "LastTestName1", Email = "TestEmail1" },
                new User { FirstName = "FirstTestName2", LastName = "LastTestName2", Email = "TestEmail2" },
                new User { FirstName = "FirstTestName3", LastName = "LastTestName3", Email = "TestEmail3" }
            };

            yield return new object[] { usersDTOs };
        }

        public static IEnumerable<object[]> UserTaughtCourses()
        {
            var courseDTOs = new List<Course>()
            {
                new Course { Name = "TestCourse1", Description = "This is the first test course." },
                new Course { Name = "TestCourse2", Description = "This is the second test course." }
            };

            yield return new object[] { courseDTOs };
        }

        public static IEnumerable<object[]> UserEnrollments()
        {
            var enrollmentDTOs = new List<Enrollment>()
            {
                new Enrollment
                {
                    Attendance = 10,
                    Grade = 5,
                    StudentReview = "TestReview1",
                    Course = new Course()
                    {
                        Name = "TestCourse1",
                        Description = "This is the first test course description."
                    }
                },
                new Enrollment
                {
                    Attendance = 12,
                    Grade = 6,
                    StudentReview = null,
                    Course = new Course()
                    {
                        Name = "TestCourse2",
                        Description = "This is the second test course description."
                    }
                }
            };

            yield return new object[] { enrollmentDTOs };
        }
    }
}
