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

/// <summary>Ciclo de vida de uma encomenda sob medida — colunas do Kanban do ateliê.</summary>
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

/// <summary>
/// Natureza de um item do quadro do ateliê. Encomenda vem do cliente; os demais são
/// trabalhos que a artista cria por conta própria (sem cliente obrigatório).
/// </summary>
public enum WorkType
{
    /// <summary>Pedido sob medida de um cliente (fluxo público ou cadastrado pela artista).</summary>
    Encomenda = 0,
    ProjetoPessoal = 1,
    Estoque = 2,
    Amostra = 3,
    Reparo = 4,
    Presente = 5,
    Evento = 6,
    Estudo = 7
}

/// <summary>Prioridade de um item no quadro.</summary>
public enum WorkPriority
{
    Baixa = 0,
    Normal = 1,
    Alta = 2
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
