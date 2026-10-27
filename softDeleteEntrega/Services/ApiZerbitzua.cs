using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using softDeleteEntrega.Models;

namespace softDeleteEntrega.Services
{
    public sealed class ApiZerbitzua : IDisposable
    {
        private readonly HttpClient client;
        private readonly JavaScriptSerializer json = new JavaScriptSerializer();

        public ApiZerbitzua() : this(new HttpClient())
        {
        }

        public ApiZerbitzua(HttpClient client)
        {
            this.client = client;
            var helbidea = ConfigurationManager.AppSettings["ApiBaseUrl"];
            this.client.BaseAddress = new Uri((helbidea ?? "http://localhost:5289/").TrimEnd('/') + "/");
            this.client.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<Erabiltzailea> LoginAsync(int bazkideId)
        {
            if (bazkideId <= 0)
                throw new ArgumentOutOfRangeException(nameof(bazkideId), "Bazkidearen ID-ak positiboa izan behar du.");

            var result = await BidaliAsync<Erabiltzailea>(HttpMethod.Post, "api/Bazkidea/login",
                json.Serialize(new { Id = bazkideId }), true);
            if (result == null) return null;
            if (result.Data == null || result.Data.Id != bazkideId)
                throw new InvalidOperationException("APIak ez du eskatutako bazkidea itzuli.");
            return result.Data;
        }

        public Task<Pista> DesgaituAsync(int id)
        {
            return EzarriEgoeraAsync(id, "desgaitu");
        }

        public Task<Pista> BerreskuratuAsync(int id)
        {
            return EzarriEgoeraAsync(id, "berreskuratu");
        }

        private async Task<Pista> EzarriEgoeraAsync(int id, string ekintza)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Pistaren ID-ak positiboa izan behar du.");

            var result = await BidaliAsync<Pista>(new HttpMethod("PATCH"),
                "api/Pista/" + id + "/" + ekintza, "{}");
            if (result.Data == null || result.Data.Id != id ||
                result.Data.Desgaituta != (ekintza == "desgaitu"))
                throw new InvalidOperationException("APIaren erantzuna ez da espero zena. Berritu zerrenda egoera egiaztatzeko.");

            return result.Data;
        }

        private async Task<ApiErantzuna<T>> BidaliAsync<T>(HttpMethod method, string path, string body,
            bool ezAurkituNull = false)
        {
            try
            {
                using (var request = new HttpRequestMessage(method, path))
                {
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    using (var response = await client.SendAsync(request))
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        ApiErantzuna<T> result = null;
                        try
                        {
                            result = json.Deserialize<ApiErantzuna<T>>(content);
                        }
                        catch (ArgumentException)
                        {
                            // HTML edo beste eduki bat badator, errore ulergarria erakutsi.
                        }
                        catch (InvalidOperationException)
                        {
                        }

                        if (ezAurkituNull && response.StatusCode == HttpStatusCode.NotFound &&
                            result != null && result.Status == "error" && result.Type == "ez_da_aurkitu")
                            return null;

                        if (!response.IsSuccessStatusCode)
                            throw new InvalidOperationException(result == null || string.IsNullOrWhiteSpace(result.Message)
                                ? "APIak ezin izan du eragiketa egin (HTTP " + (int)response.StatusCode + ")."
                                : result.Message);

                        if (result == null ||
                            !string.Equals(result.Status, "success", StringComparison.OrdinalIgnoreCase))
                            throw new InvalidOperationException("APIaren erantzuna ez da espero zena. Berritu zerrenda egoera egiaztatzeko.");

                        return result;
                    }
                }
            }
            catch (HttpRequestException)
            {
                throw new InvalidOperationException("Ezin da APIarekin konektatu. Egiaztatu APIa martxan dagoela eta haren helbidea zuzena dela.");
            }
            catch (TaskCanceledException)
            {
                throw new InvalidOperationException("APIak ez du garaiz erantzun. Saiatu berriro; pistaren egoera aldatzen ari bazinen, berritu zerrenda lehenik.");
            }
        }

        public void Dispose()
        {
            client.Dispose();
        }

        private sealed class ApiErantzuna<T>
        {
            public string Status { get; set; }
            public string Type { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
