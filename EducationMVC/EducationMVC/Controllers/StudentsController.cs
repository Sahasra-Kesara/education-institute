using EducationMVC.Models;
using EducationMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Globalization;
using System.Text;

namespace EducationMVC.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public StudentsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var students = context.Students.OrderByDescending(p => p.Id).ToList();
            return View(students);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(StudentDto studentDto)
        {
            if (studentDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(studentDto);
            }
            //save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(studentDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/students/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                studentDto.ImageFile.CopyTo(stream);
            }
            //save the new student in the database
            Student student = new Student()
            {
                Name = studentDto.Name,
                Grade = studentDto.Grade,
                ImageFileName = newFileName,
                JoinDate = DateTime.Now,
            };
            context.Students.Add(student);
            context.SaveChanges();
            return RedirectToAction("Index", "Students");
        }

        public IActionResult Edit(int id)
        {
            var student = context.Students.Find(id);

            if (student == null)
            {
                return RedirectToAction("Index", "Students");
            }

            //create studentDto from student
            var studentDto = new StudentDto()
            {
                Name = student.Name,
                Grade = student.Grade,
            };

            ViewData["StudentId"] = student.Id;
            ViewData["StudentName"] = student.Name;
            ViewData["ImageFileName"] = student.ImageFileName;
            ViewData["JoinDate"] = student.JoinDate.ToString("MM/dd/yyyy");

            return View(studentDto);

        }

        [HttpPost]
        public IActionResult Edit(int id, StudentDto studentDto)
        {
            var student = context.Students.Find(id);
            if (student == null)
            {
                return RedirectToAction("Index", "Students");
            }

            if(!ModelState.IsValid)
            {
                ViewData["StudentId"] = student.Id;
                ViewData["StudentName"] = student.Name;
                ViewData["ImageFileName"] = student.ImageFileName;
                ViewData["JoinDate"] = student.JoinDate.ToString("MM/dd/yyyy");

                return View(studentDto);
            }

            //update the image file if we have a new image file
            string newFileName = student.ImageFileName;
            if (studentDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                newFileName += Path.GetExtension(studentDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/students/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    studentDto.ImageFile.CopyTo(stream);
                }

                //delete the old image
                string oldImageFullPath = environment.WebRootPath + "/students/" + student.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);  // Corrected to delete the old image file
            }

            //update the student in the database
            student.Name = studentDto.Name;
            student.Grade = studentDto.Grade;
            student.ImageFileName = newFileName;
            student.JoinDate = student.JoinDate;

            context.SaveChanges();

            return RedirectToAction("Index", "Students");
        }
        public IActionResult Delete(int id)
        {
            var student = context.Students.Find(id);
            if (student == null)
            {
                return RedirectToAction("Index", "Students");
            }
            string imageFullPath = environment.WebRootPath + "/students/" + student.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Students.Remove(student);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Students");
        }

        [HttpGet]
        public IActionResult Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var students = from s in context.Students
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.Name.Contains(searchString) || s.Grade.Contains(searchString));
            }

            return View(students.OrderByDescending(p => p.Id).ToList());
        }
        public IActionResult DownloadCsv()
        {
            // Get all students from the database
            var students = context.Students.ToList();

            // Create a StringBuilder to build the CSV file
            var csv = new StringBuilder();

            // Add header row
            csv.AppendLine("Id,Name,Grade,JoinDate");

            // Add student data rows
            foreach (var student in students)
            {
                csv.AppendLine($"{student.Id},{student.Name},{student.Grade},{student.JoinDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");
            }

            // Convert the StringBuilder to a byte array (for file download)
            var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());

            // Return the file for download with a meaningful file name
            return File(csvBytes, "text/csv", "students.csv");
        }

    }
}
