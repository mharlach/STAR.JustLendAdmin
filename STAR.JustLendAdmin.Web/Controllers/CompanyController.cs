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
        public async Task<IActionResult> Create(CompanyViewModel viewModel)
        {
            try
            {
                log.LogInformation($"New company creation requested [{viewModel.Company.General.Name}]");
                
                var response = await service.UpsertAsync(viewModel.Company);
                if(response.Success)
                {
                    log.LogInformation($"New company created [{viewModel.Company.General.Name}]");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    log.LogWarning($"New company could not be created [{viewModel.Company.General.Name}] [{response.Message}]");
                    viewModel.Company = response.Model;
                    viewModel.Response = new ResponseCore{
                        Success = response.Success,
                        Message = response.Message,
                    };
                    
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
                    // return View(company);
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
        public async Task<IActionResult> Edit(string id, CompanyViewModel viewModel)
        {
            try
            {
                log.LogInformation($"Company edit requested [{viewModel.Company.Id}]");

                var response = await service.UpsertAsync(viewModel.Company);
                if (response.Success)
                {
                    log.LogInformation($"Company edit completed [{viewModel.Company.Id}]");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    log.LogWarning($"Company edit could not be completed [{viewModel.Company.Id}] - [{response.Message}]");
                    viewModel.Company = response.Model;
                    viewModel.Response = new ResponseCore{
                        Success = response.Success,
                        Message = response.Message,
                    };
                    
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
