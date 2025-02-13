using CityCard_API.Extentions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CityCard_API.Models.DB;
public class CityCardDBContext : IdentityDbContext
{
    public DbSet<CCUser> CCUsers { get; set; }
    public DbSet<AccountType> AccountTypes { get; set; }
    public DbSet<TransportType> TransportTypes { get; set; }
    public DbSet<City> Cities{ get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountTransaction> Transactions { get; set; }
    public DbSet<TransactionMetadata> TransactionMetadata { get; set; }
    public DbSet<Transport> Transports { get; set; }
    public DbSet<Terminal> Terminals { get; set; }
    public DbSet<TerminalToken> TerminalTokens { get; set; }
     public DbSet<AdminToken> AdminTokens { get; set; }

    public CityCardDBContext(DbContextOptions<CityCardDBContext> options)
    :base(options)
    {
        
    }
    override protected void OnModelCreating(ModelBuilder builder){
        var account = builder.Entity<Account>();
        account.HasOne(a => a.User).WithMany(u=>u.Accounts);
        account.HasOne(a => a.AccountType).WithMany();

        var transaction = builder.Entity<AccountTransaction>();
        transaction.HasOne(t => t.Metadata).WithOne().HasForeignKey<TransactionMetadata>(m => m.Id);
        transaction.HasOne(t => t.Account).WithMany();

        builder.Entity<CCUser>().HasMany(u=>u.Accounts).WithOne();

        var price = builder.Entity<Price>();
        price.HasOne(p=>p.City).WithMany();
        price.HasOne(p=>p.TransportType).WithMany();
        price.HasOne(p => p.AccountType).WithMany();
        //price.HasAlternateKey("City", "TransportType", "AccountType");

        builder.Entity<Terminal>().HasOne(t => t.LocationTransport).WithMany();

        builder.Entity<TerminalToken>().HasOne(tt=>tt.Terminal).WithMany();

        var tMetadata = builder.Entity<TransactionMetadata>();
        tMetadata.HasOne(t => t.Terminal).WithMany();
        tMetadata.HasOne(t => t.PriceUsed).WithMany();
        builder.Entity<AdminToken>()
            .HasOne(t => t.Admin)
            .WithMany()
            .HasForeignKey(t => t.AdminId);
        //Enums seeding
        builder.Entity<AccountType>().SeedEnumValues<AccountType, AccountTypeEnum>(x => x);
        builder.Entity<TransportType>().SeedEnumValues<TransportType, TransportTypeEnum>(x => x);

        //Seeding
        var adminRole = new CCRole(){
            Id = "87d24e3b-489d-e621-9123-c808f97722b9",
            Name = "Admin",
            ConcurrencyStamp = "87d24e3b-489d-e621-9123-c808f97722b9"
        };
        var userRole = new CCRole(){
            Id = "b35d45df-1693-464c-9086-17c11d02de05",
            Name = "User",
            ConcurrencyStamp = "b35d45df-1693-464c-9086-17c11d02de05"
        };

        var roles = new List<CCRole>(){
            adminRole, 
            userRole
        };
        builder.Entity<CCRole>().HasData(roles);
        var hasher = new PasswordHasher<IdentityUser>();
        var adminUser = new IdentityUser(){
            Id = "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664",
            SecurityStamp = "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664",
            ConcurrencyStamp = "d1cdc3fb-4bc1-434d-85a6-e366a0cb7664",
            UserName = "admin",
            Email = "admin@citycard.ua",
            EmailConfirmed = true,
        };
        adminUser.PasswordHash = "AQAAAAIAAYagAAAAEIlZJu/0K1lq+uZqp2F0JMTkhm2GJV8YCgUUTyOQzA4LFarHrgIbSd6m+WA0HuffEQ==";
        builder.Entity<CCUser>().HasData(adminUser);

        builder.Entity<IdentityUserRole<string>>()
        .HasData(
            new IdentityUserRole<string>{
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            }
        );

        builder.Entity<City>().HasData(
            new City{
                Id = -1,
                Region = "Volynska",
                CityName = "Lutsk"
            },
            new City{
                Id = -2,
                Region = "Kovelska",
                CityName = "Kovel"
            }
        );

        base.OnModelCreating(builder);
    }
}