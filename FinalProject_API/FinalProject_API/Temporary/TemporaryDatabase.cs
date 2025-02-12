using System.Runtime.Caching;

namespace FinalProject_API.Temporary
{
    public class TemporaryDatabase
    {
        private readonly MemoryCache _cache;
        private readonly CacheItemPolicy _cacheItemPolicy;

        public TemporaryDatabase()
        {
            _cache = MemoryCache.Default;
            _cacheItemPolicy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(3)
            };
        }

        /// <summary>
        /// Метод добавления ключа идемпотентности в кэш
        /// </summary>
        /// <param name="requestId"></param>
        public void AddOrUpdate(string requestId, object data)
        {
            if (_cache.Contains(requestId))
            {
                Console.WriteLine($"Ключ {requestId} уже существует в кэше.");
                _cache.Set(requestId, data, _cacheItemPolicy);
                Console.WriteLine($"Значение ключа {requestId} обновлено.");
            }
            else
            {
                _cache.Add(requestId, data, _cacheItemPolicy);
                Console.WriteLine($"Ключ {requestId} добавлен в кэш.");
            }
        }
        
        /// <summary>
        /// Получение объекта (ключа) по совпадению Id
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns></returns>
        public object Get(string requestId)
        {
            return _cache.Get(requestId);
        }

        public bool Remove(string requestId)
        {
            return _cache.Remove(requestId) != null;
        }
    }
}
