using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using AutoMapper;
using BaseApplication.DuoIntegration;
using BaseApplication.Entity;
using BaseApplication.Helpers;
using BaseApplication.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaseApplication.Controllers
{
    public class ClientController : Controller
    {
        private readonly ILogger _logger;
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;
        public ClientController(ILogger<ClientController> logger, ApplicationDBContext dBContext, IMapper mapper, INotyfService notyf)
        {
            _logger = logger;
            this._dbContext = dBContext;
            _mapper = mapper;
            _notyf = notyf;
        }
        public async Task<IActionResult> ClientRegistration()
        {
            TridentClientModel model = new TridentClientModel();
            var coordinators = _dbContext.Coordinators.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            coordinators.Insert(0, new SelectListItem { Text = "Please select", Value = "" });
            ViewBag.Coordinators = coordinators;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ClientRegistration(TridentClientModel tridentClient)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tridentClient.Notes = "(" + DateTime.Now + ")" +"-"+ tridentClient.Notes;
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<TridentClientModel, TridentClient>();
                    });

                    var mapper = config.CreateMapper();
                    var client = mapper.Map<TridentClientModel, TridentClient>(tridentClient);
                    //var client = _mapper.Map<Entity.TridentClient>(tridentClient);
                    //await _dbContext.SaveChangesAsync();
                    _dbContext.Add(client);
                    await _dbContext.SaveChangesAsync();
                    _notyf.Success("Client created successfully");
                    return RedirectToAction(nameof(ClientList));
                }
                else
                {
                    _notyf.Error("Error occured. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("UserController | ClientRegistration | Exception occured.." + ex.Message);
                throw;
            }
            return View();
        }
        public async Task<IActionResult> ClientList()
        {
            try
            {
                var tridentClientModels = _dbContext.TridentClients.ToList().Select(client => new TridentClientModel
                {
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                    Phone = client.Phone,
                    AlternatePhone = client.AlternatePhone,
                    ZipCode = client.ZipCode,
                    State = client.State,
                    City = client.City,
                    Address1 = client.Address1,   
                    Notes = client.Notes,
                    CoordinatorId = client.CoordinatorId,
                    Status = client.Status
                }).ToList();
                var coordinators = _dbContext.Coordinators.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
                ViewBag.Coordinators = coordinators;

                return View(tridentClientModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("UserController | ClientRegistration | Exception occured.." + ex.Message);
                throw;
            }
            return View();
        }
        public async Task<IActionResult> EditClient(int? id)
        {
            if (id == null || _dbContext.TridentClients == null)
            {
                return NotFound();
            }
            var coordinators = _dbContext.Coordinators.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            coordinators.Insert(0, new SelectListItem { Text = "Please select", Value = "" });
            ViewBag.Coordinators = coordinators;
            var tridentClientModels = _dbContext.TridentClients.ToList().Select(client => new TridentClientModel
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                AlternatePhone = client.AlternatePhone,
                ZipCode = client.ZipCode,
                State = client.State,
                City = client.City,
                Address1 = client.Address1,
                Notes = client.Notes,
                CoordinatorId = client.CoordinatorId,
                Status = client.Status
            }).ToList().Where(m => m.Id == id).FirstOrDefault();

            if (tridentClientModels?.Notes != null)
            {
                string notes = tridentClientModels.Notes;

                int notesIndex = notes.IndexOf(')');

                if (notesIndex != -1 && notesIndex < notes.Length - 1)
                {
                    string UpdatedNote = notes.Substring(notesIndex + 1);
                    tridentClientModels.Notes = UpdatedNote.Substring(1);
                }
            }
            if (tridentClientModels == null)
            {
                return NotFound();
            }

            return View(tridentClientModels);
        }
        [HttpPost]
        public async Task<IActionResult> EditClient(TridentClientModel tridentClientModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    tridentClientModel.Notes = "(" + DateTime.Now + ")" + "-" + tridentClientModel.Notes;
                    var config = new MapperConfiguration(cfg => { cfg.CreateMap<TridentClientModel, TridentClient>(); });
                    var mapper = config.CreateMapper();
                    var client = mapper.Map<TridentClientModel, TridentClient>(tridentClientModel);
                    _dbContext.Update(client);
                    await _dbContext.SaveChangesAsync();
                    _notyf.Success("Client updated successfully.");
                }
                catch (Exception ex)
                {
                    throw;
                }
                return RedirectToAction(nameof(ClientList));
            }
            return View(tridentClientModel);
        }
        public async Task<IActionResult> DeleteClient(int? id)
        {
            if (id == null || _dbContext.TridentClients == null)
            {
                return NotFound();
            }
            var tridentClients = _dbContext.TridentClients.ToList();
            var tridentClientModels = tridentClients.Select(client => new TridentClientModel
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Phone = client.Phone,
                AlternatePhone = client.AlternatePhone,
                ZipCode = client.ZipCode,
                State = client.State,
                City = client.City,
                Address1 = client.Address1,
                Notes = client.Notes,
                CoordinatorId = client.CoordinatorId,
                Status = client.Status
            }).FirstOrDefault(m => m.Id == id);
            var coordinators = _dbContext.Coordinators.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            ViewBag.Coordinators = coordinators;

            if (tridentClientModels == null)
            {
                return NotFound();
            }

            return View(tridentClientModels);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteClient(int id)
        {
            if (_dbContext.TridentClients == null)
            {
                _notyf.Error("No data found.");
                return Problem("Entity set '_dbContext.TridentClients'  is null."); // need to change
            }
            var client = await _dbContext.TridentClients.FindAsync(id);
            if (client != null)
            {
                _dbContext.TridentClients.Remove(client);
            }

            await _dbContext.SaveChangesAsync();
            _notyf.Success("User deleted successfully.");
            return RedirectToAction(nameof(ClientList));
        }
    }
}
