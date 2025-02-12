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
    /// взаимодействию с заявками через БД
    /// </summary>
    public class ApplicationData
    {
        private readonly DataContext _context;
        private DbContextOptions<DataContext> _options;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ServiceData"/> с инжекцией 
        /// зависимостей для <see cref="DataContext"/>.
        /// </summary>
        /// <param name="context">Экземпляр <see cref="DataContext"/>, используемый 
        /// для взаимодействия с базой данных.</param>
        /// <param name="options">Экземпляр <see cref="DbContextOptions{DataContext}"/>, 
        /// используемый для для взаимодействия с базой данных</param>
        public ApplicationData(DataContext context, 
            DbContextOptions<DataContext> options)
        {
            _context = context;
            _options = options;
        }

        #region Application
        /// <summary>
        /// Метод добавления нового запроса (заявки), посредством
        /// обращения к методу Add datacontext ApplicationData.
        /// По окончанию производится сохранение методом
        /// SaveChanges.
        /// </summary>
        public async Task<IActionResult> AddApplications(Application application)
        {
            try
            {
                using (var context = new DataContext(_options))
                {
                    context.Requests.Add(application);
                    await context.SaveChangesAsync();
                    return new StatusCodeResult(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new StatusCodeResult(500);
            }
        }

        /// <summary>
        /// Метод, возаращающий последовательность коллекции объектов, 
        /// реализующих интерфейс IApplication с помощью интерфейса 
        /// IEnumberable
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IApplication>> GetApplications()
        {
            return await this._context.Requests.ToListAsync();
        }

        /// <summary>
        /// Асинхронный метод удаления заявки невозвращаемого
        /// типа, который принимает в себя int значение Id 
        /// удаляемой заявки. В методе используется
        /// директива using для определения границ текущего
        /// контекста для избежания ошибки ObjectDisposedException.
        /// Происходит создание экземпляра Application, в которую
        /// записывается результат перебора таблицы Requests
        /// базы данных на предмет совпадения принимаемого id
        /// с id заявки с помощью метода FirstOrDefaultAsync.
        /// При условии, что экземпляр Application не равен нулю
        /// из таблицы Requests происходит удаление полученного
        /// экземпляра с помощью метода Remove с последующим
        /// сохранением базы данных методом SaveChangesAsync.
        /// </summary>
        /// <param name="id"></param>
        public async Task DeleteApplication(int id)
        {
            using (var context = new DataContext(_options))
            {
                Application? application = await context.Requests.FirstOrDefaultAsync(x => x.Id == id);
                if (application != null)
                {
                    context.Requests.Remove(application);
                    await context.SaveChangesAsync();
                }
            }
        }

        /// <summary>
        /// Асинхронный метод поиска заявки, принимающий int id
        /// заявки и возвращающий экземпляр заявки Application. 
        /// Метод представлен лямбда выражением, в котором 
        /// происходит перебор таблицы Requests текущей базы данных
        /// на предмет совпадения Id заявки с принимаемым Id с 
        /// помощью метода FirstOrDefaultAsync. В итоге происходит
        /// возвращение полученного экземпляра Appliacation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Application> GetApplicationByID(int id) => await _context.Requests.FirstOrDefaultAsync(x => x.Id == id);

        /// <summary>
        /// Асинхронный метод изменения заявки невозвращаемого типа,
        /// который принимает в экземпляр заявки.
        /// В методе используется директива using для определения границ 
        /// текущего контекста для избежания ошибки ObjectDisposedException.
        /// Создается экземпляр Application, в который записывается результат
        /// перебора таблицы Requests текущей базы данных на предмет
        /// совпадения Id заявки с помощью метода FirstOrDefaultAsync.
        /// Далее происходит изменение параметров полученной заявки
        /// параметрами, которые были получены в принимаемом экземпляре
        /// Application. Результат изменений сохраняется с помощью метода
        /// SaveChangesAsync.
        /// </summary>
        /// <param name="application"></param>
        public async Task ChangeStatus(Application application)
        {
            using (var context = new DataContext(_options))
            {
                Application? concreteApplication = await context.Requests.FirstOrDefaultAsync(x => x.Id == application.Id);
                if (concreteApplication != null)
                {
                    concreteApplication.Status = application.Status;
                    await context.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("Ошибка изменения статуса");
                }
            }
        }
        #endregion
    }
}
