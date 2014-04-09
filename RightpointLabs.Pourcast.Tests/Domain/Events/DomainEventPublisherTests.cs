﻿namespace RightpointLabs.Pourcast.Tests.Domain.Events
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Ploeh.AutoFixture;

    using RightpointLabs.Pourcast.Domain.Events;

    [TestClass]
    public class DomainEventPublisherTests
    {
        [TestInitialize]
        public void Initialize()
        {
        }

        [TestClass]
        public class PublishMethod
        {
            [TestMethod]
            public void FiresOfSingleHandler()
            {
                var mockEvent = new Mock<IDomainEvent>();
                var mockHandler = new Mock<IDomainEventHandler<IDomainEvent>>();

                DomainEventPublisher.Subscribe(mockHandler.Object);
                DomainEventPublisher.Publish(mockEvent.Object);

                mockHandler.Verify(x => x.HandleEvent(mockEvent.Object));
            }

            [TestMethod]
            public void FiresOfMultipleHandler()
            {
                var mockEvent = new Mock<IDomainEvent>();
                var mockHandler1 = new Mock<IDomainEventHandler<IDomainEvent>>();
                var mockHandler2 = new Mock<IDomainEventHandler<IDomainEvent>>();

                DomainEventPublisher.Subscribe(mockHandler1.Object);
                DomainEventPublisher.Subscribe(mockHandler2.Object);
                DomainEventPublisher.Publish(mockEvent.Object);

                mockHandler1.Verify(x => x.HandleEvent(mockEvent.Object));
                mockHandler2.Verify(x => x.HandleEvent(mockEvent.Object));
            }

            [TestMethod]
            public void SpecificEventsReachSpecificHandlers()
            {
                var fixture = new Fixture();
                var kegEmptiedEvent = fixture.Create<KegEmptied>();
                var kegNearingEmptyEvent = fixture.Create<KegNearingEmpty>();
                var mockHandler1 = new Mock<IDomainEventHandler<KegEmptied>>();
                var mockHandler2 = new Mock<IDomainEventHandler<KegNearingEmpty>>();

                DomainEventPublisher.Subscribe(mockHandler1.Object);
                DomainEventPublisher.Subscribe(mockHandler2.Object);
                DomainEventPublisher.Publish(kegEmptiedEvent);
                DomainEventPublisher.Publish(kegNearingEmptyEvent);

                mockHandler1.Verify(x => x.HandleEvent(kegEmptiedEvent));
                mockHandler2.Verify(x => x.HandleEvent(kegNearingEmptyEvent));
            }
        }
    }
}
