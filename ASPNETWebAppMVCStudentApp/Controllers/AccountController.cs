
using ASPNETWebAppMVCStudentApp.Filters;
using ASPNETWebAppMVCStudentApp.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace ASPNETWebAppMVCStudentApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            if (Session["Username"] != null)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string connectionString =
                ConfigurationManager.ConnectionStrings["SchoolDBSql"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(*)
                               FROM tblUser
                               WHERE Username = @Username
                               AND Password = @Password";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", model.Username);
                    cmd.Parameters.AddWithValue("@Password", model.Password);

                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();

                    if (count > 0)
                    {
                        Session["Username"] = model.Username;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.Error = "Invalid username or password!";
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", "Account");
        }
    }
}
