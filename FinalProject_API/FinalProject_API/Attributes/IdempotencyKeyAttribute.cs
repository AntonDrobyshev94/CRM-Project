using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinalProject_API.Attributes
{
    /// <summary>
    /// Атрибут проверки ключа идемпотентности
    /// </summary>
    public class IdempotencyKeyAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Осуществляет проверку заголовка на наличие ключа идемпотентности и
        /// сохраняет его в Items для дальнейшего использования
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var idempotencyKey = context.HttpContext.Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (string.IsNullOrEmpty(idempotencyKey))
            {
                context.Result = new BadRequestObjectResult("Idempotency-Key заголовок отсутствует");
                return;
            }
            context.HttpContext.Items["IdempotencyKey"] = idempotencyKey;
            base.OnActionExecuting(context);
        }
    }
}
