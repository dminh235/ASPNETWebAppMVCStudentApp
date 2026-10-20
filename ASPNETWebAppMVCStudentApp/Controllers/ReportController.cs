
using ASPNETWebAppMVCStudentApp.Filters;
using Microsoft.Reporting.WebForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ASPNETWebAppMVCStudentApp.Controllers
{
    [SessionAuthorize]
    public class ReportController : Controller
    {
        private readonly string connectionString =
            System.Configuration.ConfigurationManager
            .ConnectionStrings["SchoolDBSql"].ConnectionString;

        public ActionResult Index()
        {
            ViewBag.Courses = GetCourses();
            return View();
        }

        private DataTable GetCourses()
        {
            DataTable table = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT CourseID, Title FROM Courses ORDER BY Title", conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
            {
                adapter.Fill(table);
            }

            return table;
        }

        public ActionResult Print(int courseId, string mode)
        {
            DataTable students = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT s.StudentID, s.FirstName, s.LastName, e.Grade
                  FROM Students s
                  INNER JOIN Enrollments e ON s.StudentID = e.StudentID
                  WHERE e.CourseID = @CourseID
                  ORDER BY s.LastName, s.FirstName", conn))
            {
                cmd.Parameters.Add("@CourseID", SqlDbType.Int).Value = courseId;

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(students);
                }
            }

            LocalReport report = new LocalReport();
            report.ReportPath = Server.MapPath("~/Reports/StudentReport.rdlc");
            report.DataSources.Clear();
            report.DataSources.Add(
                new ReportDataSource("StudentDataSet", students));

            string mimeType;
            string encoding;
            string fileNameExtension;
            string[] streams;
            Warning[] warnings;

            byte[] pdf = report.Render(
                "PDF",
                null,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            if (mode == "download")
            {
                return File(pdf, "application/pdf", "StudentReport.pdf");
            }

            return File(pdf, "application/pdf");
        }
    }
}
