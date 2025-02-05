using CityCard_API.Models.DB;
using Microsoft.AspNetCore.Identity;

public class CCUser : IdentityUser
{
    public List<Account> Accounts { get; set; }
}