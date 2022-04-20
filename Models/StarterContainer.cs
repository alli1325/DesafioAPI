using DesafioAPI.HATEOAS;

namespace DesafioAPI.Models
{
    public class StarterContainer
    {
        public Starter starter { get; set; }
        public Link[] links { get; set; }
    }
}