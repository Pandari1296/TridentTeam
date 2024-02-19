using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using BaseApplication.DuoIntegration;
using BaseApplication.Entity;
using BaseApplication.Helpers;
using BaseApplication.Models;
using DuoUniversal;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace BaseApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDBContext _dbContext;
        private readonly IEmailHelper _emailHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;
        private readonly IConfiguration _config;
        private readonly IDataProtector _dataProtector;
        private readonly IDuoClientProvider _duoClientProvider;

        public UserController(ILogger<UserController> logger, ApplicationDBContext dBContext, IEmailHelper emailHelper, IWebHostEnvironment webHostEnvironment, IMapper mapper, INotyfService notyf,
            IConfiguration configuration, IDataProtectionProvider provider, IDuoClientProvider duoClientProvider)
        {
            _logger = logger;
            this._dbContext = dBContext;
            this._emailHelper = emailHelper;
            _webHostEnvironment = webHostEnvironment;
            _mapper = mapper;
            _notyf = notyf;
            _config = configuration;
            _dataProtector = provider.CreateProtector("BaseApplication.UserController");
            _duoClientProvider = duoClientProvider;
        }

        public IActionResult Index()
        {
            LoginModel loginModel = new LoginModel();
            return View(loginModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("UserController | Index | Entered into Idex() with Useremail : " + loginModel.UserEmail);
                var hashPassword = PasswordHelper.HashPassword(loginModel.Password);
                if (!string.IsNullOrWhiteSpace(loginModel.UserEmail))
                {
                    var user = _dbContext.Users.Where(x => x.UserEmail == loginModel.UserEmail && x.Password == hashPassword).FirstOrDefault();
                    if (user != null)
                    {
                        _logger.LogInformation("user verified successfully ! with UserId : " + user.Id);
                        HttpContext.Session.SetInt32(ApplicationConstant.USERID_SESSION_KEY, user.Id);
                        HttpContext.Session.SetString(ApplicationConstant.USERNAME_SESSION_KEY, user.FirstName + " ," + user.LastName);
                        HttpContext.Session.SetInt32(ApplicationConstant.ROLEID_SESSION_KEY, user.RoleId);

                        // Get a Duo client
                        Client duoClient = _duoClientProvider.GetDuoClient();

                        // Check if Duo seems to be healthy and able to service authentications.
                        // If Duo were unhealthy, you could possibly send user to an error page, or implement a fail mode
                        var isDuoHealthy = await duoClient.DoHealthCheck();

                        // Generate a random state value to tie the authentication steps together
                        string state = Client.GenerateState();
                        // Save the state and username in the session for later
                        HttpContext.Session.SetString(ApplicationConstant.STATE_SESSION_KEY, state);
                        HttpContext.Session.SetString(ApplicationConstant.USERNAME_SESSION_KEY, loginModel.UserEmail);

                        // Get the URI of the Duo prompt from the client.  This includes an embedded authentication request.
                        string promptUri = duoClient.GenerateAuthUri(loginModel.UserEmail, state);

                        _logger.LogInformation("user is redirected to url : " + promptUri);
                        // Redirect the user's browser to the Duo prompt.
                        // The Duo prompt, after authentication, will redirect back to the configured Redirect URI to complete the authentication flow.
                        // In this example, that is /duo_callback, which is implemented in Callback.cshtml.cs.
                        return new RedirectResult(promptUri);
                    }
                    else
                    {
                        _notyf.Error("Invalid Username or password. Please check your credentials and try aagin.");
                    }
                    _logger.LogInformation("UserController | Index | Exit Index Method..");
                }
            }
            else
            {
                _notyf.Error("Error occuredin Login.");
            }
            return View();
        }

        public async Task<IActionResult> DuoCallback(string state, string code)
        {
            try
            {
                _logger.LogInformation("UserController | DuoCallback | Entered into DuoCallback Method()..");
                // Duo should have sent a 'state' and 'code' parameter.  If either is missing or blank, something is wrong.
                if (string.IsNullOrWhiteSpace(state))
                {
                    throw new DuoException("Required state value was empty");
                }
                if (string.IsNullOrWhiteSpace(code))
                {
                    throw new DuoException("Required code value was empty");
                }

                // Get the Duo client again.  This can be either be cached in the session or newly built.
                // The only stateful information in the Client is your configuration, so you could even use the same client for multiple
                // user authentications if desired.
                Client duoClient = _duoClientProvider.GetDuoClient();

                // The original state value sent to Duo, as well as the username that started the auth, should be stored in the session.
                var sessionState = HttpContext.Session.GetString(ApplicationConstant.STATE_SESSION_KEY);
                var sessionUsername = HttpContext.Session.GetString(ApplicationConstant.USERNAME_SESSION_KEY);
                // If either is missing, something is wrong.
                if (string.IsNullOrEmpty(sessionState) || string.IsNullOrEmpty(sessionUsername))
                {
                    throw new DuoException("State or username were missing from your session");
                }

                // Confirm the original state (from the session) matches the state sent by Duo; this helps prevents replay attacks or session takeover
                if (!sessionState.Equals(state))
                {
                    throw new DuoException("Session state did not match the expected state");
                }

                // Get a summary of the authentication from Duo.  This will trigger an exception if the username does not match.
                IdToken token = await duoClient.ExchangeAuthorizationCodeFor2faResult(code, sessionUsername);

                // Do whatever checks you want on the returned information.  For this example, we'll simply print it to an HTML page.
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                string AuthResponse = System.Text.Json.JsonSerializer.Serialize(token, options);
                if (!string.IsNullOrWhiteSpace(AuthResponse))
                {
                    _logger.LogInformation("The Auth Response is : " + AuthResponse);
                    var response = JsonConvert.DeserializeObject<DuoAuthResponse>(AuthResponse);
                    if (response != null && response.AuthContext.result == "success")
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    _logger.LogError("UserController | DuoCallback | Auth Response is null.");
                }
                DuoResponseModel model = new DuoResponseModel { AuthResponse = AuthResponse };
                return View(model);

            }
            catch (Exception ex)
            {
                _logger.LogError("UserController | DuoCallback | Exception occured.." + ex.Message);
                throw;
            }
        }

        public async Task<IActionResult> Registration(string emailId)
        {
            UserModel userModel = new UserModel();
            string registrationId = _dataProtector.Unprotect(emailId);
            if (!string.IsNullOrWhiteSpace(registrationId))
            {
                int.TryParse(registrationId, out int id);
                if (id > 0)
                {
                    var userDetails = await _dbContext.RegistrationEmails.FirstOrDefaultAsync(x => x.Id == id && x.IsActive == false);
                    if (userDetails != null)
                        userModel.UserEmail = userDetails.Email;
                    else
                        _notyf.Error("The user is already registered.",5);
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
        public async Task<IActionResult> Registration(UserModel userModel)
        {
            try
            {
                _logger.LogInformation("UserController | Registration | Entered into Registration() with useremail : " + userModel.UserEmail);

                if (ModelState.IsValid)
                {
                    var user = _mapper.Map<Entity.User>(userModel);
                    if (user != null && !string.IsNullOrWhiteSpace(user.Password))
                    {
                        //Update Registartion Email to Activate..
                        var registartedEmail = await _dbContext.RegistrationEmails.FirstOrDefaultAsync(email => email.Email == userModel.UserEmail && email.IsActive == false);
                        if (registartedEmail != null)
                        {
                            user.RoleId = registartedEmail.RoleId;
                            registartedEmail.IsActive = true;
                            await _dbContext.SaveChangesAsync();

                            //User Registration adding values..
                            user.Password = PasswordHelper.HashPassword(user.Password);
                            _dbContext.Add(user);
                            await _dbContext.SaveChangesAsync();
                            _notyf.Success("User created successfully. Please login and continue.");
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            _notyf.Error("The user is not registered. Please try from source.");
                        }
                    }
                }
                else
                {
                    _notyf.Error("Error occured. Please try again.");
                }
                _logger.LogInformation("UserController | Registration | Exit from Registration Method()..");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserController | Registration | Exception occured.." + ex.Message);
                throw;
            }
        }

        public IActionResult EmailInvite()
        {
            EmailInviteModel model = new EmailInviteModel();
            model.Roles = _dbContext.Roles.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            model.Roles.Insert(0, new SelectListItem { Text = "Please select", Value = "" });
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EmailInvite(EmailInviteModel model)
        {
            try
            {
                _logger.LogInformation("UserController | EmailInvite | Entered into EmailInvite()..");
                if (ModelState.IsValid)
                {
                    int.TryParse(model.RoleId, out var roleId);
                    List<RegistrationEmail> registrationEmails = [new RegistrationEmail { Email = model.Email1, RoleId = roleId }];
                    if (!string.IsNullOrWhiteSpace(model.Email2))
                        registrationEmails.Add(new RegistrationEmail { Email = model.Email2, RoleId = roleId });
                    if (!string.IsNullOrWhiteSpace(model.Email3))
                        registrationEmails.Add(new RegistrationEmail { Email = model.Email3, RoleId = roleId });
                    if (!string.IsNullOrWhiteSpace(model.Email4))
                        registrationEmails.Add(new RegistrationEmail { Email = model.Email4, RoleId = roleId });
                    if (!string.IsNullOrWhiteSpace(model.Email5))
                        registrationEmails.Add(new RegistrationEmail { Email = model.Email5, RoleId = roleId });
                    if (!string.IsNullOrWhiteSpace(model.Email6))
                        registrationEmails.Add(new RegistrationEmail { Email = model.Email6, RoleId = roleId });
                    await _dbContext.RegistrationEmails.AddRangeAsync(registrationEmails);
                    int result = await _dbContext.SaveChangesAsync();

                    _logger.LogInformation("UserController | EmailInvite | Data saved successfully and result is :" + result);
                    if (result > 0)
                    {
                        foreach (var item in registrationEmails)
                        {
                            try
                            {
                                _logger.LogInformation($"UserController | EmailInvite | Started sendign invite for email : {item.Email}");
                                string encryptString = _dataProtector.Protect(item.Id.ToString());
                                string registrationLink = $"{_config.GetValue<string>("ApplicationUrl")}User/Registration?emailId={encryptString}";
                                string emailBody = GetRegistrationEmailBody(item.Email.Split('@')[0], registrationLink);
                                var isEmailSent = _emailHelper.SendEmail(_logger, item.Email, "Team Trident Registration", emailBody);
                                if (isEmailSent)
                                {
                                    _logger.LogInformation("UserController | EmailInvite | Email sent successfully for email :" + item.Email);
                                    _notyf.Success("Email Invitation send successfully.");
                                    return RedirectToAction("EmailInvite", "User");
                                }
                                else
                                {
                                    _logger.LogInformation("UserController | EmailInvite | Email sending failed for email :" + item.Email);
                                    _notyf.Error("Error occured while sending email. Please try again.");
                                }

                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"UserController | EmailInvite | Exception occured for email : {item.Email} and exception is: " + ex.Message);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogError("UserController | EmailInvite | Model state is invalid and error count :" + ModelState.ErrorCount);
                    _notyf.Error("Error occured. Please try again.");
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("UserController | EmailInvite | Exception occured.." + ex.Message);
                throw;
            }
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
