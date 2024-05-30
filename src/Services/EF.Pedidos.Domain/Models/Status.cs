namespace EF.Pedidos.Domain.Models;

public enum Status
{
    AguardandoPagamento = 0,
    Recebido = 1,
    EmPreparacao = 2,
    Pronto = 3,
    Finalizado = 4
}