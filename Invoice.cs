using InvoiceSender365_Core_2._0.TestHelper;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InvoiceSender365_Core_2._0
{
  class Invoice
  {
    public InvoiceResults invRes;
    public String htmlLineas;
    public dynamic TimbreFiscalDigital;
    public dynamic Comprobante;
    public String SelloDigitalCFDI;
    public String SelloDigitalSAT;
    public String CadenaOriginal;
    public String NumCertificadoSat;
    public String RfcPAC;
    public String FechaTimbrado;
    public String FechaTimbradoPagare;
    public String NumCertificado;
    public String FormaPago;
    public String MetodoPago;
    public String TipoDeComprobante;
    public String montoPagare;
    public String ReferenciaCliente;
    public String moneda;
    public String ExchangeRate;
    public String pagareMontoLetras;
    public String FechaExpiracion;
    public String FechaFactura;
    public String Plazo;
    public String[] DireccionCliente;
    public String RfcCliente;
    public String CalleCliente;
    public String ColoniaCliente;
    public String CiudadCliente;
    public String ZipCodeCliente;
    public String TelCliente;
    public String DeliveryModeCode;
    public String UsoCFDI;
    public String ClienteCompleto;
    public String DeudorNombre;
    public String template;
    public String Total;
    public String Vendedor;
    public DatosCompany company;
    public String qrData;
    public String qrCodeHtml;
    public String PathImgFirma;

    public Invoice(String InvoiceString)
    {
    }
    public async Task<Boolean> SetInvoiceData(String InvoiceString)
    {
      this.invRes = await GetInvoiceData(InvoiceString);
      this.htmlLineas = GetHtmlLineasFactura(invRes.SalesInvoiceLines, invRes.XML, invRes.SalesInvoiceHeaders, invRes.SalesOrderLines);
      this.TimbreFiscalDigital = invRes.XML.GetElementsByTagName("tfd:TimbreFiscalDigital")[0];
      this.Comprobante = invRes.XML.GetElementsByTagName("cfdi:Comprobante")[0];
      this.SelloDigitalCFDI = TimbreFiscalDigital.Attributes["SelloCFD"].Value;
      this.SelloDigitalSAT = TimbreFiscalDigital.Attributes["SelloSAT"].Value;
      this.CadenaOriginal = "||" + TimbreFiscalDigital.Attributes["Version"].Value + "|" + TimbreFiscalDigital.Attributes["UUID"].Value + "|" +
                              TimbreFiscalDigital.Attributes["FechaTimbrado"].Value + "|" + TimbreFiscalDigital.Attributes["SelloCFD"].Value + "|" +
                              TimbreFiscalDigital.Attributes["NoCertificadoSAT"].Value + "||";
      this.NumCertificadoSat = TimbreFiscalDigital.Attributes["NoCertificadoSAT"].Value;
      this.RfcPAC = TimbreFiscalDigital.Attributes["RfcProvCertif"].Value;
      this.FechaTimbrado = TimbreFiscalDigital.Attributes["FechaTimbrado"].Value;
      this.FechaTimbradoPagare = Convert.ToDateTime(TimbreFiscalDigital.Attributes["FechaTimbrado"].Value).ToString("dd-MM-yyyy");
      this.NumCertificado = Comprobante.Attributes["NoCertificado"].Value;
      this.FormaPago = Comprobante.Attributes["FormaPago"].Value;
      this.MetodoPago = Comprobante.Attributes["MetodoPago"].Value;
      this.TipoDeComprobante = Comprobante.Attributes["TipoDeComprobante"].Value;
      this.montoPagare = (String)invRes.SalesInvoiceHeaders.value[0].TotalInvoiceAmount;
      this.ReferenciaCliente = (String)invRes.SalesInvoiceHeaders.value[0].CustomersOrderReference;
      this.moneda = (String)invRes.SalesInvoiceHeaders.value[0].CurrencyCode;
      this.ExchangeRate = (String)invRes.ExchangeRates.value[0].Rate;
      this.pagareMontoLetras = NumberToWords.ConvertToWords(montoPagare, moneda);
      this.FechaExpiracion = Convert.ToDateTime(invRes.AYT_CustInvoiceV2Jour.value[0].DueDate.Value).ToString("dd/MM/yyyy");
      this.FechaFactura = Convert.ToDateTime(FechaTimbrado).ToString("dd/MM/yyyy");
      this.Plazo = invRes.SalesOrderHeadersV2.value[0].PaymentTermsName.Value;
      this.DireccionCliente = invRes.CustomersV3.value[0].FullPrimaryAddress.Value.Split("\n");
      this.RfcCliente = invRes.CustomersV3.value[0].RFCNumber.Value;
      this.CalleCliente = DireccionCliente[0];
      this.ColoniaCliente = DireccionCliente[1];
      this.CiudadCliente = DireccionCliente[2];
      this.ZipCodeCliente = DireccionCliente[3];
      this.TelCliente = invRes.CustomersV3.value[0].PrimaryContactPhone.Value;
      this.DeliveryModeCode = invRes.SalesInvoiceHeaders.value[0].DeliveryModeCode.Value;
      this.UsoCFDI = Comprobante["cfdi:Receptor"].Attributes["UsoCFDI"].Value;
      this.ClienteCompleto = invRes.CustomersV3.value[0].CustomerAccount.Value + " - " + invRes.CustomersV3.value[0].OrganizationName.Value;
      this.DeudorNombre = invRes.CustomersV3.value[0].OrganizationName.Value;
      this.template = File.ReadAllText("InvoiceTemplate.html").ToString();
      this.Total = invRes.SalesInvoiceHeaders.value[0].TotalInvoiceAmount.Value.ToString();
      this.Vendedor = invRes.Workers.value[0].Name.Value;
      this.company = new DatosCompany();
      await company.GetDataCompany();
      this.qrData = GetQRBarCodeData(invRes, company.RfcCompany, RfcCliente, Total);
      this.PathImgFirma = "http://inax.aytcloud.com/Facturacion365/filesEntregas/"+invRes.SalesOrderHeadersV2.value[0].SalesOrderNumber+".jpg";
      this.qrCodeHtml = getHtmlCodeForQR(qrData);
      this.template = this.template.Replace("{NombreCompany}", company.NombreCompany);
      this.template = this.template.Replace("{RfcCompany}", company.RfcCompany);
      this.template = this.template.Replace("{DireccionCompany0}", company.DireccionCompany[0]);
      this.template = this.template.Replace("{DireccionCompany1}", company.DireccionCompany[1]);
      this.template = this.template.Replace("{DireccionCompany2}", company.DireccionCompany[2]);
      this.template = this.template.Replace("{TelefonoCompany}", company.TelefonoCompany);
      this.template = this.template.Replace("{UrlCompany}", company.UrlCompany);
      this.template = this.template.Replace("{TablaArticulos}", htmlLineas);
      this.template = this.template.Replace("{SelloDigitalCFDI}", long2tiny(SelloDigitalCFDI,120));
      this.template = this.template.Replace("{SelloDigitalSAT}", long2tiny(SelloDigitalSAT,120));
      this.template = this.template.Replace("{CadenaOriginal}", long2tiny(CadenaOriginal, 120));
      this.template = this.template.Replace("{NumCertificado}", NumCertificado);
      this.template = this.template.Replace("{NumCertificadoSat}", NumCertificadoSat);
      this.template = this.template.Replace("{RfcPAC}", RfcPAC);
      this.template = this.template.Replace("{FechaTimbrado}", FechaTimbrado);
      this.template = this.template.Replace("{BuenoPor}", String.Format("{0:C}", Convert.ToDouble(invRes.SalesInvoiceHeaders.value[0].TotalInvoiceAmount.Value)));
      this.template = this.template.Replace("{PagareFecha}", FechaTimbradoPagare);
      this.template = this.template.Replace("{PagareMonto}", String.Format("{0:C}", Convert.ToDouble(invRes.SalesInvoiceHeaders.value[0].TotalInvoiceAmount.Value)));
      this.template = this.template.Replace("{PagareMontoLetras}", pagareMontoLetras);
      this.template = this.template.Replace("{FechaExpiracion}", FechaExpiracion);
      this.template = this.template.Replace("{FechaFactura}", FechaFactura);
      this.template = this.template.Replace("{Plazo}", Plazo);
      this.template = this.template.Replace("{BranchOfficeAddres}", company.BranchOfficeAddress(invRes.SalesOrderLines.value[0].ShippingSiteId.Value));
      this.template = this.template.Replace("{UUID}", TimbreFiscalDigital.Attributes["UUID"].Value);
      this.template = this.template.Replace("{Factura}", InvoiceString);
      this.template = this.template.Replace("{ClienteCompleto}", ClienteCompleto);
      this.template = this.template.Replace("{CalleCliente}", CalleCliente);
      this.template = this.template.Replace("{ColoniaCliente}", ColoniaCliente);
      this.template = this.template.Replace("{CiudadCliente}", CiudadCliente);
      this.template = this.template.Replace("{ZipCodeCliente}", ZipCodeCliente);
      this.template = this.template.Replace("{RfcCliente}", RfcCliente);
      this.template = this.template.Replace("{TelCliente}", TelCliente);
      this.template = this.template.Replace("{MetodoPago}", MetodoPago);
      this.template = this.template.Replace("{FormaPago}", FormaPago);
      this.template = this.template.Replace("{TipoDeComprobante}", TipoDeComprobante);
      this.template = this.template.Replace("{DeliveryModeCode}", DeliveryModeCode);
      this.template = this.template.Replace("{UsoCFDI}", UsoCFDI);
      this.template = this.template.Replace("{ReferenciaCliente}", ReferenciaCliente);
      this.template = this.template.Replace("{DeudorNombre}", DeudorNombre);
      this.template = this.template.Replace("{RawData}", qrCodeHtml);
      this.template = this.template.Replace("{Vendedor}", Vendedor);
      this.template = this.template.Replace("{ExchangeRate}", String.Format("{0:C}", Convert.ToDouble(ExchangeRate)));
      if (RemoteFileExistsUsingClient(this.PathImgFirma))
      {
        this.template = this.template.Replace("{PathImgFirma}",this.PathImgFirma);
      }
      else
      {
        this.template = this.template.Replace("{PathImgFirma}", " ");
      }
      return true;
    }
    private bool RemoteFileExistsUsingClient(String url)
    {
      bool result = false;
      using (WebClient client = new WebClient())
      {
        try
        {
          Stream stream = client.OpenRead(url);
          if (stream != null)
          {
            result = true;
          }
          else
          {
            result = false;
          }
        }
        catch
        {
          result = false;
        }
      }
      return result;
    }
    public async Task<String> GetTemplate()
    {
      return this.template;
    }
    public String long2tiny(String word, int cutLong)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < word.Length; i++)
      {
        if (i % cutLong == 0 && i != 0)
        {
          sb.Append("<br/>");
        }
        sb.Append(word[i]);
      }

      return sb.ToString();
    }
    public String getHtmlCodeForQR(String Data)
    {
      var client = new RestClient("http://inax.aytcloud.com/Facturacion365/QRCodeGenerateHtml.php");
      var request = new RestRequest(Method.POST);
      request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
      request.AddHeader("Accept", "application/json");
      request.AddParameter("application/x-www-form-urlencoded", "data=" + Data, ParameterType.RequestBody);
      String response = client.Execute(request).Content;

      return response;
    }
    public async Task<InvoiceResults> GetInvoiceData(String invoiceNumber)
    {
      ConsultaEntity entity = new ConsultaEntity();
      InvoiceResults invoiceResults = new InvoiceResults();
      List<String> urlsInvoice = new List<String>()
          {
            "https://ayt.operations.dynamics.com/Data/SalesInvoiceHeaders?$filter=InvoiceNumber%20eq%20'"+invoiceNumber+"'",
            "https://ayt.operations.dynamics.com/Data/AYT_CustInvoiceV2Jour?$filter=InvoiceId%20eq%20'"+invoiceNumber+"'",
            "https://ayt.operations.dynamics.com/Data/SalesInvoiceLines?$filter=InvoiceNumber%20eq%20'"+invoiceNumber+"'",
          };

      Parallel.ForEach(urlsInvoice, (url) =>
      {
        var result = entity.QueryEntity(url);
        var resultObj = JsonConvert.DeserializeObject<EntityModel>(result.Result);

        if (url.Contains("SalesInvoiceHeaders"))
        {
          invoiceResults.SalesInvoiceHeaders = resultObj;
        }
        else if (url.Contains("AYT_CustInvoiceV2Jour"))
        {
          invoiceResults.AYT_CustInvoiceV2Jour = resultObj;
        }
        else if (url.Contains("SalesInvoiceLines"))
        {
          invoiceResults.SalesInvoiceLines = resultObj;
        }
      });

      String urlXML = "https://ayt.operations.dynamics.com/api/services/STF_INAX/STF_XMLDoc/getXMLDoc";
      DateTime fechaFactura = Convert.ToDateTime(invoiceResults.SalesInvoiceHeaders.value[0].InvoiceDate);
      String[] paramts = new string[] { invoiceNumber, fechaFactura.ToString("MM/dd/yyyy"), invoiceResults.SalesInvoiceHeaders.value[0].InvoiceCustomerAccountNumber, "atp" };
      var resultXML = entity.QueryWebService(urlXML, paramts);
      String xmlTemp = JsonConvert.DeserializeObject<String>(resultXML.Result);
      byte[] utfBytes = Encoding.UTF8.GetBytes(xmlTemp);
      xmlTemp = Encoding.UTF8.GetString(utfBytes);
      invoiceResults.XmlString = xmlTemp;
      invoiceResults.XML = new XmlDocument();
      invoiceResults.XML.LoadXml(JsonConvert.DeserializeObject<String>(resultXML.Result));

      List<String> urlsSalesOrder = new List<string>()
          {
            "https://ayt.operations.dynamics.com/Data/SalesOrderLines?$filter=SalesOrderNumber%20eq%20'" + invoiceResults.SalesInvoiceHeaders.value[0].SalesOrderNumber +"'",
            "https://ayt.operations.dynamics.com/Data/SalesOrderHeadersV2?$filter=SalesOrderNumber%20eq%20'" + invoiceResults.SalesInvoiceHeaders.value[0].SalesOrderNumber + "'",
            "https://ayt.operations.dynamics.com/Data/CustomersV3?$filter=CustomerAccount%20eq%20'" + invoiceResults.SalesInvoiceHeaders.value[0].InvoiceCustomerAccountNumber + "'",
            "https://ayt.operations.dynamics.com/Data/Workers?$filter=PersonnelNumber%20eq%20'" + invoiceResults.SalesInvoiceHeaders.value[0].SalesOrderResponsiblePersonnelNumber + "'",
            "https://ayt.operations.dynamics.com/Data/ExchangeRates?$filter=RateTypeName%20eq%20'ATP'%20and%20StartDate%20eq%20"+Convert.ToDateTime(invoiceResults.SalesInvoiceHeaders.value[0].InvoiceDate).ToString("yyyy-MM-dd")+"T12%3A00%3A00Z&$orderby=StartDate%20desc&$top=1",
          };

      Parallel.ForEach(urlsSalesOrder, (urlSO) =>
      {
        ConsultaEntity entity = new ConsultaEntity();
        var result = entity.QueryEntity(urlSO);
        var resultObj = JsonConvert.DeserializeObject<EntityModel>(result.Result);

        if (urlSO.Contains("SalesOrderLines"))
        {
          invoiceResults.SalesOrderLines = resultObj;
        }
        else if (urlSO.Contains("SalesOrderHeadersV2"))
        {
          invoiceResults.SalesOrderHeadersV2 = resultObj;
        }
        else if (urlSO.Contains("CustomersV3"))
        {
          invoiceResults.CustomersV3 = resultObj;
        }
        else if (urlSO.Contains("Workers"))
        {
          invoiceResults.Workers = resultObj;
        }
        else if (urlSO.Contains("ExchangeRates"))
        {
          invoiceResults.ExchangeRates = resultObj;
        }
      });

      return invoiceResults;
    }
    public String GetProductSerialNumbers(String productNumber, String invoiceNumber)
    {
      ConsultaEntity entity = new ConsultaEntity();
      String serialNumber = String.Empty;
      String urlInventTrans = "https://ayt.operations.dynamics.com/Data/AYT_InventTrans?$filter=ItemId%20eq%20'" + productNumber + "'%20and%20InvoiceId%20eq%20'" + invoiceNumber + "'";
      String result = entity.QueryEntity(urlInventTrans).Result;
      var resultObj = JsonConvert.DeserializeObject<EntityModel>(result);
      foreach (var inventTrans in resultObj.value)
      {
        String urlInventDims = "https://ayt.operations.dynamics.com/Data/AYT_InventDims?%24filter=inventDimId%20eq%20'" + inventTrans.InventDimId.ToString().Replace("#", "%23%") + "'";
        String resultInvDim = entity.QueryEntity(urlInventDims).Result;
        var resultObjInvDim = JsonConvert.DeserializeObject<EntityModel>(resultInvDim);
        serialNumber += (resultObjInvDim.value.Count > 0) ? resultObjInvDim.value[0].inventSerialId : "";
      }
      return serialNumber;
    }
    public String GetHtmlLineasFactura(EntityModel InvoiceLines, XmlDocument XML, EntityModel InvoiceHeaders, EntityModel SalesOrderLines)
    {
      String html = "<table style=\"font-size:10px; width: 100%;\" cellspacing=\"0\" border=\"0\">" +
                        "<tr>" +
                              "<td style=\"background-color: Black; height: 1px;\" colspan=\"9\"></td>" +
                      "</tr>" +
                      "<tr style=\"background-color: #d3d3d3\">" +
                        "<th style=\"text-align:left; border-top:solid 2px black;border-bottom:solid 1px black; width:80px;\">Artículo</th>" +
                        "<th style=\"text-align:left; border-top:solid 2px black;border-bottom:solid 1px black;\"> Clave SAT</th>" +
                        "<th style=\"text-align:left; border-top:solid 2px black;border-bottom:solid 1px black;\"> Cant.</th >" +
                        "<th style=\"text-align:left; border-top:solid 2px black;border-bottom:solid 1px black;\">U.</th>" +
                        "<th style=\"text-align:left; border-top:solid 2px black;border-bottom:solid 1px black;\"> U.SAT </th>" +
                        "<th style=\"text-align:left; border-top:solid 2px black;border-bottom:solid 1px black; width:260px;\">Descripción</th>" +
                        "<th style=\"text-align:right; border-top:solid 2px black;border-bottom:solid 1px black;\"> Precio U.</th>" +
                        "<th style=\"text-align:right; border-top:solid 2px black;border-bottom:solid 1px black;\"> Importe </th >" +
                        "<th style=\"text-align:right; border-top:solid 2px black;border-bottom:solid 1px black; width:60px;\">Desc. &nbsp;</th>" +
                      "</tr>" +
                      "<tr>" +
                        "<td style=\"background-color: Black; height: 1px;\" colspan=\"9\"></td>" +
                      "</tr>";
      String cantidadIva = "16";
      foreach (var invoiceLine in InvoiceLines.value)
      {
        var list = XML.GetElementsByTagName("cfdi:Concepto");
        var existe = list.Cast<XmlNode>().Where(node => node.Attributes["Descripcion"].Value == (String)invoiceLine.ProductDescription).ToList();
        var impuestos = existe.Cast<XmlNode>().Select(node => node.ChildNodes[0].ChildNodes[0].ChildNodes[0]).ToList();
        String pid = String.Empty;
        String LineDiscountAmount = String.Empty;
        if (existe.Count > 0)
        {
          pid = (String)invoiceLine.ProductNumber;
          if (SalesOrderLines.value[0].SalesTaxGroupCode.Value == "FRONT")
          {
            cantidadIva = "8";
          }
        }
        String serialNumber = GetProductSerialNumbers(pid, (String)invoiceLine.InvoiceNumber);
        LineDiscountAmount = existe[0].Attributes["Descuento"] != null ? String.Format("{0:C}", Convert.ToDouble(existe[0].Attributes["Descuento"].Value)) : "-";
        if (existe[0].Attributes["Descuento"] != null)
        {
          if (existe[0].Attributes["Descuento"].Value == "")
          {
            LineDiscountAmount = "-";
          }
        }
        html += "<tr style=\"vertical-align:top;\">" +
                    "<td style=\"padding-top:3px !important;\"> " + pid + "</td>" +
                    "<td style=\"padding-top:3px !important;\"> " + existe[0].Attributes["ClaveProdServ"].Value + " </td>" +
                    "<td style=\"padding-top:3px !important;\"> " + existe[0].Attributes["Cantidad"].Value + " </td>" +
                    "<td style=\"padding-top:3px !important;\"> " + existe[0].Attributes["Unidad"].Value + " </td>" +
                    "<td style=\"padding-top:3px !important;\"> " + existe[0].Attributes["ClaveUnidad"].Value + " </td>" +
                    "<td style=\"padding-top:3px !important;\"><small style=\"font-size:8px;\"> " + existe[0].Attributes["Descripcion"].Value + " </small></td>" +
                    "<td style=\"padding-top:3px !important;text-align:right;\"> " + String.Format("{0:C}", Convert.ToDouble(existe[0].Attributes["ValorUnitario"].Value)) + " </td>" +
                    "<td style=\"padding-top:3px !important;text-align:right;\"> " + String.Format("{0:C}", Convert.ToDouble(existe[0].Attributes["Importe"].Value)) + " </td>" +
                    "<td style=\"padding-top:3px !important;text-align:right;\"> " + LineDiscountAmount + " </td>" +
                  "</tr>" +
                "<tr style=\"vertical-align:top;\">" +
                    "<td colspan=\"9\" style = \"font-style:italic; font-weight:normal; font-size:9:px; letter-spacing:1px; border-bottom: 1px solid #eee;\" >" +
                    "<i>Base:" + impuestos[0].Attributes["Base"].Value + "," +
                    "Impuesto: " + impuestos[0].Attributes["Impuesto"].Value + "," +
                    "TipoFactor: " + impuestos[0].Attributes["TipoFactor"].Value + "," +
                    "TasaOCuota: " + impuestos[0].Attributes["TasaOCuota"].Value + "," +
                    "Importe: " + impuestos[0].Attributes["Importe"].Value + "," +
                    serialNumber + "</i><br/><br/>" +
                    "</td>" +
                "</tr>";
      }
      var list2 = XML.GetElementsByTagName("cfdi:Comprobante");
      String totalInvoiceAmount = InvoiceHeaders.value[0].TotalInvoiceAmount;
      String moneda = InvoiceHeaders.value[0].CurrencyCode;
      String descuentoTotal = "-";
      if (Convert.ToDouble(InvoiceHeaders.value[0].TotalDiscountAmount) > 0)
      {
        descuentoTotal = "(" + String.Format("{0:C}", InvoiceHeaders.value[0].TotalDiscountAmount) + ")";
      }
      html += "<tr>";
      html += "  <td colspan=\"7\" style=\"background-color: #d3d3d3; padding-top:3px;\">";
      html += "  Son: ***" + NumberToWords.ConvertToWords(totalInvoiceAmount, moneda) + "***";
      html += "  </td>";
      html += "  <th style=\"text-align: right; padding-top:3px;\"> Subtotal &nbsp; </th>";
      html += "  <td style=\"text-align: right; padding-top:3px;\"> " + String.Format("{0:C}", list2[0].Attributes["SubTotal"].Value) + " </td>";
      html += "</tr>";
      html += "<tr>";
      html += "  <td colspan=\"7\"></td> ";
      html += "  <th style=\"text-align: right; padding-top:3px;\"> Descuento &nbsp; </th>";
      html += "  <td style=\"text-align: right; padding-top:3px;\"> " + descuentoTotal + " </td>";
      html += "</tr>";
      html += "<tr>";
      html += "  <td colspan=\"7\"></td> ";
      html += "  <th style=\"text-align: right; padding-top:3px;\"> I.V.A. " + cantidadIva + " %&nbsp; </th>";
      html += "  <td style=\"text-align: right; padding-top:3px;\" > " + String.Format("{0:C}", InvoiceHeaders.value[0].TotalTaxAmount) + " </td>";
      html += "</tr>";
      html += "<tr>";
      html += "  <td colspan=\"7\"></td> ";
      html += "  <th style=\"text-align: right; padding-top:3px;\"> TOTAL &nbsp; </th>";
      html += "  <td style=\"text-align: right; padding-top:3px;\"> " + String.Format("{0:C}", InvoiceHeaders.value[0].TotalInvoiceAmount) + " </td>";
      html += "</tr>";
      html += "</table>";
      return html;
    }
    public String GetQRBarCodeData(InvoiceResults invRes, String RfcCompany, String RfcCliente, String Total)
    {
      var TimbreFiscalDigital = invRes.XML.GetElementsByTagName("tfd:TimbreFiscalDigital")[0];
      var Comprobante = invRes.XML.GetElementsByTagName("cfdi:Comprobante")[0];
      String Version = Comprobante.Attributes["Version"].Value;
      String QRData = String.Empty;
      String UUID = TimbreFiscalDigital.Attributes["UUID"].Value;
      String SelloCFD = TimbreFiscalDigital.Attributes["SelloCFD"].Value;
      SelloCFD = SelloCFD.Substring(SelloCFD.Length - 8);
      if (Version == "3.3")
      {
        QRData = "https://verificacfdi.facturaelectronica.sat.gob.mx/default.aspx" +
            "?id=" + UUID +
            "%26re=" + RfcCompany +
            "%26rr=" + RfcCliente.Trim() +
            "%26tt=" + Total +
            "%26fe=" + SelloCFD;
      }
      else
      {
        QRData = "?re=" + RfcCompany +
            "%26rr=" + RfcCliente.Trim() +
            "%26tt=" + UUID +
            "%26id=NO DEFINIDO";
      }

      return QRData;
    }
    public class EntityModel
    {
      public dynamic value { get; set; }
    }
    public class InvoiceResults
    {
      public EntityModel SalesInvoiceHeaders { get; set; }
      public EntityModel AYT_CustInvoiceV2Jour { get; set; }
      public EntityModel SalesInvoiceLines { get; set; }
      public EntityModel ExchangeRates { get; set; }
      public EntityModel SalesOrderLines { get; set; }
      public EntityModel SalesOrderHeadersV2 { get; set; }
      public EntityModel CustomersV3 { get; set; }
      public EntityModel Workers { get; set; }
      public XmlDocument XML { get; set; }
      public String XmlString { get; set; }
    }
  }
}
