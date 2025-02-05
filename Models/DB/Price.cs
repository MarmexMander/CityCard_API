using Microsoft.EntityFrameworkCore;
namespace CityCard_API.Models.DB;

public enum TransportTypeEnum{
    Bus = 1, Trolleybus
}

public enum AccountTypeEnum{
    General = 1, Student, Child, Manager, Driver, Pensionare
}



public class Price{
    public int Id { get; set; }
    public City City { get; set; }
    public TransportType TransportType { get; set; }
    public AccountType AccountType {get; set;}
    public float Amount { get; set;}

}

public class TransportType
{
    public TransportType(TransportTypeEnum transportTypeEnum)
    {
        Name = Enum.GetName(typeof(TransportTypeEnum), transportTypeEnum);
        Id = (int)transportTypeEnum;
    }

    public TransportType(){
        Name = "Bus";
    }

    public int Id { get; set; }
    public string Name { get; set;}

    public static implicit operator TransportType(TransportTypeEnum tte) 
    => new(tte);    
}

public class AccountType
{
    public AccountType(AccountTypeEnum accountTypeEnum)
    {
        Name = Enum.GetName(typeof(AccountTypeEnum), accountTypeEnum);
        Id = (int)accountTypeEnum;
    }
    public int Id { get; set; }
    public AccountType(){
        Name = "General";
    }

    public string Name { get; set;}

    public static implicit operator AccountType(AccountTypeEnum ate) 
    => new(ate);
}