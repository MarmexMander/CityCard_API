namespace CityCard_API.Models.DB;

public class Account{
    public Guid Id { get; set;}
    public float Amount { get; set;}
    public AccountType AccountType { get; set;}
    public CCUser? User { get; set;}
    public DateTime? ValidUntill { get; set;}
}