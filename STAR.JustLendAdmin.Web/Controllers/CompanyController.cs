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
    public class CompanyController : Controller
    {
        private readonly ICompanyService service;
        private readonly ILogger<CompanyController> log;

        public CompanyController(ICompanyService companyService, ILogger<CompanyController> log)
        {
            this.service = companyService;
            this.log = log;
        }

        // GET: CompanyController
        public async Task<IActionResult> Index()
        {
            var companies = await service.GetAsync();
            return View(companies.ToList());
        }

        // GET: CompanyController/Create
        public ActionResult Create()
        {
            return View(new CompanyViewModel());
        }

        // POST: CompanyController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            try
            {
                var response = await service.UpsertAsync(company);
                if(response.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var viewModel = new CompanyViewModel { Company = response.Model, Response = new ResponseCore { Message = response.Message, Success = response.Success } };
                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                log.LogError(ex, "Error creating company");
                return View("Error", ex);
            }
        }

        // GET: CompanyController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var company = await service.GetAsync(id);
                if(company != null)
                {
                    return View(new CompanyViewModel { Company = company });
                }
                else
                {
                    return NotFound(id);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting company");
                return View("Error", ex);
            }
        }

        // POST: CompanyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Company model)
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
                    var viewModel = new CompanyViewModel { Company = response.Model, Response = new ResponseCore { Message = response.Message, Success = response.Success } };
                    return View(viewModel);
                }
            }
            catch(Exception ex)
            {
                log.LogError(ex, "Error editing company");
                return View("Error", ex);
            }
        }
    }
}
