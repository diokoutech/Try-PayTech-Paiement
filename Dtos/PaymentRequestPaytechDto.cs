namespace TestPayTech.Dtos;

public class PaymentRequestPaytechDto
{
    public string Item_Name { get; set; }
    public decimal Item_Price { get; set; }
    public string Currency { get; set; }
    public string Ref_Command { get; set; }
    public string Env { get; set; }
    public string success_url { get; set; }
    public string Ipn_Url { get; set; }
    public string Cancel_url { get; set; }
}