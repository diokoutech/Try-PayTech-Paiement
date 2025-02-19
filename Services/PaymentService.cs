using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace TestPayTech.Services
{
    public interface IPaymentService
    {
        Task<Dictionary<string, object>> ProcessPayment(
            string nameItem,
            decimal itemPrice,
            string commandName,
            string reference,
            string successUrl,
            string infoUrl);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IUrlHelper _urlHelper;

        public PaymentService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IUrlHelper urlHelper)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _urlHelper = urlHelper;
        }

        public async Task<Dictionary<string, object>> ProcessPayment(
            string nameItem,
            decimal itemPrice,
            string commandName,
            string reference,
            string successUrl,
            string infoUrl)
        {
            var data = new Dictionary<string, string>
            {
                { "item_name", nameItem },
                { "item_price", itemPrice.ToString() },
                { "currency", _configuration["Payment:Currency"] },
                { "ref_command", reference },
                { "command_name", commandName },
                { "env", _configuration["Payment:TestMode"] == "true" ? _configuration["Payment:StateDev"] : _configuration["Payment:StateProd"] },
                { "success_url", successUrl },
                { "ipn_url", _urlHelper.Action(infoUrl) },
                { "cancel_url", null }
            };

            var content = new FormUrlEncodedContent(data);
            var request = new HttpRequestMessage(HttpMethod.Post, _configuration["Payment:RequestUrl"])
            {
                Content = content
            };
            request.Headers.Add("Content-Type", "application/x-www-form-urlencoded;charset=utf-8");
            request.Headers.Add("API_KEY", _configuration["Payment:ApiKey"]);
            request.Headers.Add("API_SECRET", _configuration["Payment:ApiSecret"]);

            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.SendAsync(request);
            var jsonResponseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonResponseContent);

            if (jsonResponse.ContainsKey("token"))
            {
                return new Dictionary<string, object>
                {
                    { "success", 1 },
                    { "token", jsonResponse["token"] },
                    { "redirect_url", $"{_configuration["Payment:BaseUrl"]}{_configuration["Payment:PaymentRedirectPath"]}{jsonResponse["token"]}" }
                };
            }
            else if (jsonResponse.ContainsKey("error"))
            {
                return new Dictionary<string, object>
                {
                    { "success", -1 },
                    { "errors", jsonResponse["error"] }
                };
            }
            else
            {
                return new Dictionary<string, object>
                {
                    { "success", -1 },
                    { "errors", new List<string> { "Internal Error" } }
                };
            }
        }
    }
}