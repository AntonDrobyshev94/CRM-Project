using FinalProject_API.ContextFolder;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по
    /// взаимодействию со ссылками через БД
    /// </summary>
    public class LinkData
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
        public LinkData(DbContextOptions<DataContext> options, 
            DataContext context, ImageData imageData)
        {
            _options = options;
            _context = context;
            _imageData = imageData;
        }

        #region Links
        /// <summary>
        /// Метод добавления ссылки, посредством
        /// обращения к методу Add datacontext LinkModel.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        /// <param name="link"></param>
        public async Task AddLink(CommonWithImageModel<LinkModel> link)
        {
            using (var context = new DataContext(_options))
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    LinkModel linkModel = link.CommonModel;
                    ImageModel imgModel = link.ImgModel;
                    if (await context.Links.AnyAsync(p => p.ImageName == linkModel.ImageName))
                    {
                        string extension = "";
                        int index = linkModel.ImageName.IndexOf('.');
                        if (index != -1)
                        {
                            extension = linkModel.ImageName.Substring(index);
                            var uniqueFileName = Path.GetRandomFileName() + extension;
                            linkModel.ImageName = uniqueFileName;
                            imgModel.UniqueName = uniqueFileName;
                        }
                        else
                        {
                            Console.WriteLine("Расширение в имени объекта отсутствует");
                        }
                    }
                    await context.Links.AddAsync(linkModel);
                    await _imageData.AddToDBImage(imgModel, context);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
        }

        /// <summary>
        /// Метод изменения ссылки, принимающий модель LinkModel.
        /// В методе используется директива using для определения 
        /// границ текущего контекста для избежания ошибки 
        /// ObjectDisposedException. Внутри директивы using создается
        /// экземпляр LinkModel и с помощью метода
        /// FirstOrDefaultAsync осуществляется поиск по совпадению
        /// Id принимаемой модели и элемента LinkModel таблицы Links.
        /// Параметры полученного экземпляра перезаписываются на 
        /// параметры принимаемой методом модели.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        /// <param name="link"></param>
        public async Task ChangeLink(CommonWithImageModel<LinkModel> link)
        {
            LinkModel linkModel = link.CommonModel;
            ImageModel imgModel = link.ImgModel;
            using (var context = new DataContext(_options))
            {
                using (var transaction = await context.Database.BeginTransactionAsync())
                {
                    LinkModel? concreteLink = await context.Links.FirstOrDefaultAsync(x => x.Id == linkModel.Id);
                    if (concreteLink != null)
                    {
                        concreteLink.ImageName = linkModel.ImageName;
                        concreteLink.Url = linkModel.Url;
                        await _imageData.ChangeDBImage(imgModel, context);
                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                }
            }
        }

        /// <summary>
        /// Метод, возаращающий последовательность коллекции объектов, 
        /// реализующих интерфейс ILinkModel с помощью интерфейса 
        /// IEnumberable
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ILinkModel>> GetLinks()
        {
            return await this._context.Links.ToListAsync(); 
        }

        /// <summary>
        /// Асинхронный метод удаления ссылки невозвращаемого
        /// типа, который принимает в себя int значение Id 
        /// удаляемой ссылки. В методе используется
        /// директива using для определения границ текущего
        /// контекста для избежания ошибки ObjectDisposedException.
        /// Происходит создание экземпляра LinkModel, в который
        /// записывается результат перебора таблицы Links
        /// базы данных на предмет совпадения принимаемого id
        /// с id ссылки с помощью метода FirstOrDefaultAsync.
        /// При условии, что экземпляр link не равен нулю
        /// из таблицы Links происходит удаление полученного
        /// экземпляра link с помощью метода Remove с последующим
        /// сохранением базы данных методом SaveChangesAsync.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteLink(int id)
        {
            using (var context = new DataContext(_options))
            {
                LinkModel? link = await context.Links.FirstOrDefaultAsync(x => x.Id == id);
                if (link != null)
                {
                    context.Links.Remove(link);
                    await context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Асинхронный метод поиска ссылки, принимающий int id
        /// ссылки и возвращающий экземпляр ссылки LinkModel. 
        /// Метод представлен лямбда выражением, в котором 
        /// происходит перебор таблицы Links текущей базы данных
        /// на предмет совпадения Id ссылки с принимаемым Id с 
        /// помощью метода FirstOrDefaultAsync. В итоге происходит
        /// возвращение полученного экземпляра LinkModel.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<LinkModel> GetLinkByID(int id) => await _context.Links.FirstOrDefaultAsync(x => x.Id == id);
        #endregion
    }
}
