namespace Kessler.Domain.Orders;

/// <summary>Ciclo de vida de um pedido de peças prontas.</summary>
public enum OrderStatus
{
    Pendente = 0,
    Confirmado = 1,
    EmProducao = 2,
    Enviado = 3,
    Concluido = 4,
    Cancelado = 5
}

/// <summary>Ciclo de vida de uma encomenda sob medida.</summary>
public enum CommissionStatus
{
    Nova = 0,
    EmAnalise = 1,
    OrcamentoEnviado = 2,
    Aprovada = 3,
    EmProducao = 4,
    Concluida = 5,
    Recusada = 6
}

public enum PaymentMethod
{
    /// <summary>Combinado manualmente (Pix/WhatsApp) — padrão do v1.</summary>
    Manual = 0,
    Pix = 1,
    Gateway = 2
}

public enum PaymentStatus
{
    Pendente = 0,
    Pago = 1,
    Estornado = 2
}
