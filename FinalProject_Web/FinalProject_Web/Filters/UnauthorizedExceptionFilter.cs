using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Filters
{
    public class UnauthorizedExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is UnauthorizedAccessException)
            {
                context.Result = new RedirectResult($"/Account/Logout?returnUrl={Uri.EscapeDataString(context.HttpContext.Request.Path)}");
                context.ExceptionHandled = true;
            }
        }
    }
}
