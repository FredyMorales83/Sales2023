﻿using Sales.Shared.Entities;

namespace Sales.API.Data
{
    public class SeedDB
    {
        private readonly DataContext _context;

        public SeedDB(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "México",
                    States = new List<State>() {
                        new State (){
                            Name = "Chiapas",
                            Cities = new List<City>() {
                                new City { Name = "Tuxtla Gutierrez" },
                                new City { Name = "Chiapa de Corzo" },
                                new City { Name = "Tapachula" },
                            }
                        }
                    }
                });
                _context.Countries.Add(new Country { Name = "Colombia" });
                _context.Countries.Add(new Country { Name = "Guatemala" });
                await _context.SaveChangesAsync();
            }
        }
    }
}