using System.ComponentModel.DataAnnotations;

namespace AzureBlob.Models
{
    public class Container
    {
        [Required]
        public string Name { get; set; }
    }
}
