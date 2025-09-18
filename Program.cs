using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Lista de livros em memória
var livros = new List<Livro> { new Livro { Id = 1, Titulo = "1984", Autor = "George Orwell", ISBN = "978-0451524935", AnoPublicacao = 1949, Genero = "Distopia", Disponivel = true, DataCadastro = DateTime.Now.AddMonths(-6) }, new Livro { Id = 2, Titulo = "Dom Casmurro", Autor = "Machado de Assis", ISBN = "978-8525406958", AnoPublicacao = 1899, Genero = "Romance", Disponivel = false, DataCadastro = DateTime.Now.AddMonths(-5) }, new Livro { Id = 3, Titulo = "O Senhor dos Anéis", Autor = "J.R.R. Tolkien",  ISBN = "978-0544003415", AnoPublicacao = 1954, Genero = "Fantasia", Disponivel = true, DataCadastro = DateTime.Now.AddMonths(-4) }, new Livro { Id = 4, Titulo = "Cem Anos de Solidão", Autor = "Gabriel García Márquez", ISBN = "978-0060883287", AnoPublicacao = 1967, Genero = "Realismo Mágico", Disponivel = true, DataCadastro = DateTime.Now.AddMonths(-3) }, new Livro { Id = 5, Titulo = "O Código Limpo", Autor = "Robert C. Martin", ISBN = "978-8576082675", AnoPublicacao = 2008, Genero = "Tecnologia", Disponivel = false, DataCadastro = DateTime.Now.AddMonths(-2) } };

var ProximoId = 0;
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
//Este get livros retorna a lista de todos os livros.
app.MapGet("/Livros", () => Results.Ok(livros));
//Get /livros/id busca um livro específico pelo seu ID.
app.MapGet("/Livros/{id: int}", (int id) =>
{
    var livro = livros.FirstOrDefault(l => l.Id == id); //FirstOrDefault retorna o primeiro elemento de uma sequência ou um valor padrão se nenhum "null" elemento for encontrado.
    return livro != null ? Results.Ok(livro) : Results.NotFound($"Livro com ID {id} não encontrado."); //if ternário, coloca uma condição que nos retorna verdadeiro ou falso, dependendo do resultado.
});
//Get /livros/gereno/{genero} busca livros por gênero.
app.MapGet("/Livros/genero/{genero}", (string genero) =>
{
    var LivrosFiltrados = livros.Where(livros => livros.Genero.Equals(genero, StringComparison.OrdinalIgnoreCase)).ToList();
    return LivrosFiltrados.Any() ? Results.Ok(LivrosFiltrados) : Results.NotFound($"Nenhum livro encontrado no genero {genero}");
    //quando for propriedade e maisculo, quando for variável é minusculo.
});
//Post /livros criar um novo livro.
app.MapPost("/Livros", (Livro novoLivro) =>
{
    if (string.IsNullOrWhiteSpace(novoLivro.Titulo))
        return Results.BadRequest("Titulo é obrigatorio");

    //Configurar propriedades automaticas
    novoLivro.Id = ProximoId++;
    novoLivro.DataCadastro = DateTime.Now;
    novoLivro.Disponivel = true;
    livros.Add(novoLivro);
    return Results.Created($"/Livros/{novoLivro.Id}", novoLivro);
});

//Put /livros/id atualizar informaçoes de  um livro existente.
app.MapPut("/Livros/{id:int}", (int id, Livro livroAtualizado) =>
{
    var livro = livros.FirstOrDefault(l => l.Id == id);
    if (livro == null)
        return Results.NotFound($"Livro com ID {id} nao encontrado");
    if (string.IsNullOrWhiteSpace(livroAtualizado.Titulo))
        return Results.BadRequest("Titulo é obrigatorio");

    //Atualiza os dados do livro (campos)
    livro.Titulo = livroAtualizado.Titulo;
    livro.Autor = livroAtualizado.Autor;
    livro.ISBN = livroAtualizado.ISBN;
    livro.AnoPublicacao = livroAtualizado.AnoPublicacao;
    livro.Genero = livroAtualizado.Genero;
    livro.Disponivel = livroAtualizado.Disponivel;

    return Results.Ok(livroAtualizado);
});

//Delete /livros/id remover um livro.
app.MapDelete("/Livros/{id:int}", (int id) =>
{
    var livro = livros.FirstOrDefault(L => L.Id == id);
    if (livro == null)
        return Results.NotFound($"Livro com ID {id} nao encontrado");
    livros.Remove(livro);
    return Results.Ok($"Livro com ID {id} remmovido com sucesso");
});

//Patch /livros/id/disponibilidade emprestar um livro, altera o status de disponibilidades do livro.
app.MapPatch("/Livros/{id:int}/disponibilidade", (int id) =>
{
    var livro = livros.FirstOrDefault(l => l.Id == id);
    if (livros == null)
        return Results.NotFound($"Livro com ID {id} nao encontrado");
        livro.Disponivel = !livro.Disponivel;
        return Results.Ok(livros);
});

app.UseHttpsRedirection();

app.Run();

