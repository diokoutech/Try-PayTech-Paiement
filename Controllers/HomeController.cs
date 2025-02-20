using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TestPayTech.Database;
using TestPayTech.Database.Entities;
using TestPayTech.Dtos;
using TestPayTech.Helpers;
using TestPayTech.Models;
using TestPayTech.Services;

namespace TestPayTech.Controllers;

public class HomeController(AppDbContext context, ILogger<HomeController> logger, IConfiguration _config, IHttpClientFactory _clientFactory)
    : Controller
{
   
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
    
    [HttpPost]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("payment")]
    public async Task<JsonResult> Payment(PaymentRequestDto payment)
    {
        try
        {
            // Initialiser le paiement avec les données issues du front
            var nomPayment = "Test Payment";
            var montantTotal = 1000;
            var reference = Guid.NewGuid().ToString();
            
            // récupération des informations de configuration
            var apiSecret = _config["Payment:ApiSecret"];
            var apikey = _config["Payment:ApiKey"];
            var currency = _config["Payment:Currency"];
            var env = _config["Payment:Env"];
            var requestUrl = _config["Payment:RequestUrl"];
            var baseUrl = _config["Payment:BaseUrl"];
            var ipnUrl = _config["Payment:IpnUrl"];
            var successUrl = _config["Payment:SuccessUrl"];
            var cancelUrl = _config["Payment:CancelUrl"];
            
            
            // envoi de la requête
            var datas = new Dictionary<string, string>
            {
                { "item_name", nomPayment },
                { "item_price", montantTotal.ToString() },
                { "currency", currency },
                { "ref_command", reference },
                { "env", env },
                { "success_url", successUrl },
                { "ipn_url", ipnUrl },
                { "cancel_url", cancelUrl }
            };
            
            var content = new FormUrlEncodedContent(datas);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            
            var client = _clientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            
            client.DefaultRequestHeaders.Add("API_KEY", apikey);
            client.DefaultRequestHeaders.Add("API_SECRET",apiSecret );        

            var response = await client.PostAsync(requestUrl, content);
            if (!response.IsSuccessStatusCode)
                return Json(null);
            var data = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonSerializer.Deserialize<object>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Console.WriteLine(paymentResponse);
            return Json(paymentResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Json(null);
        }

    }

    [Route("paiement/webhook")]
    public JsonResult WebHook(PaymentPayTechNotificationDto? paymentNortificationDto)
    {
        // vérification de la notification
        var apiSecret = _config["Payment:ApiSecret"];
        var apikey = _config["Payment:ApiKey"];
        
        // vérification du paiement (annulé ou finalisé)
        if(paymentNortificationDto.Type_Event.Equals(Utils.StatutPaiement.Canceled))
            return Json(null);

        // vérification des clés hashées
        if (!Utils.VerifyPaytechRequest(paymentNortificationDto.Api_Key_Sha256,paymentNortificationDto.Api_Secret_Sha256, apiSecret, apikey))
            return Json(null);
        
        // commencer le traitement des opérations après le succés du paiement effectué
        var payment = new Paiement()
        {
            MethodePaiement = paymentNortificationDto.Payment_method,
            Currency = paymentNortificationDto.Currency,
            NomProduitPaiement = paymentNortificationDto.Item_Name,
            Reference = paymentNortificationDto.Ref_Command,
            TelephoneClient = paymentNortificationDto.Client_phone,
            MontantTotal = paymentNortificationDto.Item_Price
        };
        
        context.Paiements.Add(payment);
        context.SaveChanges();
        return Json(paymentNortificationDto);
    }
    
    
    [Route("paiement/success")]
    public JsonResult WebHookSuccess(PaymentPayTechNotificationDto paymentNortificationDto)
    {
        // vérification de la notification
        var apiSecret = _config["Payment:ApiSecret"];
        var apikey = _config["Payment:ApiKey"];
        
        // vérification du paiement (annulé ou finalisé)
        if(paymentNortificationDto.Type_Event.Equals(Utils.StatutPaiement.Canceled))
            return Json(null);

        // vérification des clés hashées
        if (!Utils.VerifyPaytechRequest(paymentNortificationDto.Api_Key_Sha256,paymentNortificationDto.Api_Secret_Sha256, apiSecret, apikey))
            return Json(null);
        
        // commencer le traitement des opérations après le succés du paiement effectué
        var payment = new Paiement()
        {
            MethodePaiement = paymentNortificationDto.Payment_method,
            Currency = paymentNortificationDto.Currency,
            NomProduitPaiement = paymentNortificationDto.Item_Name,
            Reference = paymentNortificationDto.Ref_Command,
            TelephoneClient = paymentNortificationDto.Client_phone,
            MontantTotal = paymentNortificationDto.Item_Price
        };
        
        context.Paiements.Add(payment);
        context.SaveChanges();
        return Json(paymentNortificationDto);
    }
    
    
    [Route("paiement/cancel")]
    public JsonResult WebHookCanceled(PaymentPayTechNotificationDto paymentNortificationDto)
    {
        // vérification de la notification
        var apiSecret = _config["Payment:ApiSecret"];
        var apikey = _config["Payment:ApiKey"];
        
        // vérification du paiement (annulé ou finalisé)
        if(paymentNortificationDto.Type_Event.Equals(Utils.StatutPaiement.Canceled))
            return Json(null);

        // vérification des clés hashées
        if (!Utils.VerifyPaytechRequest(paymentNortificationDto.Api_Key_Sha256,paymentNortificationDto.Api_Secret_Sha256, apiSecret, apikey))
            return Json(null);
        
        // commencer le traitement des opérations après le succés du paiement effectué
        var payment = new Paiement()
        {
            MethodePaiement = paymentNortificationDto.Payment_method,
            Currency = paymentNortificationDto.Currency,
            NomProduitPaiement = paymentNortificationDto.Item_Name,
            Reference = paymentNortificationDto.Ref_Command,
            TelephoneClient = paymentNortificationDto.Client_phone,
            MontantTotal = paymentNortificationDto.Item_Price
        };
        
        context.Paiements.Add(payment);
        context.SaveChanges();
        return Json(paymentNortificationDto);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}