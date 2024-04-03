using System.ComponentModel.DataAnnotations;

namespace TagFetcherDomain.models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Count { get; set; }
        public double Share { get; set; }
    }
}
