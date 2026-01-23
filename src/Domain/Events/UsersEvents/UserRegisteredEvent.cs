namespace Domain.Events.UsersEvents;
public class UserRegisteredEvent: BaseEvent
{
    public required Guid UserId { get; set; }
    public required string Email { get; set; }
    public string UserName { get; set; } = string.Empty;
}
