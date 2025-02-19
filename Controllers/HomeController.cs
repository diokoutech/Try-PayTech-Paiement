using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TestPayTech.Models;
using TestPayTech.Services;

namespace TestPayTech.Controllers;

public class HomeController(ILogger<HomeController> logger)
    : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    // private readonly IPaymentService _paymentService = paymentService;

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }
        
    [Route("payment")]
    public JsonResult Payment(int IdDossier, int Montant)
    {
        var paymentResponse = new Dictionary<string, object>
        {
            { "success", 1 },
            { "token", "405gzopm7b40bsm" },
            { "redirect_url", "https://paytech.sn/payment/checkout/405gzopm7b40bsm" },
            { "redirectUrl", "https://paytech.sn/payment/checkout/405gzopm7b40bsm" }
        };
        return Json(paymentResponse);
    }

    [Route("paiement/webhook")]
    public JsonResult WebHook()
    {
        var paymentResponse = new Dictionary<string, object>
        {
            { "success", 1 },
            { "token", "405gzopm7b40bsm" },
            { "redirect_url", "https://paytech.sn/payment/checkout/405gzopm7b40bsm" },
            { "redirectUrl", "https://paytech.sn/payment/checkout/405gzopm7b40bsm" }
        };
        return Json(paymentResponse);
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}