using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using STAR.JustLendAdmin.Web.Models;
using STAR.JustLendAdmin.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace STAR.JustLendAdmin.Web.Controllers
{
    public class VendorController : Controller
    {
        private readonly IVendorService service;
        private readonly ILogger<VendorController> log;

        public VendorController(IVendorService vendorService, ILogger<VendorController> log)
        {
            this.service = vendorService;
            this.log = log;
        }

        // GET: Vendor
        public IActionResult Index()
        {
            return View();
        }

        // GET: Vendor/Create
        public ActionResult Create()
        {
            return View(new VendorViewModel());
        }

        // POST: Vendor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendorViewModel viewModel)
        {
            try
            {
                log.LogInformation($"New vendor creation requested [{viewModel.Vendor.General.Name}]");
                var response = await service.UpsertAsync(viewModel.Vendor);
                if (response.Success)
                {
                    log.LogInformation($"New vendor created [{response.Model.Id}] - [{response.Model.General.Name}]");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    log.LogWarning($"New vendor could not be created [{viewModel.Vendor.General.Name}]");
                    viewModel.Vendor = response.Model;
                    viewModel.Response  = new ResponseCore { Message = response.Message, Success = response.Success };
                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                log.LogError(ex, $"Error creating vendor [{JsonConvert.SerializeObject(viewModel.Vendor)}]");
                return View("Error", ex);
            }
        }

        // GET: Vendor/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var model = await service.GetAsync(id);
                if (model != null)
                {
                    return View(new VendorViewModel { Vendor = model });
                }
                else
                {
                    return NotFound(id);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting vendor");
                return View("Error", ex);
            }
        }

        // POST: Vendor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, VendorViewModel viewModel)
        {
            try
            {
                log.LogInformation($"Vendor edit requested [{viewModel.Vendor.Id}]");
                var response = await service.UpsertAsync(viewModel.Vendor);
                if (response.Success)
                {
                    log.LogInformation($"Vendor edit completed [{viewModel.Vendor.Id}]");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    log.LogWarning($"Vendor edit could not be completed [{viewModel.Vendor.Id }]");
                    viewModel.Vendor = response.Model;
                    viewModel.Response  = new ResponseCore { Message = response.Message, Success = response.Success };
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error editing vendor [{JsonConvert.SerializeObject(viewModel.Vendor)}]");
                return View("Error", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListVendors([FromQuery]int selection)
        {
            var vendorType = (VendorType)selection;
            var vendors = await service.GetAsync(new GetVendorRequest { VendorType = vendorType });
            return PartialView("~/Views/Vendor/VendorGridPartialView.cshtml", vendors.ToList());
        }
    }
}
