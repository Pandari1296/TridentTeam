using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using BaseApplication.Entity;
using BaseApplication.Helpers;
using BaseApplication.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;

namespace BaseApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IEmailHelper _emailHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper; 
        private readonly INotyfService _notyf;

        public UserController(ApplicationDBContext dBContext, IEmailHelper emailHelper, IWebHostEnvironment webHostEnvironment, IMapper mapper, INotyfService notyf)
        {
            this._dbContext = dBContext;
            this._emailHelper = emailHelper;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            LoginModel loginModel = new LoginModel();
            return View(loginModel);
        }

        [HttpPost]
        public IActionResult Index(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                var hashPassword = PasswordHelper.HashPassword(loginModel.Password);
                if (!string.IsNullOrWhiteSpace(loginModel.UserEmail))
                {
                    var user = _dbContext.Users.Where(x => x.UserEmail == loginModel.UserEmail && x.Password == hashPassword).FirstOrDefault();
                    if (user != null)
                    {
                        HttpContext.Session.SetInt32("UserId", user.Id);
                        HttpContext.Session.SetString("UserName", user.FirstName + " ," + user.LastName);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        _notyf.Error("Invalid Username or password. Please check your credentials and try aagin.");
                    }
                }
            }
            else
            {
                _notyf.Error("Error occuredin Login.");
            }
            return View();
        }

        public IActionResult Registration()
        {
            UserModel userModel = new UserModel();
            return View(userModel);
        }

        [HttpPost]
        public JsonResult GetEmailOtp(string userEmail)
        {
            string randomOtp = string.Empty;
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                randomOtp = OtpGenerator.GenerateOtp();
                //Send email
                string emailBody = GetOtpEmailBody(randomOtp);
                var isEmialSent = true;// this._emailHelper.SendEmail(userEmail, "One-Time Password (OTP)", emailBody);
                if (isEmialSent)
                {
                    return Json(randomOtp);
                }
                else
                {
                    return Json("Problem occured while sending an email. Please check your email and try again.");
                }
            }
            return Json(randomOtp);
        }

        [HttpPost]
        public async Task<IActionResult> Registration(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var user = _mapper.Map<User>(userModel);
                if (user != null && !string.IsNullOrWhiteSpace(user.Password))
                {
                    user.Password = PasswordHelper.HashPassword(user.Password);
                    _dbContext.Add(user);
                    await _dbContext.SaveChangesAsync();
                    _notyf.Success("User created successfully. Please login and continue.");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                _notyf.Error("Error occured. Please try again.");
            }
            return View();
        }

        private string GetOtpEmailBody(string otp)
        {
            string templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "EmailTemplates", "OtpTemplate.html");

            string emailBody = System.IO.File.ReadAllText(templatePath);
            return emailBody.Replace("{{OTP_VALUE}}", otp);
        }
    }
}
