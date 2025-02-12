using FinalProject_API.ContextFolder;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по взаимодействию 
    /// с атрибутами меню навигации через БД
    /// </summary>
    public class MenuNavigationAndTagData
    {
        private DbContextOptions<DataContext> _options;
        private readonly DataContext _context;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ServiceData"/> с инжекцией 
        /// зависимостей для <see cref="DataContext"/>.
        /// </summary>
        /// <param name="context">Экземпляр <see cref="DataContext"/>, используемый 
        /// для взаимодействия с базой данных.</param>
        /// <param name="options">Экземпляр <see cref="DbContextOptions{DataContext}"/>, 
        /// используемый для для взаимодействия с базой данных</param>
        public MenuNavigationAndTagData(DbContextOptions<DataContext> options, 
            DataContext context)
        {
            _options = options;
            _context = context;
        }

        #region Title
        /// <summary>
        /// Метод, возаращающий экземпляр TitleModel. Происходит
        /// создание нового экземпляра TitleModel. Далее
        /// происходит перебор таблицы Title в цикле foreach 
        /// в котором происходит кеширование полученной страницы 
        /// в ранее созданный экземпляр TitleModel (условия в цикле 
        /// отсутствуют, так как в таблице содержится только один
        /// элемент, а создание других не предусмотрено логикой
        /// программы). По окончанию происходит возврат модели
        /// ключевым словом return.
        /// </summary>
        /// <returns></returns>
        public async Task<TitleModel> GetTitle() => await _context.Title.FirstOrDefaultAsync();

        /// <summary>
        /// Метод изменения титульных данных, принимающий модель 
        /// TitleModel. В методе Создаётся экземпляр TitleModel. Далее,
        /// в директиве using происходит перебор таблицы Title
        /// базы данных в цикле foreach, в котором происходит 
        /// кеширование полученного в цикле титульного листа в ранее
        /// созданный экземпляр TitleModel (условия в цикле 
        /// отсутствуют, так как в таблице содержится только один
        /// элемент, а создание других не предусмотрено логикой
        /// программы). Параметры полученного экземпляра перезаписываются 
        /// на параметры принимаемой методом модели.
        /// По окончанию производится сохранение методом SaveChanges.
        /// </summary>
        /// <param name="title"></param>
        public async Task ChangeTitle(TitleModel title)
        {
            TitleModel concreteTitle = new TitleModel();
            using (var context = new DataContext(_options))
            {
                foreach (var titleModel in context.Title)
                {
                    concreteTitle = titleModel;
                }
                concreteTitle.Title = title.Title;
                concreteTitle.MainTitle = title.MainTitle;
                concreteTitle.BlogTitle = title.BlogTitle;
                concreteTitle.ServicesTitle = title.ServicesTitle;
                concreteTitle.ContactsTitle = title.ContactsTitle;
                concreteTitle.ProjectsTitle = title.ProjectsTitle;
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region Tags

        /// <summary>
        /// Метод, возаращающий последовательность коллекции объектов, 
        /// реализующих интерфейс ITagModel с помощью интерфейса 
        /// IEnumberable
        /// </summary>
        /// <returns></returns>
        public async Task<ITagModel> GetRandomTag()
        {
            using (var context = new DataContext(_options))
            {
                var tags = await context.Tags.ToListAsync();
                Random random = new Random();
                if (tags.Count > 0)
                {
                    TagModel randomTag = tags.OrderBy(x => random.Next()).FirstOrDefault();
                    return randomTag;
                }
                else
                {
                    return new TagModel();
                }
            }
        }

        /// <summary>
        /// Метод, возаращающий последовательность коллекции объектов, 
        /// реализующих интерфейс ITagModel с помощью интерфейса 
        /// IEnumberable
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ITagModel>> GetTags()
        {
            return await this._context.Tags.ToListAsync();
        }

        /// <summary>
        /// Метод добавления тэга, посредством
        /// обращения к методу Add datacontext TagModel.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        /// <param name="tag"></param>
        public async Task AddTag(TagModel tag)
        {
            using (var context = new DataContext(_options))
            {
                context.Tags.Add(tag);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Асинхронный метод удаления тэга невозвращаемого
        /// типа, который принимает в себя int значение Id 
        /// удаляемого тэга. В методе используется
        /// директива using для определения границ текущего
        /// контекста для избежания ошибки ObjectDisposedException.
        /// Происходит создание экземпляра TagModel, в которую
        /// записывается результат перебора таблицы Tags
        /// базы данных на предмет совпадения принимаемого id
        /// с id тэга с помощью метода FirstOrDefaultAsync.
        /// При условии, что экземпляр tag не равен нулю
        /// из таблицы Tags происходит удаление полученного
        /// экземпляра тэга с помощью метода Remove с последующим
        /// сохранением базы данных методом SaveChangesAsync.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteTag(int id)
        {
            using (var context = new DataContext(_options))
            {
                TagModel? tag = await context.Tags.FirstOrDefaultAsync(x => x.Id == id);
                if (tag != null)
                {
                    context.Tags.Remove(tag);
                    await context.SaveChangesAsync();
                }
            }
        }
        #endregion
    }
}
