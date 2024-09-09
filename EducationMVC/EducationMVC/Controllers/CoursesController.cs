using EducationMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Linq;
using EducationMVC.Services;
using Microsoft.EntityFrameworkCore;

namespace EducationMVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public CoursesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }

        public IActionResult Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var courses = from c in context.Courses.Include(c => c.Teacher) // Eager load Teacher
                          select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                courses = courses.Where(c => c.CourseName.Contains(searchString) || c.CourseDescription.Contains(searchString));
            }

            return View(courses.OrderByDescending(c => c.Id).ToList());
        }

        public IActionResult Create()
        {
            ViewData["Teachers"] = context.Teachers.ToList(); // Provide list of teachers for dropdown
            return View();
        }

        [HttpPost]
        public IActionResult Create(CourseDto courseDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Teachers"] = context.Teachers.ToList(); // Provide list of teachers for dropdown
                return View(courseDto);
            }

            var course = new Course
            {
                CourseName = courseDto.CourseName,
                CourseDescription = courseDto.CourseDescription,
                TeacherId = courseDto.TeacherId,
                Schedule = courseDto.Schedule,
                DurationInWeeks = courseDto.DurationInWeeks
            };

            context.Courses.Add(course);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var course = context.Courses.Find(id);
            if (course == null)
            {
                return RedirectToAction("Index");
            }

            var courseDto = new CourseDto
            {
                Id = course.Id,
                CourseName = course.CourseName,
                CourseDescription = course.CourseDescription,
                TeacherId = course.TeacherId,
                TeacherName = context.Teachers.Find(course.TeacherId)?.Name ?? "",
                Schedule = course.Schedule,
                DurationInWeeks = course.DurationInWeeks
            };

            ViewData["Teachers"] = context.Teachers.ToList(); // Provide list of teachers for dropdown
            return View(courseDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, CourseDto courseDto)
        {
            var course = context.Courses.Find(id);
            if (course == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Teachers"] = context.Teachers.ToList(); // Provide list of teachers for dropdown
                return View(courseDto);
            }

            course.CourseName = courseDto.CourseName;
            course.CourseDescription = courseDto.CourseDescription;
            course.TeacherId = courseDto.TeacherId;
            course.Schedule = courseDto.Schedule;
            course.DurationInWeeks = courseDto.DurationInWeeks;

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var course = context.Courses.Find(id);
            if (course == null)
            {
                return RedirectToAction("Index");
            }

            context.Courses.Remove(course);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DownloadCsv()
        {
            var courses = context.Courses.Include(c => c.Teacher).ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Id,CourseName,CourseDescription,TeacherName,Schedule,DurationInWeeks");

            foreach (var course in courses)
            {
                csv.AppendLine($"{course.Id},{course.CourseName},{course.CourseDescription},{course.Teacher.Name},{course.Schedule},{course.DurationInWeeks}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());

            return File(csvBytes, "text/csv", "courses.csv");
        }
    }
}
