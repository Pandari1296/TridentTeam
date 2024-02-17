using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using BaseApplication.Entity;
using BaseApplication.Helpers;
using BaseApplication.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IEmailHelper _emailHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;
        private readonly IConfiguration _config;
        private readonly IDataProtector _dataProtector;

        public UserController(ApplicationDBContext dBContext, IEmailHelper emailHelper, IWebHostEnvironment webHostEnvironment, IMapper mapper, INotyfService notyf,
            IConfiguration configuration, IDataProtectionProvider provider)
        {
            this._dbContext = dBContext;
            this._emailHelper = emailHelper;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _notyf = notyf;
            _config = configuration;
            _dataProtector = provider.CreateProtector("BaseApplication.UserController");
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

        public async Task<IActionResult> Registration(string emailId)
        {
            UserModel userModel = new UserModel();
            string registrationId = _dataProtector.Unprotect(emailId);
            if(!string.IsNullOrWhiteSpace(registrationId))
            {
                int.TryParse(registrationId, out int id);
                if (id > 0)
                {
                    var userDetails = await _dbContext.RegistrationEmails.FirstOrDefaultAsync(x => x.Id == id);
                    if (userDetails != null)
                        userModel.UserEmail = userDetails.Email;
                }
                else
                    _notyf.Error("Invalid Link. Please try with original link.");
            }
            else
            {
                _notyf.Error("Error occured. Please try again.");
            }
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

        public IActionResult EmailInvite()
        {
            EmailInviteModel model = new EmailInviteModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmailInvite(EmailInviteModel model)
        {
            if (ModelState.IsValid)
            {
                List<RegistrationEmail> registrationEmails = [new RegistrationEmail { Email = model.Email1, IsActive = true }];
                if (!string.IsNullOrWhiteSpace(model.Email2))
                    registrationEmails.Add(new RegistrationEmail { Email = model.Email2, IsActive = true });
                if (!string.IsNullOrWhiteSpace(model.Email3))
                    registrationEmails.Add(new RegistrationEmail { Email = model.Email3, IsActive = true });
                if (!string.IsNullOrWhiteSpace(model.Email4))
                    registrationEmails.Add(new RegistrationEmail { Email = model.Email4, IsActive = true });
                if (!string.IsNullOrWhiteSpace(model.Email5))
                    registrationEmails.Add(new RegistrationEmail { Email = model.Email5, IsActive = true });
                if (!string.IsNullOrWhiteSpace(model.Email6))
                    registrationEmails.Add(new RegistrationEmail { Email = model.Email6, IsActive = true });
                await _dbContext.RegistrationEmails.AddRangeAsync(registrationEmails);
                int result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    foreach (var item in registrationEmails)
                    {
                        string encryptString = _dataProtector.Protect(item.Id.ToString());
                        string registrationLink = $"{_config.GetValue<string>("ApplicationUrl")}User/Registration?emailId={encryptString}";
                        string emailBody = GetRegistrationEmailBody(item.Email.Split('@')[0], registrationLink);
                        var isEmailSent = _emailHelper.SendEmail(item.Email, "Team Trident Registration", emailBody);
                        if (isEmailSent)
                        {
                            _notyf.Success("Email Invitation send successfully.");
                            return RedirectToAction("EmailInvite", "User");
                        }
                        else
                        {
                            _notyf.Error("Error occured while sending email. Please try again.");
                        }
                    }
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

        private string GetRegistrationEmailBody(string name, string registrationLink)
        {
            string templatePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "EmailTemplates", "RegistrationTemplate.html");

            string emailBody = System.IO.File.ReadAllText(templatePath);
            return emailBody.Replace("{{Recipient_Name}}", name).Replace("{{Registration_Link}}", registrationLink);
        }
    }
}
