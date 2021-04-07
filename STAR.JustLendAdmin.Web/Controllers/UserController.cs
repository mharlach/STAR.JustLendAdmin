﻿#nullable enable

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
    public class UserController : Controller
    {
        private readonly IUserService userService;
        private readonly ICompanyService companyService;
        private readonly ILogger<UserController> log;

        public UserController(IUserService userService, ICompanyService companyService, ILogger<UserController> log)
        {
            this.userService = userService;
            this.companyService = companyService;
            this.log = log;
        }

        // GET: UsersController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel viewModel)
        {
            try
            {
                var response = await userService.UpsertAsync(viewModel.User);
                if (response.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    viewModel.User = response.Model;
                    viewModel.Response = new ResponseCore { Message = response.Message, Success = response.Success };
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error creating user");
                return View("Error", ex);
            }
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var user = await userService.GetAsync(id);
                if (user != null)
                {
                    return View(new UserViewModel { User = user });
                }
                else
                {
                    return NotFound(id);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error getting user");
                return View("Error", ex);
            }
        }

        // POST: CompanyController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel viewModel)
        {
            try
            {
                var response = await userService.UpsertAsync(viewModel.User);
                if (response.Success)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    viewModel.User = response.Model;
                    viewModel.Response = new ResponseCore { Message = response.Message, Success = response.Success } ;
                    return View(viewModel);
                }
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error editing user");
                return View("Error", ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CompanyFilterOptions()
        {
            var companies = await companyService.GetAsync();
            return PartialView("~/Views/User/CompanyOptionsPartialView.cshtml", companies);
        }

        [HttpPost]
        public async Task<IActionResult> GetAllCompanyUsersPartialView(UserSearchRequest request)
        {
            var users = await userService.GetAsync(request);
            return PartialView("UsersGridPartialView", users.ToList());
        }

        [HttpPost]
        public async Task<IActionResult> GetFilteredUsersPartialView(UserSearchFilterRequest request)
        {
            var users = await userService.GetAsync(request);
            return PartialView("UsersGridPartialView", users.ToList());
        }
    }
}
