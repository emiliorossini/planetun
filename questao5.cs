    public sealed class Uf
    {
        public string Codigo { get; }

        public Uf(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("UF inválida");

            Codigo = codigo.ToUpper();
        }
    }

    public interface IAliquotaStrategy
    {
        bool AplicaPara(Uf uf);
        decimal ObterAliquota();
    }

    public class SaoPauloAliquota : IAliquotaStrategy
    {
        public bool AplicaPara(Uf uf) => uf.Codigo == "SP";
        public decimal ObterAliquota() => 0.18m;
    }

    public class RioDeJaneiroAliquota : IAliquotaStrategy
    {
        public bool AplicaPara(Uf uf) => uf.Codigo == "RJ";
        public decimal ObterAliquota() => 0.20m;
    }

    public class DefaultAliquota : IAliquotaStrategy
    {
        public bool AplicaPara(Uf uf) => true;
        public decimal ObterAliquota() => 0.17m;
    }

    public class CalculadoraIcms
    {
        private readonly IEnumerable<IAliquotaStrategy> _strategies;

        public CalculadoraIcms(IEnumerable<IAliquotaStrategy> strategies)
        {
            _strategies = strategies;
        }

        public decimal Calcular(decimal valor, Uf uf)
        {
            if (valor <= 0)
                throw new ArgumentException("Valor deve ser maior que zero");

            if (uf is null)
                throw new ArgumentNullException(nameof(uf));

            var strategy = _strategies.First(s => s.AplicaPara(uf));
            var aliquota = strategy.ObterAliquota();

            return valor * aliquota;
        }
    }
