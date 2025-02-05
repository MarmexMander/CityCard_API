namespace CityCard_API.Models.DB;
public class Terminal{
    public Guid Id { get; set; }
    public string? LocationAddress { get; set; }
    public Transport? LocationTransport { get; set; }
}