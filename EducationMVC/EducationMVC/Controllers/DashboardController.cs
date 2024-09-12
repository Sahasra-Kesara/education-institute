using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace EducationMVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly string _connectionString;

        public DashboardController(IConfiguration configuration)
        {
            // Get the connection string from appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IActionResult> Index()
        {
            var dashboardData = new DashboardViewModel();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Get the number of Teachers
                var teachersQuery = "SELECT COUNT(*) FROM Teachers";
                using (var command = new SqlCommand(teachersQuery, connection))
                {
                    dashboardData.NumberOfTeachers = (int)await command.ExecuteScalarAsync();
                }

                // Get the number of Students
                var studentsQuery = "SELECT COUNT(*) FROM Students";
                using (var command = new SqlCommand(studentsQuery, connection))
                {
                    dashboardData.NumberOfStudents = (int)await command.ExecuteScalarAsync();
                }

                // Get the number of Courses
                var coursesQuery = "SELECT COUNT(*) FROM Courses";
                using (var command = new SqlCommand(coursesQuery, connection))
                {
                    dashboardData.NumberOfCourses = (int)await command.ExecuteScalarAsync();
                }

                // Get the number of Class Sessions
                var classSessionsQuery = "SELECT COUNT(*) FROM ClassSessions";
                using (var command = new SqlCommand(classSessionsQuery, connection))
                {
                    dashboardData.NumberOfClassSessions = (int)await command.ExecuteScalarAsync();
                }

                // Get the number of Enrollments
                var enrollmentsQuery = "SELECT COUNT(*) FROM Enrollments";
                using (var command = new SqlCommand(enrollmentsQuery, connection))
                {
                    dashboardData.NumberOfEnrollments = (int)await command.ExecuteScalarAsync();
                }

                // Get the total payment amount
                var totalAmountQuery = "SELECT SUM(Amount) FROM Payments";
                using (var command = new SqlCommand(totalAmountQuery, connection))
                {
                    dashboardData.TotalPayments = (decimal)await command.ExecuteScalarAsync();
                }
            }

            return View(dashboardData);
        }
    }

    public class DashboardViewModel
    {
        public int NumberOfTeachers { get; set; }
        public int NumberOfStudents { get; set; }
        public int NumberOfCourses { get; set; }
        public int NumberOfClassSessions { get; set; }
        public int NumberOfEnrollments { get; set; }
        public decimal TotalPayments { get; set; }
    }
}
