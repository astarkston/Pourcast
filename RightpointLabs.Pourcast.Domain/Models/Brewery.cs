﻿namespace RightpointLabs.Pourcast.Domain.Models
{
    public class Brewery
    {
        public Brewery(string name)
        {
            Name = name;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Website { get; set; }
        public string Logo { get; set; }
    }
}