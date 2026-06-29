using Kessler.Domain.Common.Errors;
using Kessler.Domain.Orders;

namespace Domain.Tests.Orders;

public class OrderTests
{
    private static CustomerInfo Customer() => CustomerInfo.Create("Ana", "ANA@Email.com", " 47999 ");

    [Fact]
    public void Create_ComputesTotal_AndGeneratesCode()
    {
        var lines = new[]
        {
            new OrderLine(Guid.NewGuid(), "Manta", 120m, 2),
            new OrderLine(Guid.NewGuid(), "Coelho", 50m, 1),
        };

        var order = Order.Create(Customer(), lines);

        Assert.Equal(290m, order.TotalAmount);
        Assert.StartsWith("KES-", order.Code);
        Assert.Equal(OrderStatus.Pendente, order.Status);
        Assert.Equal(PaymentStatus.Pendente, order.PaymentStatus);
    }

    [Fact]
    public void Create_NoItems_Throws()
    {
        Assert.Throws<DomainException>(() => Order.Create(Customer(), []));
    }

    [Fact]
    public void CustomerInfo_NormalizesEmail()
    {
        var customer = Customer();
        Assert.Equal("ana@email.com", customer.Email);
        Assert.Equal("47999", customer.Phone);
    }

    [Fact]
    public void MarkPaid_ConfirmsPendingOrder()
    {
        var order = Order.Create(Customer(), [new OrderLine(Guid.NewGuid(), "X", 10m, 1)]);

        order.MarkPaid();

        Assert.Equal(PaymentStatus.Pago, order.PaymentStatus);
        Assert.Equal(OrderStatus.Confirmado, order.Status);
    }
}

public class CommissionTests
{
    [Fact]
    public void SendQuote_SetsPriceAndStatus()
    {
        var commission = CommissionRequest.Create(
            CustomerInfo.Create("Bia", "bia@email.com", "11999"),
            "Quero um amigurumi de gato cinza, ~20cm.");

        commission.SendQuote(150m);

        Assert.Equal(150m, commission.QuotedPrice);
        Assert.Equal(CommissionStatus.OrcamentoEnviado, commission.Status);
        Assert.StartsWith("ENC-", commission.Code);
    }
}
