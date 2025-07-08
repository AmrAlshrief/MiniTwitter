using System.Threading.Tasks;
using MiniTwitter.Core.Domain.Events;

namespace MiniTwitter.Core.Application.Events
{
    public interface IEventDispatcher
    {
        Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent;
    }
}
