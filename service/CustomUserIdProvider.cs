using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;


namespace service;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Extract the user's ID from the JWT claim or other authentication token
        return connection.User?.FindFirst(ClaimTypes.Sid)?.Value;
    }
}
