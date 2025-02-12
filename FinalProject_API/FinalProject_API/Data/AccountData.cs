using FinalProject_API.AuthFinalProjectApp;
using System.Collections.Concurrent;
using System.Text.Json;

namespace FinalProject_API.Data
{
    /// <summary>
    /// Репозиторий-бизнес логика приложения, 
    /// осуществляющая операции по взаимодействию
    /// микросервисом авторизации
    /// </summary>
    public class AccountData
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<TokenResponseModel>> _pendingResponses;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AccountData"/> с инжекцией зависимостей для 
        /// <see cref="ConcurrentDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="pendingResponses">Экземпляр <see cref="ConcurrentDictionary{TKey, TValue}"/>, 
        /// используемый для хранения задач, ожидающих завершения, при взаимодействии с Kafka.</param>
        public AccountData(ConcurrentDictionary<string, TaskCompletionSource<TokenResponseModel>> pendingResponses)
        {
            _pendingResponses = pendingResponses;
        }

        /// <summary>
        /// Метод ожидания ответа, использующий Concurrent словарь
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        internal Task<TokenResponseModel> WaitForResponse(string requestId)
        {
            var tcs = new TaskCompletionSource<TokenResponseModel>();
            if (!_pendingResponses.TryAdd(requestId, tcs))
            {
                throw new InvalidOperationException($"Задача с requestId {requestId} уже существует.");
            }
            Console.WriteLine($"Добавлен Id {requestId} в Concurrent коллекцию. Текущий размер Concurrent коллекции: {_pendingResponses.Count}");
            return tcs.Task;
        }

        /// <summary>
        /// Метод создания CommonModel
        /// </summary>
        /// <param name="nameOfType"></param>
        /// <param name="modelObject"></param>
        /// <returns></returns>
        internal CommonModel CommonModelCreate(string nameOfType, object modelObject)
        {
            return new CommonModel()
            {
                Type = nameOfType,
                BaseModel = JsonSerializer.SerializeToElement(modelObject)
            };
        }
    }
}
