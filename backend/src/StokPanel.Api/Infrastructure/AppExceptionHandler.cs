using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using StokPanel.Application.Common;
using StokPanel.Domain.Common;

namespace StokPanel.Api.Infrastructure
{
    internal sealed class AppExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            (int Status, string? Message) result = exception switch
            {
                DomainException e => (StatusCodes.Status400BadRequest, e.Message),
                NotFoundException e => (StatusCodes.Status404NotFound, e.Message),
                DbUpdateConcurrencyException => (StatusCodes.Status409Conflict,
                    "Kayıt siz işlem yaparken değişti. Sayfayı yenileyip tekrar deneyin"),
                _ => (0, null)
            };

            if (result.Message is null)
                return false;   // Beklenmeyen hata → ProblemDetails (500)

            context.Response.StatusCode = result.Status;
            await context.Response.WriteAsJsonAsync(new { error = result.Message }, cancellationToken);
            return true;
        }
    }
}