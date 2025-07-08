using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MiniTwitter.Core.Application.Events;
using MiniTwitter.Core.Domain.Events;

namespace MiniTwitter.Infrastructure.Events
{
    public class InMemoryEventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync<TEvent>(TEvent domainEvent) where TEvent : IDomainEvent
        {
            var handlers = _serviceProvider.GetServices<IEventHandler<TEvent>>();
            foreach (var handler in handlers)
            {
                await handler.HandleAsync(domainEvent);
            }
        }
    }
}
