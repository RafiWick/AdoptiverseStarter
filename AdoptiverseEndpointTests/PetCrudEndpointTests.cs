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
    public class PetCrudEndpointTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public PetCrudEndpointTests(WebApplicationFactory<Program> factory)
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
        public async void GetPets_ReturnsListOfPets()
        {
            var context = GetDbContext();

            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            Pet pet1 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 3, Breed = "Black Lab", Name = "Jet" };
            Pet pet2 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 1, Breed = "Golden Retriever", Name = "Nuggs" };
            Pet pet3 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 4, Breed = "Akbash", Name = "Peter" };
            List<Pet> pets = new() { pet1, pet2, pet3 };

            context.Pets.AddRange(pets);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();


            HttpResponseMessage response = await client.GetAsync($"/api/shelters/{shelter1.Id}/pets");
            string content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(pets);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }
        [Fact]
        public async void GetShelterById_ReturnsShelterWithGivenId()
        {
            var context = GetDbContext();

            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            Pet pet1 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 3, Breed = "Black Lab", Name = "Jet" };
            Pet pet2 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 1, Breed = "Golden Retriever", Name = "Nuggs" };
            Pet pet3 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 4, Breed = "Akbash", Name = "Peter" };
            List<Pet> pets = new() { pet1, pet2, pet3 };

            context.Pets.AddRange(pets);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();


            HttpResponseMessage response = await client.GetAsync($"/api/shelters/{shelter1.Id}/pets/{pet1.Id}");
            string content = await response.Content.ReadAsStringAsync();

            string expected = ObjectToJson(pet1);

            response.EnsureSuccessStatusCode();
            Assert.Equal(expected, content);
        }
        [Fact]
        public async void GetShelterById_Returns404WithInvalidId()
        {
            var context = GetDbContext();

            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            Pet pet1 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 3, Breed = "Black Lab", Name = "Jet" };
            Pet pet2 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 1, Breed = "Golden Retriever", Name = "Nuggs" };
            Pet pet3 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 4, Breed = "Akbash", Name = "Peter" };
            List<Pet> pets = new() { pet1, pet2, pet3 };

            context.Pets.AddRange(pets);
            context.SaveChanges();

            HttpClient client = _factory.CreateClient();


            HttpResponseMessage response = await client.GetAsync($"/api/shelters/{shelter1.Id}/pets/5");
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
            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            context.Shelters.Add(shelter1);
            context.SaveChanges();
            string jsonString = "{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\", \"Adoptable\": true, \"Age\": 2, \"Breed\": \"Bulldog\", \"Name\": \"Paws\"}";//"{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\",\"FosterProgram\":\"true\", \"Rank\":\"3\", \"City\":\"Lakewood\", \"Name\":\"Lakewood Pet Shelter\"}";
            StringContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"/api/shelters/{shelter1.Id}/pets", requestContent);
            Assert.Equal(201, (int)response.StatusCode);
            var newPet = context.Pets.Last();

            Assert.Equal("Paws", newPet.Name);
        }
        [Fact]
        public async void DeleteShelter_DeletesShelterWithGivenId()
        {
            var context = GetDbContext();

            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            Pet pet1 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 3, Breed = "Black Lab", Name = "Jet" };
            Pet pet2 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 1, Breed = "Golden Retriever", Name = "Nuggs" };
            Pet pet3 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 4, Breed = "Akbash", Name = "Peter" };
            List<Pet> pets = new() { pet1, pet2, pet3 };

            context.Pets.AddRange(pets);
            context.SaveChanges();

            var client = _factory.CreateClient();

            var response = await client.DeleteAsync($"/api/shelters/{shelter1.Id}/pets/{pet1.Id}");
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(204, (int)response.StatusCode);
            Assert.DoesNotContain("Jet", content);
        }

        [Fact]
        public async void PutShelter_UpdatesDatabaseRecord()
        {
            var context = GetDbContext();

            Shelter shelter1 = new Shelter { FosterProgram = false, Rank = 3, City = "Denver", Name = "Dumb Friends League" };
            Shelter shelter2 = new Shelter { FosterProgram = false, Rank = 2, City = "Denver", Name = "The Pound" };
            List<Shelter> shelters = new() { shelter1, shelter2 };
            context.Shelters.AddRange(shelters);
            context.SaveChanges();

            Pet pet1 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 3, Breed = "Black Lab", Name = "Jet" };
            Pet pet2 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 1, Breed = "Golden Retriever", Name = "Nuggs" };
            Pet pet3 = new Pet { ShelterId = Convert.ToInt32($"{shelter1.Id}"), Adoptable = true, Age = 4, Breed = "Akbash", Name = "Peter" };
            List<Pet> pets = new() { pet1, pet2, pet3 };

            context.Pets.AddRange(pets);
            context.SaveChanges();

            var client = _factory.CreateClient();
            string jsonString = "{\"CreatedAt\": \"2023-08-29T12:00:00.000Z\", \"UpdatedAt\": \"2023-08-29T12:05:00.000Z\", \"Adoptable\": true, \"Age\": 2, \"Breed\": \"Bulldog\", \"Name\": \"Paws\"}";//"{\"FosterProgram\":\"true\", \"Rank\":\"3\", \"City\":\"Lakewood\", \"Name\":\"Lakewood Pet Shelter\"}";
            StringContent requestContent = new StringContent(jsonString, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"/api/shelters/{shelter1.Id}/pets/{pet1.Id}", requestContent);
            context.ChangeTracker.Clear();
            Assert.Equal(204, (int)response.StatusCode);
            Assert.Equal("Paws", context.Pets.Find(1).Name);

        }
    }
}