using Microsoft.AspNetCore.SignalR;

namespace ManageTask.Infrastructure.SignalR
{
    public class TaskHub: Hub
    {
        public async Task JoinTaskGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
