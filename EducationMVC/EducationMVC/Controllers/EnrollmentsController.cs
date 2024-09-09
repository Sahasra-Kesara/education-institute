using EducationMVC.Models;
using EducationMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EducationMVC.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var enrollments = context.Enrollments
                .Include(e => e.Student) // Include the Student
                .Include(e => e.Course) // Include the Course
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                enrollments = enrollments.Where(e => e.Student.Name.Contains(searchString) || e.Course.CourseName.Contains(searchString));
            }

            return View(enrollments.OrderByDescending(e => e.Id).ToList());
        }

        public IActionResult Create()
        {
            ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
            ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
            return View();
        }

        [HttpPost]
        public IActionResult Create(EnrollmentDto enrollmentDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
                ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
                return View(enrollmentDto);
            }

            var enrollment = new Enrollment()
            {
                StudentId = enrollmentDto.StudentId,
                CourseId = enrollmentDto.CourseId,
                EnrollmentDate = enrollmentDto.EnrollmentDate
            };

            context.Enrollments.Add(enrollment);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var enrollment = context.Enrollments
                .Include(e => e.Student) // Include the Student
                .Include(e => e.Course) // Include the Course
                .FirstOrDefault(e => e.Id == id);

            if (enrollment == null)
            {
                return RedirectToAction("Index");
            }

            var enrollmentDto = new EnrollmentDto()
            {
                Id = enrollment.Id,
                StudentId = enrollment.StudentId,
                CourseId = enrollment.CourseId,
                EnrollmentDate = enrollment.EnrollmentDate
            };

            ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
            ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
            return View(enrollmentDto);
        }

        [HttpPost]
        public IActionResult Edit(int id, EnrollmentDto enrollmentDto)
        {
            var enrollment = context.Enrollments.Find(id);
            if (enrollment == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
                ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
                return View(enrollmentDto);
            }

            enrollment.StudentId = enrollmentDto.StudentId;
            enrollment.CourseId = enrollmentDto.CourseId;
            enrollment.EnrollmentDate = enrollmentDto.EnrollmentDate;

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var enrollment = context.Enrollments.Find(id);
            if (enrollment == null)
            {
                return RedirectToAction("Index");
            }

            context.Enrollments.Remove(enrollment);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult DownloadCsv()
        {
            var enrollments = context.Enrollments
                .Include(e => e.Student) // Include the Student
                .Include(e => e.Course) // Include the Course
                .ToList();

            var csv = new StringBuilder();
            csv.AppendLine("Id,StudentName,CourseName,EnrollmentDate");

            foreach (var enrollment in enrollments)
            {
                csv.AppendLine($"{enrollment.Id},{enrollment.Student?.Name},{enrollment.Course?.CourseName},{enrollment.EnrollmentDate:yyyy-MM-dd}");
            }

            var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());
            return File(csvBytes, "text/csv", "enrollments.csv");
        }
    }
}