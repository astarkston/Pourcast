﻿namespace RightpointLabs.Pourcast.Web.SignalR
{
    using Microsoft.AspNet.SignalR.Infrastructure;

    using RightpointLabs.Pourcast.Application.Transactions;
    using RightpointLabs.Pourcast.Domain.Events;

    public class PourStartedClientHandler : IEventHandler<PourStarted>
    {
        private readonly IConnectionManager _connectionManager;

        public PourStartedClientHandler(IConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public void Handle(PourStarted domainEvent)
        {
            var context = _connectionManager.GetHubContext<EventsHub>();

            TransactionExtensions.WaitForTransactionCompleted(() => context.Clients.All.PourStarted(domainEvent));
        }
    }
}