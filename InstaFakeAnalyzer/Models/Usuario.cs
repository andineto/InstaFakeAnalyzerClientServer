namespace InstaFakeAnalyzer.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string NomeUsuario { get; set; }
        public string Senha { get; set; }
        public DateTime DataCadastro { get; set; }
        public TipoUsuario? tipoUsuario { get; set; }
        public bool? snAtivo { get; set; } = true;
    }
}
