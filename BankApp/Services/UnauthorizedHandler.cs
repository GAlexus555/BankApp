using System.Net;
using System.Net.Http;

namespace BankApp.Services;

public class UnauthorizedHandler : DelegatingHandler
{
    public static event Action? Unauthorized;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            Unauthorized?.Invoke();
        return response;
    }
}
