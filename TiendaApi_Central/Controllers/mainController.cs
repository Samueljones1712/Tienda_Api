using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using TiendaApi_Central.Models;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace TiendaApi_Central.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class mainController : ControllerBase
    {

        private readonly HttpClient _httpClient;
        private readonly string urlLogin = "https://localhost:7051/Login";
        private readonly string urlUser = "https://localhost:7051/api/Users";
        public mainController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password) {

            string queryParams = $"?username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(password)}";

            // Realizar la solicitud POST
            HttpResponseMessage response = await _httpClient.PostAsync(urlLogin + queryParams, null);

            // Leer la respuesta como texto
            string responseContent = await response.Content.ReadAsStringAsync();

            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);

            if (tokenResponse != null && tokenResponse.Token != null)
            {
                HttpContext.Session.SetString("JWToken", tokenResponse.Token);

                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + HttpContext.Session.GetString("JWToken"));
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer ", HttpContext.Session.GetString("JWToken"));
            }
                return Ok("Usuario autenticado correctamente");
        }

        [HttpGet]
        public async Task<IActionResult> getUsers()
        {
            using var client = new HttpClient();
            var content = await client.GetStringAsync(urlUser);

            return Ok(content);

        }


    }
}
