namespace TestPayTech.Dtos;

public class PaymentResponseDto
{
    public string success { get; set; }
    public string token { get; set; }
    public string redirect_url { get; set; }
    public string redirectUrl { get; set; }
}