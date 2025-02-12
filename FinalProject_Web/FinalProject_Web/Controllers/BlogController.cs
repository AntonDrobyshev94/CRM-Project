using Models.DataServiceModels;
using BusinessLogicService.Interfaces;
using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogData _blogData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        private readonly IImageData _imageData;
        public BlogController(IBlogData blogData, 
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate,
            IImageData imageData)
        {
            _blogData = blogData;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
            _imageData = imageData;
        }

        #region Blog
        /// <summary>
        /// Метод перехода во View блога.
        /// В качестве модели во View передаётся
        /// коллекция List<BlogModel> (запрос в API
        /// с помощью метода _applicationData.GetBlog)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Blog()
        {
            IEnumerable<BlogModel> blogs = await _blogData.GetBlog("https://localhost:7037/api/blog");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: true);
            CompositeViewNumerableModel<BlogModel> viewModel = new CompositeViewNumerableModel<BlogModel>()
            {
                ContentViewModel = blogs,
                LayoutViewModel = layout,
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод перехода во View добавления поста в блог
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> AddBlogWindow()
        {
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            SimpleViewModel viewModel = new SimpleViewModel()
            {
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод добавления поста, в котором создаётся
        /// уникальное имя для файла картинки поста и
        /// происходит загрузка картинки с помощью метода
        /// UploadImageMethod, а также с помощью метода
        /// _applicationData.AddBlog происходит создание
        /// нового поста в блоге с параметрами, переданными 
        /// в экземпляре BlogModel.
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="blogPost"></param>
        [HttpPost]
        public async Task<IActionResult> AddBlog(IFormFile imageFile, string name, string description, string blogPost,
            string requestId)
        {
            try
            {
                if (imageFile == null || string.IsNullOrEmpty(name) ||
                    string.IsNullOrEmpty(description) || string.IsNullOrEmpty(blogPost))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Изображение или описание отсутствуют!"
                    });
                }
                var uniqueFileName = Path.GetRandomFileName() + Path.GetExtension(imageFile.FileName);
                BlogModel newBlog = new BlogModel
                {
                    Name = name,
                    Description = description,
                    BlogPost = blogPost,
                    ImageName = uniqueFileName,
                    DateTimePublication = DateTime.Now
                };
                ImageModel newImage = new ImageModel()
                {
                    UniqueName = uniqueFileName,
                    TypeOfModel = newBlog.GetType().Name,
                    ImageByteArray = await _imageData.ImageToByteArrayMethod(imageFile)
                };
                CommonWithImageModel<BlogModel> commonModel = new CommonWithImageModel<BlogModel>
                {
                    CommonModel = newBlog,
                    ImgModel = newImage
                };
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _blogData.AddBlog(commonModel, token, 
                    "https://localhost:7037/api/blog", requestId);
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
            catch (HttpRequestException)
            {
                return Json(new
                {
                    sucess = false,
                    message = "Ошибка на стороне сервера, блог не был добавлен",
                    redirectUrl = Url.Action("Blog", "Blog")
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
        /// Метод перехода во View изменения поста в блоге, в
        /// котором происходит поиск конкретного поста с помощью
        /// метода FindBlogById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        public async Task<IActionResult> ChangeBlogPostWindow(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            BlogModel concretePost = await _blogData.FindBlogById(token, $"https://localhost:7037/api/blog/{id}");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            CompositeViewModel<BlogModel> viewModel = new CompositeViewModel<BlogModel>()
            {
                ContentViewModel = concretePost,
                LayoutViewModel = layout,
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод изменения поста, в котором происходит загрузка новой картинки
        /// с помощью метода UploadImageMethod, а также изменение поста
        /// методом _applicationData.ChangeBlog
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="imageName"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <param name="blogPost"></param>
        [HttpPost]
        public async Task<IActionResult> ChangeBlogPost(IFormFile imageFile, string imageName, string name, string description, int id, string blogPost,
            string requestId)
        {
            try
            {
                if (imageFile == null || string.IsNullOrEmpty(name) ||
                string.IsNullOrEmpty(description) || string.IsNullOrEmpty(blogPost))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Изображение или описание отсутствуют!"
                    });
                }
                BlogModel newBlog = new BlogModel
                {
                    Id = id,
                    Name = name,
                    Description = description,
                    BlogPost = blogPost,
                    ImageName = imageName,
                    DateTimePublication = DateTime.Now
                };
                ImageModel newImage = new ImageModel()
                {
                    UniqueName = imageName,
                    TypeOfModel = newBlog.GetType().Name,
                    ImageByteArray = await _imageData.ImageToByteArrayMethod(imageFile)
                };
                CommonWithImageModel<BlogModel> commonModel = new CommonWithImageModel<BlogModel>
                {
                    CommonModel = newBlog,
                    ImgModel = newImage
                };
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _blogData.ChangeBlog(commonModel, token, 
                    "https://localhost:7037/api/blog/ChangeBlog", requestId);
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
            catch (HttpRequestException)
            {
                return Json(new
                {
                    sucess = false,
                    message = "Ошибка на стороне сервера, блог не был добавлен",
                    redirectUrl = Url.Action("Blog", "Blog")
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Json(new
                {
                    sucess = false,
                    message = "Непредвиденная ошибка"
                });
            }
        }

        /// <summary>
        /// Метод перехода во View "детали поста в блоге" 
        /// (открывает конкретный пост в блоге). В методе
        /// происходит поиск конкретного поста с помощью
        /// метода FindBlogById по передаваемому в него id и
        /// кеширование этого экземпляра. Далее происходит
        /// возвращение экземпляра в качестве модели во View.
        /// </summary>
        /// <param name="id"></param>
        [HttpGet]
        public async Task<IActionResult> BlogPostDetails(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            BlogModel concretePost = await _blogData.FindBlogById(token, $"https://localhost:7037/api/blog/{id}");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: false);
            CompositeViewModel<BlogModel> viewModel = new CompositeViewModel<BlogModel>()
            {
                ContentViewModel = concretePost,
                LayoutViewModel = layout
            };
            return View(viewModel);
        }

        /// <summary>
        /// Метод удаления блога. Метод принимает int значение id 
        /// блога. В методе происходит сохранение в экземпляр BlogModel 
        /// результата запроса FindLinkById по указанному Id блога.
        /// Из полученного экземпляра в переменную uniqueFileName
        /// записывается параметр ImageName с целью дальнейшего удаления
        /// файла картинки из папки с помощью метода 
        /// System.IO.File.Delete(filePath) (при условии, что фал картинки
        /// существует). Далее с помощью метода _applicationData.DeleteBlog
        /// происходит запрос на удаление блога в БД. 
        /// </summary>
        /// <param name="id"></param>
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            await _blogData.DeleteBlog(token, $"https://localhost:7037/api/blog/{id}");
            return RedirectToAction("Blog");
        }
        #endregion
    }
}
