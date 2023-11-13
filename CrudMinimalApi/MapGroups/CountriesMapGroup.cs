using Entities;
using Microsoft.EntityFrameworkCore;

namespace CrudMinimalApi.MapGroups
{
    public static class CountriesMapGroup
    {
        public static RouteGroupBuilder CountriesApi(this RouteGroupBuilder Group)
        {
            // GET: api/Countries
            Group.MapGet("/", async (CrudDbContext dbContext) =>
                await dbContext.Countries.ToListAsync());

            // GET: api/Countries/5
            Group.MapGet("/{id}", async (Guid id, CrudDbContext dbcontext) =>
                await dbcontext.Countries.FindAsync(id)
                    is Country country
                        ? Results.Ok(country)
                        : Results.NotFound());

            // PUT: api/Countries/5
            Group.MapPut("/{id}", async (Guid id, Country country, CrudDbContext dbcontext) =>
            {
                var countryToUpdate = await dbcontext.Countries.FindAsync(id);
                if (countryToUpdate is null) return Results.NotFound();
                countryToUpdate.Name = country.Name;
                await dbcontext.SaveChangesAsync();
                return Results.NoContent();
            });

            // POST: api/Countries
            Group.MapPost("/", async (Country country, CrudDbContext dbcontext) =>
            {
                dbcontext.Countries.Add(country);
                await dbcontext.SaveChangesAsync();
                return Results.Created($"/api/countries/{country.Id}", country);
            });

            // DELETE: api/Countries/5
            Group.MapDelete("/{id}", async (Guid id, CrudDbContext dbcontext) =>
            {
                if (await dbcontext.Countries.FindAsync(id) is Country country)
                {
                    dbcontext.Countries.Remove(country);
                    await dbcontext.SaveChangesAsync();
                    return Results.Ok(country);
                }
                return Results.NotFound();
            });

            return Group;
        }
    }
}