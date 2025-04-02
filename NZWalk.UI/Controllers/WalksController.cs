using Microsoft.AspNetCore.Mvc;
using NZWalk.UI.Models;
using NZWalk.UI.Models.DTO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NZWalk.UI.Controllers
{
    public class WalksController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public WalksController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }
        public async Task<IActionResult> Index()
        {
            List<WalkDto> response = new List<WalkDto>();
            try
            {
                var client = httpClientFactory.CreateClient();
                //doubtfull code
                var httpResponseMessage = await client.GetAsync("https://localhost:7149/api/walks");

                httpResponseMessage.EnsureSuccessStatusCode();

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<WalkDto>>());

                ViewBag.Response = response;
            }
            catch (Exception)
            {
                //log error
            }

            return View(response);
        }
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(AddWalkViewModel model)
        {
            var client = httpClientFactory.CreateClient();

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7149/api/walks"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();
            var response = await httpResponseMessage.Content.ReadFromJsonAsync<WalkDto>();

            if(response is not null)
            {
                return RedirectToAction("Index", "Walks");
            }
            return View();
        }
        public async Task<IActionResult> Edit(Guid id)
        {
            var client = httpClientFactory.CreateClient();

            var response = await client.GetFromJsonAsync<WalkDto>($"https://localhost:7149/api/walks/{id.ToString()}");

            if(response is not null)
            {
                return View(response);
            }
            
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(WalkDto request)
        {
            var client = httpClientFactory.CreateClient();
            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7149/api/walks/{request.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };
            
            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<WalkDto>();

            if(response is not null)
            {
                return RedirectToAction("Edit", "Walks");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(WalkDto request)
        {
            try
            {
                var client = httpClientFactory.CreateClient();

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7149/api/walks/{request.Id}");

                httpResponseMessage.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Walks");
            }
            catch (Exception)
            {
                //Log Error
            }
            return View("Edit");

        }
    }
}
