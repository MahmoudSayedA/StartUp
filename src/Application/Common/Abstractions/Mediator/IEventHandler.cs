using Domain.Common;

namespace Application.Common.Abstractions.Mediator;
public interface IEventHandler<T> : INotificationHandler<T>
    where T : BaseEvent
{

}
