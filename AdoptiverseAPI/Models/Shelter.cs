namespace AdoptiverseAPI.Models
{
    public class Shelter
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; } = null;
        public bool FosterProgram { get; set; }
        public int Rank { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
    }
}
