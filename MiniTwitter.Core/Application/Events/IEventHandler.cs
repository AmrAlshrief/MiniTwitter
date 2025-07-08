using System.Threading.Tasks;
using MiniTwitter.Core.Domain.Events;

namespace MiniTwitter.Core.Application.Events
{
    public interface IEventHandler<TEvent> where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent);
    }
}
