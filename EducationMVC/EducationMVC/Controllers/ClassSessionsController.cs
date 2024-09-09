using EducationMVC.Models;
using EducationMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace EducationMVC.Controllers
{
    public class ClassSessionsController : Controller
    {
        private readonly ApplicationDbContext context;

        public ClassSessionsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult DownloadCsv()
        {
            // Get all class sessions from the database
            var classSessions = context.ClassSessions.ToList();

            // Create a StringBuilder to build the CSV file
            var csv = new StringBuilder();

            // Add header row
            csv.AppendLine("SessionID,CourseID,Date,Time,Duration");

            // Add class session data rows
            foreach (var session in classSessions)
            {
                // Format Time and Duration properties
                var timeFormatted = session.Time.ToString(@"hh\:mm\:ss");
                var durationFormatted = session.Duration.ToString(@"hh\:mm\:ss");

                // Append data row
                csv.AppendLine($"{session.SessionID},{session.CourseID},{session.Date:yyyy-MM-dd},{timeFormatted},{durationFormatted}");
            }

            // Convert the StringBuilder to a byte array (for file download)
            var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());

            // Return the file for download with a meaningful file name
            return File(csvBytes, "text/csv", "class_sessions.csv");
        }

        public IActionResult Index()
        {
            var classSessions = context.ClassSessions.OrderByDescending(p => p.SessionID).ToList();
            return View(classSessions);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ClassSessionDto classSessionDto)
        {
            if (!ModelState.IsValid)
            {
                return View(classSessionDto);
            }

            // Save the new class session in the database
            var classSession = new ClassSession()
            {
                SessionID = classSessionDto.SessionID,
                CourseID = classSessionDto.CourseID,
                Date = classSessionDto.Date,
                Time = classSessionDto.Time,
                Duration = classSessionDto.Duration,
            };

            context.ClassSessions.Add(classSession);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(string id)
        {
            var classSession = context.ClassSessions.Find(id);

            if (classSession == null)
            {
                return RedirectToAction("Index");
            }

            var classSessionDto = new ClassSessionDto()
            {
                SessionID = classSession.SessionID,
                CourseID = classSession.CourseID,
                Date = classSession.Date,
                Time = classSession.Time,
                Duration = classSession.Duration,
            };

            return View(classSessionDto);
        }

        [HttpPost]
        public IActionResult Edit(string id, ClassSessionDto classSessionDto)
        {
            var classSession = context.ClassSessions.Find(id);
            if (classSession == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                return View(classSessionDto);
            }

            // Update the class session in the database
            classSession.CourseID = classSessionDto.CourseID;
            classSession.Date = classSessionDto.Date;
            classSession.Time = classSessionDto.Time;
            classSession.Duration = classSessionDto.Duration;

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var classSession = context.ClassSessions.Find(id);
            if (classSession == null)
            {
                return RedirectToAction("Index");
            }

            context.ClassSessions.Remove(classSession);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var classSessions = from s in context.ClassSessions
                                select s;

            if (!string.IsNullOrEmpty(searchString))
            {
                classSessions = classSessions.Where(s => s.SessionID.Contains(searchString) || s.CourseID.Contains(searchString));
            }

            return View(classSessions.OrderByDescending(p => p.SessionID).ToList());
        }


    }

}
