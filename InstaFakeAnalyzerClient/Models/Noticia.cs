using InstaFakeAnalyzer.Utils;

namespace InstaFakeAnalyzerClient.Models
{
    public class Noticia
    {
        public int? Id { get; set; }
        public string? Conteudo { get; set; }
        public string? Justificativa { get; set; }
        public bool? snFalsa { get; set; }
        public bool? snAnalizada { get; set; }
        public DateTime? DataCadastro { get; set; }
        public string Md5 => MD5.Md5Hash(Conteudo);
        public int Tipo = 1;
    }
}