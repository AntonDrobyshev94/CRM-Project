using FinalProject_API.ContextFolder;
using FinalProject_API.Models;
using FinalProject_API.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по
    /// взаимодействию с услугами через БД
    /// </summary>
    public class ServiceData
    {
        private DbContextOptions<DataContext> _options;
        private readonly DataContext _context;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ServiceData"/> с инжекцией зависимостей для <see cref="DataContext"/>.
        /// </summary>
        /// <param name="context">Экземпляр <see cref="DataContext"/>, используемый для взаимодействия с базой данных.</param>
        /// <param name="options">Экземпляр <see cref="DbContextOptions{DataContext}"/>, 
        /// используемый для для взаимодействия с базой данных</param>
        public ServiceData(DbContextOptions<DataContext> options,
            DataContext context)
        {
            _options = options;
            _context = context;
        }

        #region Services
        /// <summary>
        /// Метод добавления услуги, посредством
        /// обращения к методу Add datacontext Service.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        /// <param name="service"></param>
        public async Task AddService(Service service)
        {
            using (var context = new DataContext(_options))
            {
                context.Services.Add(service);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Метод изменения услуги, принимающий модель услуги
        /// Service. В методе используется директива using 
        /// для определения границ текущего контекста для избежания 
        /// ошибки ObjectDisposedException. Внутри директивы создаётся
        /// экземпляр Service, в которой асинхронно, с помощью
        /// метода FirstOrDefaultAsync, происходит перебор таблицы Services
        /// на предмет совпадения id принимаемой модели и id хранящейся в 
        /// базе данных услуги. Результат перебора сохраняется в ранее
        /// созданный экземпляр Service. Параметры полученного экземпляра 
        /// перезаписываются на параметры принимаемой методом модели.
        /// По окончанию производится сохранение методом SaveChanges.
        /// </summary>
        /// <param name="service"></param>
        public async Task ChangeService(Service service)
        {
            using (var context = new DataContext(_options))
            {
                Service? concreteService = await context.Services.FirstOrDefaultAsync(x => x.Id == service.Id);
                if (concreteService != null)
                {
                    concreteService.Description = service.Description;
                    concreteService.Name = service.Name;
                    await context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Ошибка изменения сервиса");
                }
            }
        }

        /// <summary>
        /// Метод, возаращающий последовательность коллекции объектов, 
        /// реализующих интерфейс IService с помощью интерфейса 
        /// IEnumberable
        /// </summary>
        /// <returns></returns>
        public async Task <IEnumerable<IService>> GetServices()
        {
            return await this._context.Services.ToListAsync();
        }

        /// <summary>
        /// Асинхронный метод удаления услуги невозвращаемого
        /// типа, который принимает в себя int значение Id 
        /// удаляемой услуги. В методе используется
        /// директива using для определения границ текущего
        /// контекста для избежания ошибки ObjectDisposedException.
        /// Происходит создание экземпляра Service, в который
        /// записывается результат перебора таблицы Services
        /// базы данных на предмет совпадения принимаемого id
        /// с id услуги с помощью метода FirstOrDefaultAsync.
        /// При условии, что экземпляр Service не равен нулю
        /// из таблицы Services происходит удаление полученного
        /// экземпляра с помощью метода Remove с последующим
        /// сохранением базы данных методом SaveChangesAsync.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteService(int id)
        {
            using (var context = new DataContext(_options))
            {
                Service? service = await context.Services.FirstOrDefaultAsync(x => x.Id == id);
                if (service != null)
                {
                    context.Services.Remove(service);
                    await context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Асинхронный метод поиска заявки, принимающий int id
        /// заявки и возвращающий экземпляр заявки Service. 
        /// Метод представлен лямбда выражением, в котором 
        /// происходит перебор таблицы Services текущей базы данных
        /// на предмет совпадения Id заявки с принимаемым Id с 
        /// помощью метода FirstOrDefaultAsync. В итоге происходит
        /// возвращение полученного экземпляра Service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Service> GetServiceByID(int id) => await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
        #endregion
    }
}
