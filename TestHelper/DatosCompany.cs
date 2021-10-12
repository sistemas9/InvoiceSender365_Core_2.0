using Newtonsoft.Json;
using RestSharp;
using serviceTest.TestHelper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceSender365_Core_2._0.TestHelper
{
  class DatosCompany
  {
    public String NombreCompany { get; set; }

    public String[] DireccionCompany { get; set; }

    public String RfcCompany { get; set; }

    public String TelefonoCompany { get; set; }

    public String UrlCompany { get; set; }

    public DatosCompany()
    {
      
    }
    public async Task<Boolean> GetDataCompany()
    {
      var auth = new AuthenticationHeader();
      var token = await auth.getAuthenticationHeader();
      var client = new RestClient("https://ayt.operations.dynamics.com/Data/LegalEntities(LegalEntityId=%27atp%27)?%24expand=LegalEntityContacts");
      var request = new RestRequest(Method.GET);
      request.AddHeader("Content-Type", "application/json");
      request.AddHeader("Accept", "application/json");
      request.AddHeader("Authorization", "Bearer "+ token);
      request.AddCookie("ApplicationGatewayAffinity", "e7fb295f94cb4b5e0cd1e2a4712e4a803fc926342cc4ecca988f29125dbd4b04");
      String response = client.Execute(request).Content;

      var dataEntity = JsonConvert.DeserializeObject<dynamic>(response);
      NombreCompany = dataEntity.Name;
      RfcCompany = dataEntity.Rfc;
      TelefonoCompany = dataEntity.LegalEntityContacts[0].Locator;
      UrlCompany = "http://www.avanceytec.com.mx";
      String direccion = dataEntity.FullPrimaryAddress;
      DireccionCompany = direccion.Split("\n");
      return true;
    }

    public String BranchOfficeAddress(String InvoiceAddressCity)
    {
      String x = InvoiceAddressCity;
      String y = String.Empty;
      String branchOfficeAddress = String.Empty;
      if (x == "")
      {
        y = "";
      }
      else if (x == "CHIHUAHUA" || x == "CHIH")
      {
        y = "Chihuahua, Chih.";
      }
      else if (x == "MÉXICO" || x == "MEXICO" || x == "TLANEPANTLA DE BAZ" || x == "NAUCALPAN" || x == "EDMX")
      {
        y = "Tlalnepantla, Edo.De México.";
      }
      else if (x == "AGUASCALIENTES" || x == "AGSC")
      {
        y = "Aguascalientes, Ags.";
      }
      else if (x == "CULIACÁN" || x == "CULIACAN" || x == "CULN")
      {
        y = "Culiacán, Sinaloa";
      }
      else if (x == "DURANGO" || x == "DURN")
      {
        y = "Durango, Dgo.";
      }
      else if (x == "GUADALAJARA" || x == "GDLJ")
      {
        y = "Guadalajara, Jal.";
      }
      else if (x == "HERMOSILLO" || x == "HERM")
      {
        y = "Hermosillo, Son.";
      }
      else if (x == "JUAREZ" || x == "JURZ")
      {
        y = "Cd.Juárez, Chihuahua.";
      }
      else if (x == "LEÓN" || x == "LEON" || x == "LEON")
      {
        y = "León, Gto";
      }
      else if (x == "MEXICALI" || x == "MEXL")
      {
        y = "Mexicali, B.C.";
      }
      else if (x == "MONTERREY" || x == "MTRY")
      {
        y = "Monterrey, Nuevo León";
      }
      else if (x == "OBREGÓN" || x == "OBREGON" || x == "OBRG")
      {
        y = "Cd.Obregón, Sonora";
      }
      else if (x == "PUEBLA" || x == "PBLA")
      {
        y = "Puebla, Puebla";
      }
      else if (x == "QUERÉTARO" || x == "QUERETARO" || x == "QRTO")
      {
        y = "Querétaro, Qro.";
      }
      else if (x == "SALTILLO" || x == "SALT")
      {
        y = "Saltillo, Coah.";
      }
      else if (x == "SAN LUIS POTOSÍ" || x == "SAN LUIS POTOSI" || x == "SLPS")
      {
        y = "San Luis Potosí, San Luis Potosí";
      }
      else if (x == "TIJUANA" || x == "TJNA")
      {
        y = "Tijuana, Baja California";
      }
      else if (x == "TORREÓN" || x == "TORREON" || x == "TORN")
      {
        y = "Torreón, Coah.";
      }
      else if (x == "TUXTLA" || x == "TXLA")
      {
        y = "Tuxtla Gutiérrez, Chiapas. ";
      }
      else if (x == "VERACRUZ" || x == "VCRZ")
      {
        y = "Veracruz, Veracruz.";
      }
      else if (x == "ZACATECAS" || x == "ZACS")
      {
        y = "Zacatecas, Zacatecas.";
      }

      else
      {
        y = "";
      }

      branchOfficeAddress = y;

      return branchOfficeAddress;
    }
  }
}
