﻿namespace School_TV_Show.DTO
{
    public class CreatePackageRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; } 
    }
}
