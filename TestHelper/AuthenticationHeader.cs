using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace serviceTest.TestHelper
{
    public class AuthenticationHeader
    {
        //private static string ODataEntityPath = "https://tes-ayt.sandbox.operations.dynamics.com/Data/";
        //private static Uri oDataUri = new Uri(ODataEntityPath, UriKind.Absolute);
        //private static Resources context = new Resources(oDataUri);
        public static String clientId;
        public static String clientSecretId;
        public static String urlBase;

        public async Task<String> getAuthenticationHeader()
        {
          var AppSettings = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

          String ambiente = AppSettings["Config"];

          String token = await GetToken(ambiente);
          return token;
        }

        public async Task<String> GetToken(String ambiente)
        {
          String url = String.Empty;   
          if (ambiente == "DESARROLLO")
          {
            url = "https://solutiontinaxdev.azurewebsites.net/SolutionToken/api/SolutionToken";
          }
          else
          {
            url = "https://solutiontinax-solutiontokeninaxpr.azurewebsites.net/SolutionToken/api/SolutionToken";
          }
          /////////////////////////////////////generar token/////////////////////////////////////////////////////////////////////////////
          token authenticationHeader = new token();
          System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
          HttpClient token = new HttpClient();
          HttpResponseMessage response = await token.GetAsync(url);
          response.EnsureSuccessStatusCode();
          String responseBody = await response.Content.ReadAsStringAsync();
          responseBody = responseBody.Substring(1, responseBody.Length - 2);
          authenticationHeader = JsonConvert.DeserializeObject<token>(responseBody);
          ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
          return authenticationHeader.Token;
    }
  }

    public class token
    {
      public String Token { get; set; }
    }
}