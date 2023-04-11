using System.Text.Json.Serialization;

namespace MinimalAPICatalogo.Models
{
    public class Categoria
    {
        public int CategoriaId { get; set; }
        public string? Nome { get; set; }

        public string? Descricao { get; set; }

        // atribuir uma propriedade 1 para muitos

        [JsonIgnore] //não irá mostrar os dados referentes a produtos no swagger na hora de adicionar algo a categoria
        public ICollection<Produto> ? Produtos { get; set; } 
        //propriedade de navegação - Produto
    }
}
