using System.ComponentModel.DataAnnotations;

namespace TestPayTech.Database.Entities;

public class Paiement
{
    [Key]
    public int Id { get; set; }
    public DateTime DateCreation { get; set; } = DateTime.Now;
    public string MethodePaiement { get; set; }
    public decimal MontantTotal { get; set; }
    public string Reference { get; set; }
    public string  Currency { get; set; }
    public string  NomProduitPaiement { get; set; }
    public string TelephoneClient { get; set; }
}