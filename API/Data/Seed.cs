using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync())
                return;

                //get the jaon from file
                var jsonData = System.IO.File.ReadAllText("Data/userSeedData.json");
                var users = JsonSerializer.Deserialize<List<AppUser>>(jsonData);

                //populate the users with default passwords
                using var hmac = new HMACSHA512();
                foreach (var user in users)
                {
                    user.PasswordHash = await hmac.ComputeHashAsync(new MemoryStream(Encoding.UTF8.GetBytes("Pa$$w0rd")));
                    user.PasswordSalt = hmac.Key;
                }
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
        }
    }
}