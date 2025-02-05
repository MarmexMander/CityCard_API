
using CityCard_API.Models.DB;

public class TerminalToken
{
    public int Id { get; set; }
    public Terminal Terminal { get; set; }
    public string TokenHash { get; set; }
    public DateTime ValidUntil { get; set; }
}