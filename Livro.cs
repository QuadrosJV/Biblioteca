public class Livro
{
    public int Id { get; set; }
    public string? Titulo { get; set; }
    public string? Autor { get; set; }
    public string? ISBN { get; set; }
    public int AnoPublicacao { get; set; }
    public string? Genero { get; set; }
    public bool Disponivel { get; set; }
    public DateTime DataCadastro { get; set; }
}