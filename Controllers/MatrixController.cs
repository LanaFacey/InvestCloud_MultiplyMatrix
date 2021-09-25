using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultiplyMatrix.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace MultiplyMatrix.Controllers
{
    public class MatrixController : Controller
    {
        private readonly ILogger<MatrixController> _logger;
        public static string BaseUrl = "https://recruitment-test.investcloud.com/api/";
        int[,] Matrix_A = null;
        int[,] Matrix_B = null;
        public MatrixController(ILogger<MatrixController> logger)
        {
            _logger = logger;
        }

        public void GetMatricesFromWebAPI()
        {
            using (System.Net.Http.HttpClient client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);

                var InitilizeMatrix = client.GetAsync("numbers/init/999").Result;

                if (InitilizeMatrix.IsSuccessStatusCode)
                {
                    string res = InitilizeMatrix.Content.ReadAsStringAsync().Result;
                    var content = JsonSerializer.Deserialize<Matrix>(res);
                    int index1 = content.Value;
                    int index2 = content.Value;
                    Matrix_A = new int[index1, index2];
                    Matrix_B = new int[index1, index2];

                    var PopulateRowMatrix = client.GetAsync("numbers/init/999").Result;
                    var PopulateColumnMatrix = client.GetAsync("numbers/init/999").Result;

                    for(int i = 0; i <= index1; i++)
                    {

                    }
                }

                foreach (string element in matrix)
                {

                }
            }

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
