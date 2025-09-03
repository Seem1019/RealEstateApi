﻿namespace RealEstate.Application.DTOs
{
    public class UpdatePropertyDto
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public decimal Price { get; set; }
        public string? CodeInternal { get; set; }
        public int Year { get; set; }
    }
}