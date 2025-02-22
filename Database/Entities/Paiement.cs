using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TestPayTech.Database.Entities;

[Index(nameof(IdDossier), IsUnique = true)]
public class Paiement
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public string? MethodePaiement { get; set; }
    [Required]
    public decimal MontantTotal { get; set; }
    public string? Reference { get; set; }
    public string?  Currency { get; set; } 
    [Required]
    public string IdDossier { get; set; } 
    [Required]
    public string?  NomProduitPaiement { get; set; }
    public string? TelephoneClient { get; set; }
    public string? StatutPaiement { get; set; }
    public DateTime? DateModificationWebhook { get; set; }
}