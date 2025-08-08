using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestPayTech.Database;
using TestPayTech.Database.Entities;
using TestPayTech.Dtos;
using TestPayTech.Helpers;
using TestPayTech.Services;

namespace TestPayTech.Controllers;

public class HomeController(AppDbContext context, ILogger<HomeController> logger, IConfiguration _config, IHttpClientFactory _clientFactory)
    : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Paiement()
    {
        var liste = context.Paiements.OrderByDescending(p => p.DateCreation).ToList();
        return View(liste);
    }
    [HttpPost]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("payment")]
    public async Task<JsonResult> Payment(PaymentRequestDto payment)
    {
        try
        {
            // Initialiser le paiement avec les données issues du front
            var newReference = Guid.NewGuid() + "-" + payment.Reference;
            // récupération des informations de configuration
            var apiSecret = _config["Payment:ApiSecret"];
            var apikey = _config["Payment:ApiKey"];
            var currency = _config["Payment:Currency"];
            var env = _config["Payment:Env"];
            var requestUrlPaytech = _config["Payment:RequestUrlPaytech"];
            var baseUrlApp = _config["Payment:BaseUrlApp"];
            var ipnUrl = _config["Payment:IpnUrl"];
            var successUrl = _config["Payment:SuccessUrl"];
            var cancelUrl = _config["Payment:CancelUrl"];
            // envoi de la requête
            var datas = new Dictionary<string, string>
            {
                { "item_name",  payment.NomPayment },
                { "item_price", payment.MontantTotal.ToString() },
                { "currency", currency  },
                { "ref_command", newReference },
                { "custom_field", "123" },
                { "env", env },
                { "success_url", string.Format("{0}{1}/{2}", baseUrlApp, successUrl,payment.Reference) },
                { "ipn_url", baseUrlApp + ipnUrl },
                { "cancel_url", string.Format("{0}{1}/{2}", baseUrlApp, cancelUrl,payment.Reference)}
                
            };
            var content = new FormUrlEncodedContent(datas);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("API_KEY", apikey);
            client.DefaultRequestHeaders.Add("API_SECRET",apiSecret );
            var response = await client.PostAsync(new Uri(requestUrlPaytech), content);
            if (!response.IsSuccessStatusCode)
                return Json(null);
            var data = await response.Content.ReadAsStringAsync();
            var paymentResponse = JsonSerializer.Deserialize<object>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            // initialiser paiement
            var paymentNew = new Paiement()
            {
                NomProduitPaiement = payment.NomPayment,
                Reference = newReference,
                MontantTotal = payment.MontantTotal,
                StatutPaiement = "EnBrouillon",
                IdDossier = payment.Reference,
                Currency = currency
            };
            context.Paiements.Add(paymentNew);
            await context.SaveChangesAsync();
            return Json(paymentResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Json(null);
        }
    }
    [Route("paiement/webhook")]
    public void WebHook(PaymentPayTechNotificationDto? paymentNortificationDto)
    {
        try
        {
            // vérification de la notification
            var apiSecret = _config["Payment:ApiSecret"];
            var apikey = _config["Payment:ApiKey"];
            // vérification des clés hashées
            if (!Utils.VerifyPaytechRequest(paymentNortificationDto.Api_Key_Sha256,
                    paymentNortificationDto.Api_Secret_Sha256, apikey,apiSecret)) return;
            // commencer le traitement des opérations après le succés du paiement effectué
            var payment =  context.Paiements.FirstOrDefault(p => p.Reference == paymentNortificationDto.Ref_Command);
            if(payment == null) return;
            payment.MethodePaiement = paymentNortificationDto.Payment_method;
            payment.Currency = paymentNortificationDto.Currency;
            payment.NomProduitPaiement = paymentNortificationDto.Item_Name;
            payment.Reference = paymentNortificationDto.Ref_Command;
            payment.TelephoneClient = paymentNortificationDto.Client_phone;
            payment.MontantTotal = paymentNortificationDto.Item_Price;
            payment.StatutPaiement = paymentNortificationDto.Type_Event.Equals(Utils.StatutPaiement.Canceled) ? "Non Payé" : "Payé";
            payment.DateModificationWebhook = DateTime.Now;
            context.Paiements.Update(payment);
            context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    [Route("paiement/success/{reference}")]
    public async Task<IActionResult> PaiementSuccess(string reference)
    {
        // recupération du paiement
        return RedirectToAction("Index");
    }
    [Route("paiement/cancel/{reference}")]
    public async Task<IActionResult> PaiementCanceled(string  reference)
    {
        // recupération du paiement
        var paiment = context.Paiements.FirstOrDefault(p => p.Reference == reference);
        if(paiment == null)
               return RedirectToAction("Index");
        paiment.StatutPaiement = "Canceled";
        context.Paiements.Update(paiment);
        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

}