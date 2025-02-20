namespace TestPayTech.Dtos;

public class PaymentPayTechNotificationDto
{
        public string Type_Event { get; set; }
        public string Custom_Field { get; set; }
        public string Client_phone { get; set; }
        public string Payment_method { get; set; }
        public string Ref_Command { get; set; }
        public string Item_Name { get; set; }
        public decimal Item_Price { get; set; }
        public string Currency { get; set; }
        public string Command_Name { get; set; }
        public string Env { get; set; }
        public string Token { get; set; }
        public string Api_Key_Sha256 { get; set; }
        public string Api_Secret_Sha256 { get; set; }
}

