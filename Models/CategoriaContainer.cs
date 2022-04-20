using DesafioAPI.HATEOAS;

namespace DesafioAPI.Models
{
    public class CategoriaContainer
    {
        public Categoria categoria { get; set; }
        public Link[] links { get; set; }
    }
}