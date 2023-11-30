using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Uploading_File.Models;

namespace Uploading_File.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public IActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile(UploadFileModel model)
        {
            if (ModelState.IsValid)
            {
                using (var stream = model.File.OpenReadStream())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    var excelReaderConfig = new ExcelReaderConfiguration()
                    {
                        FallbackEncoding = Encoding.GetEncoding(1252),
                        AutodetectSeparators = new char[] { ',', '\t', ';', '|' },
                        LeaveOpen = true 
                    };

                    using (var reader = ExcelReaderFactory.CreateReader(stream, excelReaderConfig))
                    {
                        var students = new List<StudentModel>();
                        int rowIndex = 0;
                        while (reader.Read())
                        {
                            if (rowIndex == 0)
                            {
                                rowIndex++;
                                continue;
                            }
                            students.Add(new StudentModel
                            {
                                Studentid = Convert.ToInt32(reader.GetValue(0)),
                                StudentName = reader.GetValue(1).ToString(),
                                Section = reader.GetValue(2).ToString(),
                                Subject1 = Convert.ToInt32(reader.GetValue(3)),
                                Subject2 = Convert.ToInt32(reader.GetValue(4)),
                                Total = Convert.ToInt32(reader.GetValue(5))
                            });

                            rowIndex++;
                        }
                        StoreDataInDatabase(students);
                    }
                }

                return RedirectToAction("Success");
            }

            return View(model);
        }
        private void StoreDataInDatabase(List<StudentModel> students)
        {
            string connectionString = _config.GetConnectionString("connectionstring");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    foreach (var student in students)
                    {
                        // Create InsertCommand with parameters
                        command.CommandText = "INSERT INTO Student (Studentid, StudentName, Section, Subject1, Subject2, Total) VALUES (@Studentid, @StudentName, @Section, @Subject1, @Subject2, @Total)";

                        //command.Parameters.Clear(); // Clear previous parameters

                        command.Parameters.AddWithValue("@Studentid", student.Studentid);
                        command.Parameters.AddWithValue("@StudentName", student.StudentName);
                        command.Parameters.AddWithValue("@Section", student.Section);
                        command.Parameters.AddWithValue("@Subject1", student.Subject1);
                        command.Parameters.AddWithValue("@Subject2", student.Subject2);
                        command.Parameters.AddWithValue("@Total", student.Total);

                        // Execute the SQL command for each student
                        int result = command.ExecuteNonQuery();
                    }
                }
            }
        }


        public IActionResult Success()
        {
            return View();
        }

        // Other actions and methods as needed
    }
}
