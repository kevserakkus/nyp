using Kevser_Akkus_FinalProje.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Security.Policy;
using System.Xml.Linq;

namespace Kevser_Akkus_FinalProje.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static string username;
        public static string usersifre;
        public static string AdSoyad;
        public static string CreateAt;
        public static string rol;
        public static string connectionString = "Data Source=DESKTOP-1SV0TLS\\MSSQLSERVER01;Initial Catalog=nypodev;Integrated Security=True;";
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Profile();
            ViewBag.username = username;
            ViewBag.usersifre = usersifre;
            ViewBag.AdSoyad = AdSoyad;
            ViewBag.CreateAt = CreateAt;
            ViewBag.rol = rol;

            return View();
        }
        public async Task<IActionResult> Profile()
        {
            string query = "SELECT * FROM dbo.KevserUsers WHERE UserName = @isim AND UserPassword = @sifre";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@isim", username);
                command.Parameters.AddWithValue("@sifre", usersifre);

                await con.OpenAsync();
                SqlDataReader reader = await command.ExecuteReaderAsync();

                reader.Read();

                username = reader["UserName"].ToString();
                usersifre = reader["UserPassword"].ToString();
                AdSoyad = reader["AdSoyad"].ToString();
                CreateAt = reader["CreateAt"].ToString();
                rol = reader["Rol"].ToString();

            }



            return View();
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string isim, string sifre)
        {

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT count(*) FROM dbo.KevserUsers WHERE UserName = @Name AND UserPassword = @Password";
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@Name", isim);
                command.Parameters.AddWithValue("@Password", sifre);
                username = isim;
                usersifre = sifre;
                await con.OpenAsync();
                int count = (int)await command.ExecuteScalarAsync();

                if (count > 0)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }

        }
        public async Task<IActionResult> Register()
        {


            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(string isim, string sifre, string adsoyad, string roller)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"INSERT INTO dbo.KevserUsers
        (
            UserName
            ,UserPassword
            ,AdSoyad
            ,CreateAt
            ,Locked
            ,Rol
        )
        VALUES
        (
            @Namespace,
            @Sifre,
            @AdSoyad,
            GETDATE(),
            NULL,
            @Roller
        )";

                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@Namespace", isim);
                command.Parameters.AddWithValue("@Sifre", sifre);
                command.Parameters.AddWithValue("@AdSoyad", adsoyad);
                command.Parameters.AddWithValue("@Roller", roller);

                await con.OpenAsync();

                await command.ExecuteNonQueryAsync();

                return RedirectToAction("Index", "Home");

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}