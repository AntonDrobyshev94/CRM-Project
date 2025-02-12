using FinalProject_Web.Filters;
using BusinessLogicService.Interfaces;
using Models.DataServiceModels;
using FinalProject_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Controllers
{
    public class DekstopController : Controller
    {
        private readonly IApplicationData _applicationData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        private readonly IAccountData _accountData;
        public DekstopController(IApplicationData applicationData,
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate,
            IAccountData accountData)
        {
            _applicationData = applicationData;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
            _accountData = accountData;
        }

        [CheckToken]
        [ApiEndpoint("/api/account/")]
        [HttpGet]
        public async Task<IActionResult> DekstopWindow()
        {
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            SimpleViewModel viewModel = new SimpleViewModel()
            {
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        [HttpGet]
        public async Task<JsonResult> GetFilteredRequests(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.AddDays(1).AddSeconds(-1);
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            var applications = await _applicationData.GetApplications(token, "https://localhost:7037/api/values");
            var filteredApplications = applications
                .Where(app => app.Date >= startDate && app.Date <= endDate)
                .Select(app => new {
                    app.Id,
                    Date = app.Date.ToString("dd.MM.yyyy HH:mm"),
                    app.Name,
                    app.Message,
                    app.EMail,
                    app.Status
                })
                .ToList();

            return Json(new
            {
                allRequestCounter = applications.Count(),
                currentRequestCounter = filteredApplications.Count(),
                applications = filteredApplications
            });
        }

        [HttpPost]
        public IActionResult SetDateFilter(DateTime startDate, DateTime endDate)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult SetPeriodFilter(string period)
        {
            return Ok();
        }

        /// <summary>
        /// Метод изменения статуса заявки, принимающий строковую переменную
        /// заявки, значение id заявки и остальные переменные (их значения
        /// не учитываются). Изменение статуса происходит с помощью метода
        /// взаимодействия с API ChangeApplicationStatus
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeRequestStatus(string status, int id)
        {
            try
            {
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _applicationData.ChangeApplicationStatus(status, id, token, "https://localhost:7037/api/values/ChangeStatus");
                return Ok();
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new
                {
                    redirectToUrl = Url.Action("Logout", "Account")
                });
            }
        }
    }
}
