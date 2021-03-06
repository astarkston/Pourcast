﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RightpointLabs.Pourcast.Web.Areas.Admin.Models
{
    public class BeerViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double ABV { get; set; }
        [DisplayName("Beer Advocate Score")]
        public double BAScore { get; set; }
        public string Style { get; set; }
        public string Color { get; set; }
        public string Glass { get; set; }
        public string BreweryId { get; set; }
        [DisplayName("Brewery Name")]
        public string BreweryName { get; set; }

    }
}