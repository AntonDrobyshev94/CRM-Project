using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.Interfaces;
using Models.DataServiceModels;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicService.Interfaces;

namespace FinalProject_Web.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectData _projectData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        private readonly IImageData _imageData;
        public ProjectController(IProjectData projectData,
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate,
            IImageData imageData)
        {
            _projectData = projectData;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
            _imageData = imageData;
        }
        #region Projects
        /// <summary>
        /// Метод перехода во View меню Проектов.
        /// В качестве модели во View передаётся коллекция
        /// проектов List<ProjectModel>, получаемая из API
        /// методом _projectDataApi.GetProjects
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Projects()
        {
            IEnumerable<ProjectModel> projects = await _projectData.GetProjects("https://localhost:7037/api/project");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: true);
            CompositeViewNumerableModel<ProjectModel> viewModel = new CompositeViewNumerableModel<ProjectModel>()
            {
                ContentViewModel = projects,
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод перехода во View добавления проекта
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddProjectWindow()
        {
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            SimpleViewModel viewModel = new SimpleViewModel()
            {
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод добавления проекта, в котором создаётся
        /// уникальное имя для файла картинки проекта и
        /// происходит загрузка картинки с помощью метода
        /// UploadImageMethod, а также с помощью метода
        /// _projectDataApi.AddProject происходит создание
        /// нового проекта в блоге с параметрами, переданными 
        /// в экземпляре ProjectModel.
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProject(IFormFile imageFile, string name, string description)
        {
            try
            {
                if (imageFile == null || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Изображение или описание отсутствуют!"
                    });
                }
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                ProjectModel newProject = new ProjectModel
                {
                    Name = name,
                    Description = description,
                    ImageName = uniqueFileName
                };
                ImageModel newImage = new ImageModel()
                {
                    UniqueName = uniqueFileName,
                    TypeOfModel = newProject.GetType().Name,
                    ImageByteArray = await _imageData.ImageToByteArrayMethod(imageFile)
                };
                CommonWithImageModel<ProjectModel> commonModel = new CommonWithImageModel<ProjectModel>
                {
                    CommonModel = newProject,
                    ImgModel = newImage
                };
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _projectData.AddProject(commonModel, token, "https://localhost:7037/api/project");
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
        /// Метод перехода во View изменения проекта, в
        /// котором происходит поиск конкретного проекта с помощью
        /// метода FindProjectById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeProjectWindow(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            ProjectModel concreteProject = await _projectData.FindProjectById(token, $"https://localhost:7037/api/project/{id}");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            CompositeViewModel<ProjectModel> viewModel = new CompositeViewModel<ProjectModel>()
            {
                ContentViewModel = concreteProject,
                LayoutViewModel = layout,
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод изменения проекта, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение проекта
        /// методом _projectDataApi.ChangeProject
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="imageName"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ChangeProject(IFormFile imageFile, string imageName, string name, string description, int id)
        {
            try
            {
                if (imageFile == null || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Изображение или описание отсутствуют!"
                    });
                }
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                ProjectModel newProject = new ProjectModel
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    ImageName = imageName
                };
                ImageModel newImage = new ImageModel()
                {
                    UniqueName = imageName,
                    TypeOfModel = newProject.GetType().Name,
                    ImageByteArray = await _imageData.ImageToByteArrayMethod(imageFile)
                };
                CommonWithImageModel<ProjectModel> commonModel = new CommonWithImageModel<ProjectModel>
                {
                    CommonModel = newProject,
                    ImgModel = newImage
                };
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _projectData.ChangeProject(commonModel, token, "https://localhost:7037/api/project/ChangeProject");
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
        /// Метод перехода во View "детали проекта" 
        /// (открывает конкретный проект). В методе
        /// происходит поиск конкретного проекта с помощью
        /// метода FindProjectById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ProjectDetails(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            ProjectModel concreteProject = await _projectData.FindProjectById(token, $"https://localhost:7037/api/project/{id}");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            CompositeViewModel<ProjectModel> viewModel = new CompositeViewModel<ProjectModel>()
            {
                ContentViewModel = concreteProject,
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод удаления проекта. Метод принимает int значение id 
        /// проекта. В методе происходит сохранение в экземпляр ProjectModel 
        /// результата запроса FindProjectById по указанному Id проекта.
        /// Из полученного экземпляра в переменную uniqueFileName
        /// записывается параметр ImageName с целью дальнейшего удаления
        /// файла картинки из папки с помощью метода 
        /// System.IO.File.Delete(filePath) (при условии, что фал картинки
        /// существует). Далее с помощью метода _projectDataApi.DeleteProject
        /// происходит запрос на удаление проекта в БД. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteProject(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            await _projectData.DeleteProject(token, $"https://localhost:7037/api/project/{id}");
            return RedirectToAction("Projects");
        }
        #endregion
    }
}
