namespace DesafioAPI.Models
{
    public class Starter
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Letra { get; set; }
        public string Email { get; set; }
        public string Foto { get; set; }
        public Categoria Categoria { get; set; }
    }
}