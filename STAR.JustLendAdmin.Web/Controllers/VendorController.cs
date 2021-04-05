using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public async Task<IActionResult> Create(Vendor vendor)
        {
            try
            {
                var response = await service.UpsertAsync(vendor);
                if (response.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var viewModel = new VendorViewModel { Vendor = response.Model, Response = new ResponseCore { Message = response.Message, Success = response.Success } };
                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                log.LogError(ex, "Error creating vendor");
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
        public async Task<IActionResult> Edit(string id, Vendor model)
        {
            try
            {
                var response = await service.UpsertAsync(model);
                if (response.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var viewModel = new VendorViewModel { Vendor = response.Model, Response = new ResponseCore { Message = response.Message, Success = response.Success } };
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error editing vendor");
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
