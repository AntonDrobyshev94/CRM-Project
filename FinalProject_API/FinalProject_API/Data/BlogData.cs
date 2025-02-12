using FinalProject_API.ContextFolder;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по
    /// взаимодействию с блогом через БД
    /// </summary>
    public class BlogData
    {
        private DbContextOptions<DataContext> _options;
        private readonly DataContext _context;
        private readonly ImageData _imageData;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ServiceData"/> с инжекцией 
        /// зависимостей для <see cref="DataContext"/>.
        /// </summary>
        /// <param name="context">Экземпляр <see cref="DataContext"/>, используемый 
        /// для взаимодействия с базой данных.</param>
        /// <param name="options">Экземпляр <see cref="DbContextOptions{DataContext}"/>, 
        /// используемый для для взаимодействия с базой данных</param>
        /// <param name="imageData">Экземпляр <see cref="ImageData"/>, используемый 
        /// для взаимодействия с репозиторием-логикой обработки изображений ImageData.</param>
        public BlogData(DbContextOptions<DataContext> options,
            DataContext context, ImageData imageData)
        {
            _options = options;
            _context = context;
            _imageData = imageData;
        }

        #region Blog
        /// <summary>
        /// Метод добавления блога, посредством
        /// обращения к методу Add datacontext BlogModel.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        /// <param name="blog"></param>
        public async Task<IActionResult> AddBlog(CommonWithImageModel<BlogModel> blog)
        {
            try
            {
                using (var context = new DataContext(_options))
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        BlogModel blogModel = blog.CommonModel;
                        ImageModel imgModel = blog.ImgModel;
                        if (await context.Blogs.AnyAsync(p => p.ImageName == blogModel.ImageName))
                        {
                            string extension = "";
                            int index = blogModel.ImageName.IndexOf('.');
                            if (index != -1)
                            {
                                extension = blogModel.ImageName.Substring(index);
                                var uniqueFileName = Path.GetRandomFileName() + extension;
                                blogModel.ImageName = uniqueFileName;
                                imgModel.UniqueName = uniqueFileName;
                            }
                            else
                            {
                                Console.WriteLine("Расширение в имени объекта отсутствует");
                            }
                        }
                        await context.Blogs.AddAsync(blogModel);
                        await _imageData.AddToDBImage(imgModel, context);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new StatusCodeResult(200);
                    }
                }
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }  
        }

        /// <summary>
        /// Метод изменения блога, принимающий модель блога
        /// BlogModel. В методе используется директива using 
        /// для определения границ текущего контекста для избежания 
        /// ошибки ObjectDisposedException. В директиве создаётся
        /// экземпляр BlogModel, в который асинхронно, с помощью
        /// метода FirstOrDefaultAsync, записывается результат
        /// перебора таблицы Blogs на предмет совпадения id
        /// принимаемой модели и id хранящегося в базе данных блога.
        /// Параметры полученного экземпляра перезаписываются на 
        /// параметры принимаемой методом модели.
        /// По окончанию производится сохранение методом SaveChanges.
        /// </summary>
        /// <param name="blog"></param>
        public async Task<IActionResult> ChangeBlog(CommonWithImageModel<BlogModel> blog)
        {
            try
            {
                BlogModel blogModel = blog.CommonModel;
                ImageModel imgModel = blog.ImgModel;
                using (var context = new DataContext(_options))
                {
                    using (var transaction = await context.Database.BeginTransactionAsync())
                    {
                        BlogModel? concreteBlog = await context.Blogs.FirstOrDefaultAsync(x => x.Id == blogModel.Id);
                        if (concreteBlog != null)
                        {
                            concreteBlog.Description = blogModel.Description;
                            concreteBlog.BlogPost = blogModel.BlogPost;
                            concreteBlog.Name = blogModel.Name;
                            concreteBlog.ImageName = blogModel.ImageName;
                            await _imageData.ChangeDBImage(imgModel, context);
                            await context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        return new StatusCodeResult(200);
                    }
                }
            }
            catch (Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Метод, возаращающий последовательность коллекции объектов, 
        /// реализующих интерфейс IBlogModel с помощью интерфейса 
        /// IEnumberable
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IBlogModel>> GetBlog()
        {
            return await this._context.Blogs.ToListAsync();
        }

        /// <summary>
        /// Асинхронный метод удаления блога невозвращаемого
        /// типа, который принимает в себя int значение Id 
        /// удаляемого блога. В методе используется
        /// директива using для определения границ текущего
        /// контекста для избежания ошибки ObjectDisposedException.
        /// Происходит создание экземпляра BlogModel, в который
        /// записывается результат перебора таблицы Blogs
        /// базы данных на предмет совпадения принимаемого id
        /// с id блога с помощью метода FirstOrDefaultAsync.
        /// При условии, что экземпляр blog не равен нулю
        /// из таблицы Blogs происходит удаление полученного
        /// экземпляра blog с помощью метода Remove с последующим
        /// сохранением базы данных методом SaveChangesAsync.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteBlog(int id)
        {
            using (var context = new DataContext(_options))
            {
                BlogModel? blog = await context.Blogs.FirstOrDefaultAsync(x => x.Id == id);
                if (blog != null)
                {
                    context.Blogs.Remove(blog);
                    await context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Асинхронный метод поиска блога, принимающий int id
        /// блога и возвращающий экземпляр блога BlogModel. 
        /// Метод представлен лямбда выражением, в котором 
        /// происходит перебор таблицы Blogs текущей базы данных
        /// на предмет совпадения Id блога с принимаемым Id с 
        /// помощью метода FirstOrDefaultAsync. В итоге происходит
        /// возвращение полученного экземпляра BlogModel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BlogModel> GetBlogByID(int id) => await _context.Blogs.FirstOrDefaultAsync(x => x.Id == id);
        #endregion
    }
}
