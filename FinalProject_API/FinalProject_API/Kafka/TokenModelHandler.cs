using Azure;
using Azure.Core;
using FinalProject_API.AuthFinalProjectApp;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace FinalProject_API.Kafka
{
    public class TokenModelHandler
    {
        private readonly ConcurrentDictionary<string, TaskCompletionSource<TokenResponseModel>> _pendingResponses;

        public TokenModelHandler(ConcurrentDictionary<string, TaskCompletionSource<TokenResponseModel>> pendingResponses)
        {
            _pendingResponses = pendingResponses;
        }
        public void HandleMessage(string key, byte[] message)
        {
            try
            {
                var tokenModel = JsonSerializer.Deserialize<TokenResponseModel>(Encoding.UTF8.GetString(message));
                if (tokenModel is null)
                {
                    CompleteResponse(key, null);
                }
                else
                {
                    Console.WriteLine($"Id {key} готов к передаче в контроллер");
                    Console.WriteLine($"Проверка состояния Id {key} до удаления. Колличество элементов в Concurrent коллекции: {_pendingResponses.Count}");
                    CompleteResponse(key, tokenModel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void CompleteResponse(string requestId, TokenResponseModel response)
        {
            if (_pendingResponses.TryRemove(requestId, out var tcs))
            {
                Console.WriteLine($"Успешное удаление Id {requestId}.");
                tcs.SetResult(response);
            }
            else
            {
                Console.WriteLine($"RequestId {requestId} не найден. Текущий размер коллекции: {_pendingResponses.Count}");
            }
        }
    }
}
