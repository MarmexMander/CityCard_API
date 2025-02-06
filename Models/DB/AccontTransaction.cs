namespace CityCard_API.Models.DB;

public class AccountTransaction{
    public Guid Id { get; set; }
    public Account Account{ get; set; }
    public float Amount { get; set; }
    public TransactionMetadata Metadata { get; set; }
    public DateTime Timestamp { get; set; }
}