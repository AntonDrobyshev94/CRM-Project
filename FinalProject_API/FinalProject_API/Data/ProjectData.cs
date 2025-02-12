using FinalProject_API.ContextFolder;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по
    /// взаимодействию с проектами через БД
    /// </summary>
    public class ProjectData
    {
        private DbContextOptions<DataContext> _options;
        private readonly ImageData _imageData;
        private readonly DataContext _context;
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
        public ProjectData(DbContextOptions<DataContext> options,
            ImageData imageData, DataContext context)
        {
            _options = options;
            _imageData = imageData;
            _context = context;
        }
        #region Projects
        /// <summary>
        /// Метод добавления проекта, посредством
        /// обращения к методу Add datacontext ProjectModel.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        /// <param name="project"></param>
        public async Task AddProjects(CommonWithImageModel<ProjectModel> project)
        {
            using (var context = new DataContext(_options))
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    ProjectModel projectModel = project.CommonModel;
                    ImageModel imgModel = project.ImgModel;
                    if (await context.Projects.AnyAsync(p => p.ImageName == projectModel.ImageName))
                    {
                        string extension = Path.GetExtension(projectModel.ImageName);
                        if (!string.IsNullOrEmpty(extension))
                        {
                            var uniqueFileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + extension;
                            projectModel.ImageName = uniqueFileName;
                            imgModel.UniqueName = uniqueFileName;
                        }
                        else
                        {
                            Console.WriteLine("Расширение в имени объекта отсутствует");
                        }
                    }
                    await context.Projects.AddAsync(projectModel);
                    await _imageData.AddToDBImage(imgModel, context);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
        }

        /// <summary>
        /// Метод изменения проекта, принимающий модель проекта
        /// ProjectModel. В методе используется директива using 
        /// для определения границ текущего контекста для избежания 
        /// ошибки ObjectDisposedException. В директиве создаётся
        /// экземпляр ProjectModel, в который асинхронно, с помощью
        /// метода FirstOrDefaultAsync, записывается результат
        /// перебора таблицы Projects на предмет совпадения id
        /// принимаемой модели и id хранящегося в базе данных проекта.
        /// Параметры полученного экземпляра перезаписываются на 
        /// параметры принимаемой методом модели.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        /// <param name="project"></param>
        public async Task ChangeProjects(CommonWithImageModel<ProjectModel> project)
        {
            ProjectModel projectModel = project.CommonModel;
            ImageModel imgModel = project.ImgModel;

            using (var context = new DataContext(_options))
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    ProjectModel? concreteProject = await context.Projects.FirstOrDefaultAsync(x => x.Id == projectModel.Id);
                    if (concreteProject != null)
                    {
                        concreteProject.ImageName = projectModel.ImageName;
                        concreteProject.Name = projectModel.Name;
                        concreteProject.Description = projectModel.Description;
                        await _imageData.ChangeDBImage(imgModel, context);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    else
                    {
                        Console.WriteLine("Ошибка изменения проекта");
                    }
                }
            }
        }

        /// <summary>
        /// Метод, возаращающий последовательность коллекции объектов, 
        /// реализующих интерфейс IProjectModel с помощью интерфейса 
        /// IEnumberable
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IProjectModel>> GetProjects()
        {
             return await this._context.Projects.ToListAsync();
        }

        /// <summary>
        /// Асинхронный метод удаления проекта невозвращаемого
        /// типа, который принимает в себя int значение Id 
        /// удаляемого проекта. В методе используется
        /// директива using для определения границ текущего
        /// контекста для избежания ошибки ObjectDisposedException.
        /// Происходит создание экземпляра ProjectModel, в который
        /// записывается результат перебора таблицы Projects
        /// базы данных на предмет совпадения принимаемого id
        /// с id проекта с помощью метода FirstOrDefaultAsync.
        /// При условии, что экземпляр ProjectModel не равен нулю
        /// из таблицы Projects происходит удаление полученного
        /// экземпляра с помощью метода Remove с последующим
        /// сохранением базы данных методом SaveChangesAsync.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteProject(int id)
        {
            using (var context = new DataContext(_options))
            {
                ProjectModel? project = await context.Projects.FirstOrDefaultAsync(x => x.Id == id);
                if (project != null)
                {
                    context.Projects.Remove(project);
                    ImageModel? img = await context.Images.FirstOrDefaultAsync(p => p.UniqueName == project.ImageName);
                    if (img != null)
                    {
                        context.Images.Remove(img);
                    }
                    await context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Ошибка удаления проекта");
                }
            }
        }

        /// <summary>
        /// Асинхронный метод поиска проекта, принимающий int id
        /// проекта и возвращающий экземпляр проекта ProjectModel. 
        /// Метод представлен лямбда выражением, в котором 
        /// происходит перебор таблицы Projects текущей базы данных
        /// на предмет совпадения Id проекта с принимаемым Id с 
        /// помощью метода FirstOrDefaultAsync. В итоге происходит
        /// возвращение полученного экземпляра ProjectModel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProjectModel> GetProjectByID(int id) => await _context.Projects.FirstOrDefaultAsync(x => x.Id == id);
        #endregion
    }
}
