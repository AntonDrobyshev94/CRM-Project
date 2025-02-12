using BusinessLogicService.Interfaces;
using Models.DataServiceModels;
using FinalProject_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Controllers
{
    public class ServiceController : Controller
    {
        private readonly IServiceData _serviceData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        public ServiceController(IServiceData serviceData,
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate)
        {
            _serviceData = serviceData;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
        }

        #region Services
        /// <summary>
        /// Метод перехода во View меню Услуги.
        /// В качестве модели во View передаётся коллекция
        /// услуг List<Service>, получаемая по запросу в API
        /// с помощью метода _applicationData.GetServices.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Services()
        {
            IEnumerable<Service> services = await _serviceData.GetServices("https://localhost:7037/api/service");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: true);
            CompositeViewNumerableModel<Service> viewModel = new CompositeViewNumerableModel<Service>()
            {
                ContentViewModel = services,
                LayoutViewModel = layout,
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод перехода во View добавления услуги
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddServiceWindow()
        {
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            SimpleViewModel viewModel = new SimpleViewModel()
            {
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод добавления услуги, в котором с помощью метода
        /// _applicationData.AddService происходит создание
        /// нового проекта в блоге с параметрами, переданными 
        /// в экземпляре Service.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddService(string name, string description)
        {
            var model = new Service
            {
                Name = name,
                Description = description
            };
            if (ModelState.IsValid)
            {
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _serviceData.AddService(model, token, "https://localhost:7037/api/service");
            }
            return RedirectToAction("Services");
        }

        /// <summary>
        /// Метод удаления услуги. Метод принимает int значение id 
        /// услуги и с помощью метода _applicationData.DeleteService
        /// происходит запрос на удаление услуги в БД. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteService(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            await _serviceData.DeleteService(token, $"https://localhost:7037/api/service/{id}");
            return RedirectToAction("Services");
        }

        /// <summary>
        /// Метод перехода во View изменения услуги, в
        /// котором происходит поиск конкретной услуги с помощью
        /// метода FindServiceById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeServiceWindow(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            Service concreteService = await _serviceData.FindServiceById(token, $"https://localhost:7037/api/service/{id}");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            CompositeViewModel<Service> viewModel = new CompositeViewModel<Service>()
            {
                ContentViewModel = concreteService,
                LayoutViewModel = layout,
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод изменения услуги, в котором происходит изменение услуги
        /// методом _applicationData.ChangeService
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeService(string name, string description, int id)
        {
            Service service = new Service()
            {
                Id = id,
                Description = description,
                Name = name
            };
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            await _serviceData.ChangeService(service, token, "https://localhost:7037/api/service/ChangeService");
            return RedirectToAction("Services");
        }
        #endregion
    }
}
