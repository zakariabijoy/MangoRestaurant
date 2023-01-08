using IdentityModel;
using Mango.Services.IdentityServer.DbContext;
using Mango.Services.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Mango.Services.IdentityServer.Initializer;

public class DbInitailizer : IDbInitializer
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DbInitailizer(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public void Initialize()
    {
        if(_roleManager.FindByNameAsync(SD.Admin).Result is null)
        {
            _roleManager.CreateAsync(new IdentityRole(SD.Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Customer)).GetAwaiter().GetResult();
        }
        else { return; }

        ApplicationUser adminUser = new ()
        {
            UserName = "admin1",
            FirstName = "Ben",
            LastName = "Admin",
            Email = "admin1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "11111111111",
        };

        _userManager.CreateAsync(adminUser,"Admin123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(adminUser,SD.Admin).GetAwaiter().GetResult();

        var temp1 = _userManager.AddClaimsAsync(adminUser, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{adminUser.FirstName} {adminUser.LastName}"),
            new Claim(JwtClaimTypes.GivenName, adminUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, adminUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Admin)
        }).Result;

        ApplicationUser customerUser = new ()
        {
            UserName = "customer1",
            FirstName = "Ben",
            LastName = "Cust",
            Email = "customer1@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "2222222222",
        };

        _userManager.CreateAsync(customerUser, "Cust123*").GetAwaiter().GetResult();
        _userManager.AddToRoleAsync(customerUser, SD.Customer).GetAwaiter().GetResult();

        var temp2 = _userManager.AddClaimsAsync(customerUser, new Claim[]
        {
            new Claim(JwtClaimTypes.Name, $"{customerUser.FirstName} {customerUser.LastName}"),
            new Claim(JwtClaimTypes.GivenName, customerUser.FirstName),
            new Claim(JwtClaimTypes.FamilyName, customerUser.LastName),
            new Claim(JwtClaimTypes.Role, SD.Customer)
        }).Result;
    }
}
