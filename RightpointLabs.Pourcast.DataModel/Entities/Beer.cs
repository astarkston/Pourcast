﻿using MongoDB.Bson.Serialization.Attributes;

namespace RightpointLabs.Pourcast.DataModel.Entities
{
    public class Beer
    {
        public string Name { get; set; }
        [BsonElement("Brewery")]
        public Brewery Brewer { get; set; }
        public double ABV { get; set; }
        public int BAScore { get; set; }
        public int RPScore { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string Glass { get; set; }
    }
}