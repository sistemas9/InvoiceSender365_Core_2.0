using Newtonsoft.Json;
using RestSharp;
using serviceTest.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSender365_Core_2._0.TestHelper
{
  class ConsultaEntity
  {

    public async Task<String> QueryEntity(String url)
    {
      var auth = new AuthenticationHeader();
      var token = await auth.getAuthenticationHeader();
      var client = new RestClient(url);
      var request = new RestRequest(Method.GET);
      request.AddHeader("Content-Type", "application/json");
      request.AddHeader("Accept", "application/json");
      request.AddHeader("Authorization", "Bearer " + token);
      request.AddCookie("ApplicationGatewayAffinity", "e7fb295f94cb4b5e0cd1e2a4712e4a803fc926342cc4ecca988f29125dbd4b04");
      String response = client.Execute(request).Content;

      return response;
    }

    public async Task<String> QueryWebService(String url, String[] parametros)
    {
      var auth = new AuthenticationHeader();
      var token = await auth.getAuthenticationHeader();
      var client = new RestClient(url);
      var request = new RestRequest(Method.POST);
      request.AddHeader("Content-Type", "application/json");
      request.AddHeader("Accept", "application/json");
      request.AddHeader("Authorization", "Bearer "+token);
      request.AddCookie("ApplicationGatewayAffinity", "e7fb295f94cb4b5e0cd1e2a4712e4a803fc926342cc4ecca988f29125dbd4b04");
      if (parametros.Count() > 0)
      {
        request.AddParameter("application/json", "{\n\"invoiceId\":\"" + parametros[0] + "\",\n\t\"transDate\":\"" + parametros[1] + "\",\n\t\"custInvoiceAccount\":\"" + parametros[2] + "\",\n\t\"company\":\"" + parametros[3] + "\",\n}", ParameterType.RequestBody);
      }
      String response = client.Execute(request).Content;

      return response;
    }
  }
}
