using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Maxter_Management.FunctionsAndMethods
{
    public static class ApiFunctions
    {
        static HttpClient client = new HttpClient();
        //private static string apiKey = "71f52b6dbdb027293a02b9b27c0a2c45";
        private static string apiKey;

        //private static string apiPrivateKey =
            //"3b1d24a68d642cf92485e93ccbca3a84fd32b67e9c31c0a9792a4cf2b78e31599d4c349b96ea6c83ca14f9d9ee0d024d8d102ca0454b4169622ef12fac38dc17";
        private static string apiPrivateKey;

        public static bool LoadKey()
        {
            /*if (File.Exists("key.zip"))
            {
                string zipPath = "key.zip";

                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                        {
                            using (StreamReader reader = new StreamReader(entry.Open()))
                            {
                                apiKey = reader.ReadLine();
                                apiPrivateKey = reader.ReadLine();
                            }
                        }
                    }

                    return true;
                }
            }*/

            try
            {
                if (File.Exists("key.txt"))
                {
                    using (StreamReader reader = new StreamReader(File.Open("key.txt", FileMode.Open)))
                    {
                        apiKey = reader.ReadLine();
                        apiPrivateKey = reader.ReadLine();

                        if (apiKey.Length > 0 && apiPrivateKey.Length > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            return false;
        }

        //public static string RunAsync()
        public static void Run()
        {
            client.BaseAddress = new Uri("https://www.billingo.hu/api");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtToken());

            /*Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Int32 unixTimestampExp = unixTimestamp + 10;

            string header = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
            string payload = $"{{\"sub\":\"{apiKey}\",\"iat\":{unixTimestamp},\"exp\":{unixTimestampExp}}}";

            header = Base64UrlEncoder.Encode(header);
            payload = Base64UrlEncoder.Encode(payload);

            string signature = $"{header}.{payload},{apiPrivateKey}";

            var tokenEnd = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(apiPrivateKey).AddClaim("sub", apiKey)
                .AddClaim("iat", unixTimestamp)
                .AddClaim("exp", unixTimestampExp)
                .Encode();

            string token = $"Bearer {tokenEnd}";

            //File.Create("apiKey.txt");
            //File.WriteAllText("apiKey.txt", $"Bearer {header}.{payload}.{signature}");
            File.WriteAllText("apiKey.txt", token);
            File.WriteAllText("header.txt", header);
            File.WriteAllText("payload.txt", payload);
            File.WriteAllText("signature.txt", signature);

            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Bearer {{\"alg\":\"HS256\",\"typ\":\"JWT\"}}.{{\"sub\":\"71f52b6dbdb027293a02b9b27c0a2c45\",\"iat\":\"{unixTimestamp}\",\"\"exp\":{unixTimestampExp}\"");

            return $"Bearer {header}.{payload}.{signature}";*/
        }

        public static string JwtToken()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Int32 unixTimestampExp = unixTimestamp + 3;

            string header = "{\"alg\":\"HS256\",\"typ\":\"JWT\"}";
            string payload = $"{{\"sub\":\"{apiKey}\",\"iat\":{unixTimestamp-3600},\"exp\":{unixTimestampExp-3600}}}";

            File.WriteAllText("time.txt", unixTimestamp.ToString() + "\n" + unixTimestampExp.ToString());

            header = Base64UrlEncoder.Encode(header);
            payload = Base64UrlEncoder.Encode(payload);

            string signature = $"{header}.{payload},{apiPrivateKey}";

            var tokenEnd = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(apiPrivateKey).AddClaim("sub", apiKey)
                .AddClaim("iat", unixTimestamp)
                .AddClaim("exp", unixTimestampExp)
                .Encode();

            return tokenEnd;
        }

        public static async Task<Uri> CreateInvoiceAsync(Invoice invoice)
        {
            //client.BaseAddress = new Uri("https://www.billingo.hu/api/invoices");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken());

            File.WriteAllText("auth.txt", client.DefaultRequestHeaders.Authorization.ToString());


            //var authenticationHeaderValue = new AuthenticationHeaderValue(JwtToken());
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.Formatting = Formatting.Indented;
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            

            //string invoiceSerialized = JsonConvert.SerializeObject(invoice, Formatting.Indented);
            string invoiceSerialized = JsonConvert.SerializeObject(invoice, serializerSettings);

            File.WriteAllText("json.txt", invoiceSerialized);

            
            var content2 = new StringContent(invoiceSerialized, Encoding.UTF8, "application/json");
            var content = new StringContent(File.ReadAllText("text.txt"), Encoding.UTF8, "application/json");



            //var result = client.PostAsync(client.BaseAddress, content).Result;

            //var response = await client.PostAsJsonAsync(client.BaseAddress, content2);

            var response = await client.PostAsync("https://www.billingo.hu/api/invoices", content2);


            File.WriteAllText("error.txt", $"{response.ReasonPhrase}\n{response.Headers}\n{response.Content}\n{response.RequestMessage}\n{response.StatusCode}");

            string responseBody = await response.Content.ReadAsStringAsync();

            File.WriteAllText("responseBody.txt", responseBody);

            response.EnsureSuccessStatusCode();

            return response.Headers.Location;
        }

        public static async Task<string> CreateInvoiceComplexAsync(Invoice invoice, Client clientParam)
        {
            var outResult1 = CreateClientAsync(clientParam);//Task.Run(() => ApiFunctions.CreateClientAsync(result));

            //string output = outResult.Result.ToString();

            var parsedObject = JObject.Parse(outResult1.Result);

            int id = int.Parse(parsedObject["data"]["id"].ToString());

            invoice.Client_uid = id;

            var outResult2 = await CreateInvoiceAsync(invoice);

            var outResult3 = await DeleteClientAsync(id);

            return outResult3.ToString();

        }

        //public static async Task<Uri> GetInvoiceAsync()
        //public static async Task<List<Invoice>> GetInvoiceAsync()
        public static async Task<string> GetInvoiceAsync()
        {
            try
            {
                //client.BaseAddress = new Uri("https://www.billingo.hu/api/invoices?page=1&max_per_page=20");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken());

                //File.WriteAllText("auth.txt", client.DefaultRequestHeaders.Authorization.ToString());

                var response = await client.GetAsync("https://www.billingo.hu/api/invoices?page=1&max_per_page=20");

            

                var invoiceJson = await response.Content.ReadAsStringAsync();



            

                //invoices = JsonConvert.DeserializeObject<List<Invoice>>(invoiceJson);
                

                File.WriteAllText("invoicesJson.txt", invoiceJson);

                //File.WriteAllText("error2.txt", $"{response.ReasonPhrase}\n{response.Headers}\n{response.Content}\n{response.RequestMessage}\n{response.StatusCode}");

                string responseBody = await response.Content.ReadAsStringAsync();

                File.WriteAllText("responseBody2.txt", responseBody);

                response.EnsureSuccessStatusCode();


                MessageBox.Show("Invoices downloaded successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return responseBody;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Invoices download failed. Exception: {e.Message}", "Debug", MessageBoxButtons.OK);
                return null;
            }
            //return invoices;

            //return response.Headers.Location;
        }

        /*public static Task<string> GetInvoiceAsync_TEST()
        {
            //client.BaseAddress = new Uri("https://www.billingo.hu/api/invoices?page=1&max_per_page=20");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken());

            var response = client.GetAsync("https://www.billingo.hu/api/invoices?page=1&max_per_page=20");



            //var invoiceJson = response.Content.ReadAsStringAsync();


            //response.EnsureSuccessStatusCode();

            return invoiceJson;
        }*/

        //public static async Task<Uri> CreateClientAsync()
        public static async Task<string> CreateClientAsync(Client newClient)
        {
            //client.BaseAddress = null;
            //client.BaseAddress = new Uri("https://www.billingo.hu/api/clients");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken());

            File.WriteAllText("auth.txt", client.DefaultRequestHeaders.Authorization.ToString());


            string output = JsonConvert.SerializeObject(newClient);
            //File.WriteAllText("jsonTest.txt", output);

            var content = new StringContent(output, Encoding.UTF8, "application/json");
            var content2 = new StringContent(File.ReadAllText("textClient.txt"), Encoding.UTF8, "application/json");


            var response = await client.PostAsync("https://www.billingo.hu/api/clients", content/*content2*/).ConfigureAwait(false);


            File.WriteAllText("error.txt", $"{response.ReasonPhrase}\n{response.Headers}\n{response.Content}\n{response.RequestMessage}\n{response.StatusCode}");

            string responseBody = await response.Content.ReadAsStringAsync();//.ConfigureAwait(false);
            

            File.WriteAllText("responseBody.txt", responseBody);

            response.EnsureSuccessStatusCode();

            //return response.Headers.Location;

            //result = response.Content.ReadAsStringAsync().Result;
            return responseBody;
        }

        public static async Task<string> DeleteClientAsync(int id)
        {
            //client.BaseAddress = new Uri("https://www.billingo.hu/api/clients/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken());

            string address = client.BaseAddress + "/" + id.ToString();

            var response = await client.DeleteAsync("https://www.billingo.hu/api/clients/" + id);//.ConfigureAwait(false);

            string responseBody = await response.Content.ReadAsStringAsync();
            File.WriteAllText("DELETEresponseBody.txt", responseBody);

            return responseBody;
        }
    }
}