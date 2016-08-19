using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Hubs;

namespace BugTracker.Hubs
{
    [HubName("bugs")]
    public class BugHub : Hub
    {
    }
}