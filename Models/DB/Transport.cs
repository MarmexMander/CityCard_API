using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace CityCard_API.Models.DB;

public class Transport{
    [Key]
    public string LicensePlate { get; set; }
    public TransportType Type { get; set; }
    public City City { get; set; }
    public List<Terminal> Terminals{ get; set; }
}