using ServicesLibrary.Enums;
using FinalProject_Web.Interfaces;
using Models.DataServiceModels;
using BusinessLogicService.Interfaces;
using FinalProject_Web.Services.Interfaces;
using ServicesLibrary.Vars;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Controllers
{
    public class ContactController : Controller
    {
        private readonly IImageData _imageData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        private readonly IContactData _contactData;
        private readonly ILinkData _linkData;
        private readonly IImageDataService _imageDataService;
        public ContactController(IImageData imageData, IImageDataService imageDataService,
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate,
            IContactData contactData, ILinkData linkData)
        {
            _imageData = imageData;
            _imageDataService = imageDataService;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
            _contactData = contactData;
            _linkData = linkData;
        }
        #region Contacts
        /// <summary>
        /// Метод для перехода во View меню Контакты.
        /// В качестве модели во View передаётся коллекция объектов
        /// List<LinkModel>
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Contacts()
        {
            ViewBag.Contacts = Variables.Contacts;
            IEnumerable<LinkModel> links = await _linkData.GetLinks("https://localhost:7037/api/link");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: true);
            CompositeViewNumerableModel<LinkModel> viewModel = new CompositeViewNumerableModel<LinkModel>()
            {
                ContentViewModel = links,
                LayoutViewModel = layout,
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод для перехода во View изменения контакта. Во
        /// View метода в качестве модели передаётся экземпляр
        /// Contacts.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeContactWindow()
        {
            IEnumerable<LinkModel> links = await _linkData.GetLinks("https://localhost:7037/api/link");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            CompositeViewNumerableModel<LinkModel> viewModel = new CompositeViewNumerableModel<LinkModel>()
            {
                ContentViewModel = links,
                LayoutViewModel = layout,
            };
            ViewBag.Contacts = Variables.Contacts;
            return PartialView(viewModel);
        }

        /// <summary>
        /// Метод изменения контакта, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение проекта
        /// методом _applicationData.ChangeContacts
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="fileName"></param>
        /// <param name="address"></param>
        /// <param name="email"></param>
        /// <param name="fax"></param>
        /// <param name="telephone"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeContact(IFormFile imageFile, string fileName, string address,
            string email, string fax, string telephone)
        {
            if (imageFile == null || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(telephone) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fax))
            {
                return Json(new
                {
                    success = false,
                    message = "Изображение или описание отсутствуют!"
                });
            }
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            var multipartModel = await _imageData.ConvertFileToMultipartForm(imageFile, "ContactsMap.png");
            var result = await _imageDataService.UploadImageAsync(multipartModel, token, "https://localhost:7037/api/image");
            switch (result)
            {
                case UploadImageResult.Ok:
                    Variables.Contacts = new Contacts
                    {
                        Address = address,
                        Email = email,
                        Id = 0,
                        Fax = fax,
                        Telephone = telephone
                    };
                    var contactResult = await _contactData.ChangeContacts(Variables.Contacts, token, "https://localhost:7037/api/contact");
                    switch (contactResult)
                    {
                        case ChangeContactsResult.Ok:
                            return Json(new
                            {
                                success = true
                            });
                        case ChangeContactsResult.Unauthorized:
                            return Json(new
                            {
                                sucess = false,
                                message = "Ваш токен недействителен",
                                redirectUrl = Url.Action("Logout", "Account")
                            });
                        case ChangeContactsResult.BadRequest:
                        default:
                            return Json(new
                            {
                                sucess = false,
                                message = $"Непредвиденная ошибка"
                            });
                    }
                case UploadImageResult.Unauthorized:
                    return Json(new
                    {
                        sucess = false,
                        message = "Ваш токен недействителен",
                        redirectUrl = Url.Action("Logout", "Account")
                    });
                case UploadImageResult.BadRequest:
                default:
                    return Json(new
                    {
                        sucess = false,
                        message = $"Непредвиденная ошибка"
                    });
            }
        }
        #endregion
    }
}
