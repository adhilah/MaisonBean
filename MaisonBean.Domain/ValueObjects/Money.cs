namespace MaisonBean.Domain.ValueObjects;

public record Money(decimal Amount, string Currency = "USD")
{
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add different currencies.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Multiply(int quantity) => new(Amount * quantity, Currency);
}