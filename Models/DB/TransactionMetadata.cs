namespace CityCard_API.Models.DB;

public class TransactionMetadata
{
    public Guid Id { get; set; }
    public Terminal Terminal { get; set; }
    public Price? PriceUsed { get; set; }
}