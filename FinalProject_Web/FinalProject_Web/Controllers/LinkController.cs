using BusinessLogicService.Interfaces;
using FinalProject_Web.Interfaces;
using Models.DataServiceModels;
using FinalProject_Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Controllers
{
    public class LinkController : Controller
    {
        private readonly ILinkData _linkData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        private readonly IImageData _imageData;
        public LinkController(ILinkData linkData,
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate,
            IImageData imageData)
        {
            _linkData = linkData;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
            _imageData = imageData;
        }
        #region ContactLinks

        /// <summary>
        /// Метод перехода во View добавления ссылки
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddLinkWindow()
        {
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            SimpleViewModel viewModel = new SimpleViewModel()
            {
                LayoutViewModel = layout
            };
            return PartialView(viewModel);
        }

        /// <summary>
        /// Метод добавления ссылки, в котором создаётся
        /// уникальное имя для файла картинки ссылки и
        /// происходит загрузка картинки с помощью метода
        /// UploadImageMethod, а также с помощью метода
        /// _applicationData.AddLink происходит создание
        /// новой ссылки с параметрами, переданными в экземпляре
        /// LinkModel.
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddLink(IFormFile imageFile, string url)
        {
            try
            {
                if (imageFile == null || string.IsNullOrEmpty(url))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Изображение или url отсутствуют!"
                    });
                }
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                LinkModel newLink = new LinkModel
                {
                    ImageName = uniqueFileName,
                    Url = url
                };
                ImageModel newImage = new ImageModel()
                {
                    UniqueName = uniqueFileName,
                    ImageByteArray = await _imageData.ImageToByteArrayMethod(imageFile),
                    TypeOfModel = newLink.GetType().Name
                };
                CommonWithImageModel<LinkModel> commonModel = new CommonWithImageModel<LinkModel>()
                {
                    CommonModel = newLink,
                    ImgModel = newImage
                };
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _linkData.AddLink(commonModel, token, "https://localhost:7037/api/link");
                return Json(new
                {
                    success = true
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new
                {
                    sucess = false,
                    message = "Ваш токен недействителен",
                    redirectUrl = Url.Action("Logout", "Account")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new
                {
                    sucess = false,
                    message = $"Непредвиденная ошибка {ex}"
                });
            }
        }

        /// <summary>
        /// Метод перехода на View изменения ссылки, в котором
        /// происходит поиск и кеширование модели ссылки с
        /// помощью метода FindLinkById по id, который принимается
        /// методом. По окончанию экземпляр ссылки передаётся
        /// во View в качестве модели.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeLinkWindow(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            LinkModel concreteLink = await _linkData.FindLinkById(token, $"https://localhost:7037/api/link/{id}");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            CompositeViewModel<LinkModel> viewModel = new CompositeViewModel<LinkModel>()
            {
                ContentViewModel = concreteLink,
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод изменения ссылки, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение ссылки
        /// методом _applicationData.ChangeLink
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="url"></param>
        /// <param name="imageName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeLink(IFormFile imageFile, string url, string imageName, int id)
        {
            try
            {
                if (imageFile == null || string.IsNullOrEmpty(url))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Изображение или url отсутствуют!"
                    });
                }
                LinkModel newLink = new LinkModel
                {
                    Id = id,
                    ImageName = imageName,
                    Url = url
                };
                ImageModel newImage = new ImageModel()
                {
                    UniqueName = imageName,
                    ImageByteArray = await _imageData.ImageToByteArrayMethod(imageFile),
                    TypeOfModel = newLink.GetType().Name
                };
                CommonWithImageModel<LinkModel> commonModel = new CommonWithImageModel<LinkModel>()
                {
                    CommonModel = newLink,
                    ImgModel = newImage
                };
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _linkData.ChangeLink(commonModel, token, "https://localhost:7037/api/link/ChangeLink");
                return Json(new
                {
                    success = true
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Json(new
                {
                    sucess = false,
                    message = "Ваш токен недействителен",
                    redirectUrl = Url.Action("Logout", "Account")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new
                {
                    sucess = false,
                    message = $"Непредвиденная ошибка {ex}"
                });
            }
        }
        /// <summary>
        /// Метод удаления ссылки. Метод принимает int значение id 
        /// ссылки. В методе происходит сохранение в экземпляр LinkModel 
        /// результата запроса FindLinkById по указанному Id ссылки.
        /// Из полученного экземпляра в переменную uniqueFileName
        /// записывается параметр ImageName с целью дальнейшего удаления
        /// файла картинки из папки с помощью метода 
        /// System.IO.File.Delete(filePath) (при условии, что фал картинки
        /// существует). Далее с помощью метода _applicationData.DeleteLink
        /// происходит запрос на удаление ссылки в БД. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteLink(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            await _linkData.DeleteLink(token, $"https://localhost:7037/api/link/{id}");
            return RedirectToAction("Contacts", "Contact");
        }
        #endregion
    }
}
