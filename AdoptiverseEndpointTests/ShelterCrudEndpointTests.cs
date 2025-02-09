using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using AdoptiverseAPI.DataAccess;
using System.Net;
using System.Text;
using AdoptiverseAPI.Models;

namespace AdoptiverseEndpointTests
{
    [Collection("Controller Tests")]
    public class ShelterCrudEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ShelterCrudEndpointTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        private AdoptiverseApiContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AdoptiverseApiContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new AdoptiverseApiContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
        private string ObjectToJson(object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver
            });

            return json;
        }

        [Fact]
        public async void GetShelters_ReturnsListOfShelters()
        {
            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };

            var context = GetDbContext();
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();


            HttpResponseMessage response = await client.GetAsync("/api/shelters");
            string content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(shelters);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }
        [Fact]
        public async void GetShelterById_ReturnsShelterWithGivenId()
        {
            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };

            var context = GetDbContext();
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();


            HttpResponseMessage response = await client.GetAsync($"/api/shelters/{shelter1.Id}");
            string content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(shelter1);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }
        [Fact]
        public async void GetShelterById_Returns404WithInvalidId()
        {
            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };

            var context = GetDbContext();
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();


            HttpResponseMessage response = await client.GetAsync($"/api/shelters/4");
            string content = await response.Content.ReadAsStringAsync();

            var expected = HttpStatusCode.NotFound;

            //response.EnsureSuccessStatusCode();
            Assert.Equal(expected, response.StatusCode);
        }
        [Fact]
        public async void PostShelter_CreatesShelterInDb()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            string jsonString = "{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\", \"FosterProgram\": true, \"Rank\": 1, \"City\": \"San Francisco\", \"Name\": \"Happy Paws\"}";//"{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\",\"FosterProgram\":\"true\", \"Rank\":\"3\", \"City\":\"Lakewood\", \"Name\":\"Lakewood Pet Shelter\"}";
            StringContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/api/shelters", requestContent);
            Assert.Equal(201, (int)response.StatusCode);
            var newShelter = context.Shelters.Last();

            Assert.Equal("Happy Paws", newShelter.Name);
        }
        [Fact]
        public async void DeleteShelter_DeletesShelterWithGivenId()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            var response = await client.DeleteAsync("/api/shelters/1");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(204, (int)response.StatusCode);
            Assert.DoesNotContain("Lakewood Pet Shelter", content);
        }

        [Fact]
        public async void PutShelter_UpdatesDatabaseRecord()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };
            context.Shelters.AddRange(shelters);
            context.SaveChanges();
            string jsonString = "{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\", \"FosterProgram\": true, \"Rank\": 1, \"City\": \"San Francisco\", \"Name\": \"Happy Paws\"}";//"{\"FosterProgram\":\"true\", \"Rank\":\"3\", \"City\":\"Lakewood\", \"Name\":\"Lakewood Pet Shelter\"}";
            StringContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync("/api/shelters/1", requestContent);
            context.ChangeTracker.Clear();
            Assert.Equal(204, (int)response.StatusCode);
            Assert.Equal("Happy Paws", context.Shelters.Find(1).Name);

        }
    }
}