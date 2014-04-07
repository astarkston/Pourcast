﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RightpointLabs.Pourcast.Tests
{
    using RightpointLabs.Pourcast.Domain.Models;
    using RightpointLabs.Pourcast.Infrastructure.Data;

    [TestClass]
    public class MongoConnectionHandlerTest
    {
        [TestMethod]
        public void AbleToRetrieveItemsFromTestDatabase()
        {
            // Setup
            var mch = new MongoConnectionHandler<Keg>("mongodb://localhost", "test");

            // Act
            var kegs = mch.MongoCollection.FindAllAs<Keg>();
            

            // Assert
            Assert.AreNotEqual(0, kegs.Count());
        }
    }
}
