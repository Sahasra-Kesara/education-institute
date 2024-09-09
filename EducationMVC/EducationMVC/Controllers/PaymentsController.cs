using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EducationMVC.Models;
using System.Linq;
using System.Threading.Tasks;
using EducationMVC.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.IO;

namespace EducationMVC.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext context;

        public IActionResult DownloadCsv()
        {
            var payments = context.Payments
                .Include(p => p.Student)  // Include the Student
                .Include(p => p.Course)   // Include the Course
                .ToList();

            var builder = new StringBuilder();
            builder.AppendLine("Id,StudentName,CourseName,Amount,PaymentDate,PaymentMethod");

            foreach (var payment in payments)
            {
                builder.AppendLine($"{payment.Id},{payment.Student.Name},{payment.Course.CourseName},{payment.Amount},{payment.PaymentDate.ToString("yyyy-MM-dd")},{payment.PaymentMethod}");
            }

            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "payments.csv");
        }


        public PaymentsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // List all payments, with optional search
        public IActionResult Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var payments = context.Payments
                .Include(p => p.Student)  // Include the Student
                .Include(p => p.Course)   // Include the Course
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                payments = payments.Where(p => p.Student.Name.Contains(searchString) || p.Course.CourseName.Contains(searchString));
            }

            return View(payments.OrderByDescending(p => p.Id).ToList());
        }

        // Display Create Payment form
        public IActionResult Create()
        {
            ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
            ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
            return View();
        }

        // Handle Create Payment form submission
        [HttpPost]
        public IActionResult Create(PaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
                ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
                return View(paymentDto);
            }

            var payment = new Payment()
            {
                StudentId = paymentDto.StudentId,
                CourseId = paymentDto.CourseId,
                Amount = paymentDto.Amount,
                PaymentDate = paymentDto.PaymentDate,
                PaymentMethod = paymentDto.PaymentMethod
            };

            context.Payments.Add(payment);
            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Display Edit Payment form
        public IActionResult Edit(int id)
        {
            var payment = context.Payments
                .Include(p => p.Student)  // Include the Student
                .Include(p => p.Course)   // Include the Course
                .FirstOrDefault(p => p.Id == id);

            if (payment == null)
            {
                return RedirectToAction("Index");
            }

            var paymentDto = new PaymentDto()
            {
                Id = payment.Id,
                StudentId = payment.StudentId,
                CourseId = payment.CourseId,
                Amount = payment.Amount,
                PaymentDate = payment.PaymentDate,
                PaymentMethod = payment.PaymentMethod
            };

            ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
            ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
            return View(paymentDto);
        }

        // Handle Edit Payment form submission
        [HttpPost]
        public IActionResult Edit(int id, PaymentDto paymentDto)
        {
            var payment = context.Payments.Find(id);
            if (payment == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewData["Students"] = new SelectList(context.Students, "Id", "Name");
                ViewData["Courses"] = new SelectList(context.Courses, "Id", "CourseName");
                return View(paymentDto);
            }

            payment.StudentId = paymentDto.StudentId;
            payment.CourseId = paymentDto.CourseId;
            payment.Amount = paymentDto.Amount;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.PaymentMethod = paymentDto.PaymentMethod;

            context.SaveChanges();

            return RedirectToAction("Index");
        }

        // Delete payment
        public IActionResult Delete(int id)
        {
            var payment = context.Payments.Find(id);
            if (payment == null)
            {
                return RedirectToAction("Index");
            }

            context.Payments.Remove(payment);
            context.SaveChanges();

            return RedirectToAction("Index");
        }
    }

}
