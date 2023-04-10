namespace MinimalAPICatalogo.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        // atribuir uma propriedade 1 para muitos

        public ICollection<Produto> ? Produtos { get; set; } 
        //propriedade de navegação - Produto
    }
}
