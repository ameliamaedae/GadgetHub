using System.ComponentModel.DataAnnotations;

namespace GadgetHub.WebUI.Models;

public class CheckoutViewModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    [Required] public string FullName { get; set; }

    [Required] public string Address { get; set; }

    [Required] public string City { get; set; }

    public string State { get; set; }

    [Required] public string ZipCode { get; set; }

    [Required] public string Country { get; set; }

    public List<CartItem> CartItems { get; set; } = new();

    public decimal Total { get; set; }
}