using System.ComponentModel.DataAnnotations;

namespace TestPayTech.Dtos;

public class PaymentRequestDto
{
    [Required]
    public int MontantTotal { get; set; }
    [Required]
    public string Reference { get; set; }
    [Required]
    public string NomPayment { get; set; }
}