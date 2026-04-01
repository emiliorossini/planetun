
public enum StatusDesembolso
{
    Solicitado,
    Aprovado,
    Pago
}

public class DesembolsoSolicitadoEvent : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
    public decimal Valor { get; }
    public DesembolsoSolicitadoEvent(decimal valor) => Valor = valor;
}

public class DesembolsoPagoEvent : IDomainEvent
{
    public DateTime OcorridoEm { get; } = DateTime.UtcNow;
    public decimal Valor { get; }
    public DesembolsoPagoEvent(decimal valor) => Valor = valor;
}

public class Desembolso : AggregateRoot
{
    public decimal Valor { get; private set; }
    public StatusDesembolso Status { get; private set; }

    private Desembolso() { }

    public static Desembolso Solicitar(decimal valor)
    {
        var desembolso = new Desembolso
        {
            Valor = valor,
            Status = StatusDesembolso.Solicitado
        };
        desembolso.AddEvent(new DesembolsoSolicitadoEvent(valor));
        return desembolso;
    }

    public void Aprovar()
    {
        if (Status != StatusDesembolso.Solicitado)
            throw new InvalidOperationException("Apenas desembolsos solicitados podem ser aprovados.");

        Status = StatusDesembolso.Aprovado;
    }

    public void Pagar()
    {
        if (Status != StatusDesembolso.Aprovado)
            throw new InvalidOperationException("Apenas desembolsos aprovados podem ser pagos.");

        Status = StatusDesembolso.Pago;
        AddEvent(new DesembolsoPagoEvent(Valor));
    }
}
