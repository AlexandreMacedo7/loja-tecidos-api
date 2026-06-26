using LojaTecidos.Domain.Entities;

namespace LojaTecidos.Domain.Tests.Entities;

public class ItemVendaTests
{
    [Fact]
    public void Subtotal_DeveArredondarPorItemComDuasCasas()
    {
        var item = new ItemVenda("INT-001", 1.50m, 10.33m);

        Assert.Equal(15.50m, item.Subtotal);
    }
}
