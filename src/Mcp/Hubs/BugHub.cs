using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace McpSmyrilLine.Hubs
{
    [HubName("bugs")]
    public class BugHub : Hub
    {
    }
}