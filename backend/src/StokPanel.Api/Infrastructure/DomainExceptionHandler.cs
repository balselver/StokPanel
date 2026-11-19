using Microsoft.AspNetCore.Diagnostics;
using StokPanel.Domain.Common;

namespace StokPanel.Api.Infrastructure
{

    internal sealed class DomainExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not DomainException domainException)
                return false;

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = domainException.Message }, cancellationToken);
            return true;
        }
    }
}
