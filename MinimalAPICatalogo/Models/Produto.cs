using System.Text.Json.Serialization;

namespace MinimalAPICatalogo.Models
{
    public class Produto
    {
        public int ProdutoId { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        public decimal? Preco { get; set; }

        public string? Imagem { get; set; }
        public DateTime DataCompra {  get; set; }
        public int Estoque { get; set; }

        //propriedades de navegação, para reforçar o relacionamento entre categoria-produto
        public int CategoriaID { get; set; } //cria na tabela produtos, uma coluna categoriaid

        [JsonIgnore]
        public Categoria? Categoria { get;set; }
    }
}
