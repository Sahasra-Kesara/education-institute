using EducationMVC.Models;
using EducationMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;

namespace EducationMVC.Controllers
{
    public class TeachersController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;

        public TeachersController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index()
        {
            var teachers = context.Teachers.OrderByDescending(p => p.Id).ToList();
            return View(teachers);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(TeacherDto teacherDto)
        {
            if (teacherDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "The file is required");
            }
            if (!ModelState.IsValid)
            {
                return View(teacherDto);
            }
            //save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(teacherDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/teachers/" + newFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                teacherDto.ImageFile.CopyTo(stream);
            }
            //save the new teacher in the database
            Teacher teacher = new Teacher()
            {
                Name = teacherDto.Name,
                Subject = teacherDto.Subject,
                ImageFileName = newFileName,
                JoinDate = DateTime.Now,
            };
            context.Teachers.Add(teacher);
            context.SaveChanges();
            return RedirectToAction("Index", "Teachers");
        }

        public IActionResult Edit(int id)
        {
            var teacher = context.Teachers.Find(id);

            if (teacher == null)
            {
                return RedirectToAction("Index", "Teachers");
            }

            //create teacherDto from teacher
            var teacherDto = new TeacherDto()
            {
                Name = teacher.Name,
                Subject = teacher.Subject,
            };

            ViewData["TeacherId"] = teacher.Id;
            ViewData["TeacherName"] = teacher.Name;
            ViewData["ImageFileName"] = teacher.ImageFileName;
            ViewData["JoinDate"] = teacher.JoinDate.ToString("MM/dd/yyyy");

            return View(teacherDto);

        }

        [HttpPost]
        public IActionResult Edit(int id, TeacherDto teacherDto)
        {
            var teacher = context.Teachers.Find(id);
            if (teacher == null)
            {
                return RedirectToAction("Index", "Teachers");
            }

            if (!ModelState.IsValid)
            {
                ViewData["TeacherId"] = teacher.Id;
                ViewData["TeacherName"] = teacher.Name;
                ViewData["ImageFileName"] = teacher.ImageFileName;
                ViewData["JoinDate"] = teacher.JoinDate.ToString("MM/dd/yyyy");

                return View(teacherDto);
            }

            //update the image file if we have a new image file
            string newFileName = teacher.ImageFileName;
            if (teacherDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssffff");
                newFileName += Path.GetExtension(teacherDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/teachers/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    teacherDto.ImageFile.CopyTo(stream);
                }

                //delete the old image
                string oldImageFullPath = environment.WebRootPath + "/teachers/" + teacher.ImageFileName;
                System.IO.File.Delete(oldImageFullPath);  // Corrected to delete the old image file
            }

            //update the teacher in the database
            teacher.Name = teacherDto.Name;
            teacher.Subject = teacherDto.Subject;
            teacher.ImageFileName = newFileName;
            teacher.JoinDate = teacher.JoinDate;

            context.SaveChanges();

            return RedirectToAction("Index", "Teachers");
        }
        public IActionResult Delete(int id)
        {
            var teacher = context.Teachers.Find(id);
            if (teacher == null)
            {
                return RedirectToAction("Index", "Teachers");
            }
            string imageFullPath = environment.WebRootPath + "/teachers/" + teacher.ImageFileName;
            System.IO.File.Delete(imageFullPath);

            context.Teachers.Remove(teacher);
            context.SaveChanges(true);

            return RedirectToAction("Index", "Teachers");
        }

        [HttpGet]
        public IActionResult Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var teachers = from s in context.Teachers
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                teachers = teachers.Where(s => s.Name.Contains(searchString) || s.Subject.Contains(searchString));
            }

            return View(teachers.OrderByDescending(p => p.Id).ToList());
        }
        public IActionResult DownloadCsv()
        {
            // Get all teachers from the database
            var teachers = context.Teachers.ToList();

            // Create a StringBuilder to build the CSV file
            var csv = new StringBuilder();

            // Add header row
            csv.AppendLine("Id,Name,Subject,JoinDate");

            // Add teacher data rows
            foreach (var teacher in teachers)
            {
                csv.AppendLine($"{teacher.Id},{teacher.Name},{teacher.Subject},{teacher.JoinDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}");
            }

            // Convert the StringBuilder to a byte array (for file download)
            var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());

            // Return the file for download with a meaningful file name
            return File(csvBytes, "text/csv", "teachers.csv");
        }

    }
}
