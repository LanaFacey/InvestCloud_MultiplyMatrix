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
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MultiplyMatrix.Controllers
{
    public class MatrixController : Controller
    {
        private readonly ILogger<MatrixController> _logger;
        public static string BaseUrl = "https://recruitment-test.investcloud.com/api/";
        int[,] Matrix_A = null;
        int[,] Matrix_B = null;
        int[,] Matrix_C = null;
        String MatrixResult = "";
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
                    var content = System.Text.Json.JsonSerializer.Deserialize<Matrix>(res);
                    int index1 = content.Value;
                    int index2 = content.Value;
                    Matrix_A = new int[index1, index2];
                    Matrix_B = new int[index1, index2];
                    Matrix_C = new int[index1, index2];
                    int upperbound = Matrix_A.GetUpperBound(0);
                    int arrayLength = Matrix_A.GetLength(0);

                    for (int r = 0; r < index1; r++) //Populate Matrix A and B
                    {
                        var PopulateRowMatrix = client.GetAsync("numbers/A/row/" + r.ToString()).Result;
                        var PopulateColMatrix = client.GetAsync("numbers/B/col/" + r.ToString()).Result;
                        if (PopulateRowMatrix.IsSuccessStatusCode && PopulateColMatrix.IsSuccessStatusCode)
                        {
                            int Acolumn = 0;
                            int Bcolumn = 0;
                            string resPopulateRowMatrix_A = PopulateRowMatrix.Content.ReadAsStringAsync().Result;
                            string resPopulateRowMatrix_B = PopulateColMatrix.Content.ReadAsStringAsync().Result;
                            var Matrix_A_Row = System.Text.Json.JsonSerializer.Deserialize<MatrixPopulation>(resPopulateRowMatrix_A);
                            var Matrix_B_Col = System.Text.Json.JsonSerializer.Deserialize<MatrixPopulation>(resPopulateRowMatrix_B);
                            foreach (int val in Matrix_A_Row.Value)
                            {
                                Matrix_A[r, Acolumn] = val;
                                Acolumn++;
                            }
                            foreach (int val in Matrix_A_Row.Value)
                            {
                                Matrix_B[r, Bcolumn] = val;
                                Bcolumn++;
                            }
                        }

                    }
                    int aRows = Matrix_A.GetLength(0);
                    int aCols = Matrix_A.GetLength(1);
                    int bRows = Matrix_A.GetLength(0);
                    int bCols = Matrix_A.GetLength(1);
                    Parallel.For(0, aRows, i =>
                      {
                          for (int j = 0; j < bCols; ++j) // each col of B
                              for (int k = 0; k < aCols; ++k) // could use k < bRows
                              {
                                  Matrix_C[i, j] += Matrix_A[i, k] * Matrix_B[k, j];
                                  MatrixResult = MatrixResult + (Matrix_C[i, j]).ToString();
                              }
                      }
                    );
                    //Hash Result using md5 checksum
                    string hash = MatrixResult.Remove(0, 1);
                    //Console.WriteLine(hash);
                    MD5 md5 = System.Security.Cryptography.MD5.Create();
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(hash);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);

                    // Step 2, convert byte array to hex string
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        sb.Append(hashBytes[i].ToString("X2"));
                    }
                    string valstring = sb.ToString();

                    //Validate Result
                    var jsonContent = JsonConvert.SerializeObject(valstring);
                    var contentString = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    contentString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var ValidateMatrix = client.PostAsync("numbers/validate/", contentString).Result;
                    if (ValidateMatrix.IsSuccessStatusCode)
                    {
                        string vres = ValidateMatrix.Content.ReadAsStringAsync().Result;
                        var vcontent = System.Text.Json.JsonSerializer.Deserialize<MatrixValidation>(vres);
                        ViewBag.ValidationResult = vcontent.Value;

                    }
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
