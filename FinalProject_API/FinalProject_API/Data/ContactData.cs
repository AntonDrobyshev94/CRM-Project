using FinalProject_API.ContextFolder;
using FinalProject_API.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по
    /// взаимодействию с контактами через БД
    /// </summary>
    public class ContactData
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
        public ContactData(DbContextOptions<DataContext> options, 
            DataContext context)
        {
            _options = options;
            _context = context;
        }
        #region Contacts
        /// <summary>
        /// Метод, возаращающий экземпляр Contacts
        /// </summary>
        /// <returns></returns>
        public async Task<Contacts> GetContacts() => await _context.Contacts.FirstOrDefaultAsync();


        /// <summary>
        /// Метод изменения контактных данных, принимающий модель 
        /// Contacts. В методе Создаётся экземпляр Contact. Далее,
        /// в директиве using происходит перебор таблицы Contacts
        /// базы данных в цикле foreach, в котором происходит 
        /// кеширование полученного в цикле контакта в ранее
        /// созданный экземпляр Contacts (условия в цикле 
        /// отсутствуют, так как в таблице содержится только один
        /// элемент, а создание других не предусмотрено логикой
        /// программы). Параметры полученного экземпляра перезаписываются 
        /// на параметры принимаемой методом модели.
        /// По окончанию производится сохранение методом SaveChanges.
        /// </summary>
        /// <param name="contacts"></param>
        public async Task<bool> ChangeContacts(Contacts contacts)
        {
            try
            {
                Contacts? concreteContacts = new Contacts();
                using (var context = new DataContext(_options))
                {
                    concreteContacts = await context.Contacts.FirstOrDefaultAsync();
                    if (concreteContacts != null)
                    {
                        concreteContacts.Address = contacts.Address;
                        concreteContacts.Telephone = contacts.Telephone;
                        concreteContacts.Fax = contacts.Fax;
                        concreteContacts.Email = contacts.Email;
                        await context.SaveChangesAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось сохранить контакты {ex.Message}");
                return false;
            }
        }
        #endregion
    }
}
