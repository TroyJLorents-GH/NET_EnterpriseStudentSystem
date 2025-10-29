using Microsoft.AspNetCore.SignalR;

namespace StudentSystem.API.Hubs;

/// <summary>
/// SignalR Hub for real-time student system updates
/// Supports real-time notifications for assignment changes, grade updates, etc.
/// </summary>
public class StudentHub : Hub
{
    public async Task SendAssignmentUpdate(string message)
    {
        await Clients.All.SendAsync("ReceiveAssignmentUpdate", message);
    }

    public async Task SendStudentUpdate(int studentId, string updateType)
    {
        await Clients.All.SendAsync("ReceiveStudentUpdate", studentId, updateType);
    }

    public async Task NotifyClassUpdate(string classNum, string message)
    {
        await Clients.Group($"Class_{classNum}").SendAsync("ClassUpdate", message);
    }

    public async Task JoinClassGroup(string classNum)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Class_{classNum}");
    }

    public async Task LeaveClassGroup(string classNum)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Class_{classNum}");
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
