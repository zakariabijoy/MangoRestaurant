using static UI.Pages.Login.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.IdentityServer.Pages.Account.Register;

public class RegisterViewModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    [Required]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
    public string RoleName { get; set; }
}