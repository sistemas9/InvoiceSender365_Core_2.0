using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace InvoiceSender365_Core_2._0.TestHelper
{
  class Mail
  {
    private SmtpClient client = new SmtpClient();
    private NetworkCredential Credential = new NetworkCredential("cfdi@facturasavance.com.mx", "@Soport3");
    private MailMessage mail = new MailMessage();

    public Mail()
    {
      client.Port = 587;
      client.DeliveryMethod = SmtpDeliveryMethod.Network;
      client.UseDefaultCredentials = false;
      client.Host = "smtp.office365.com";
      client.EnableSsl = true;
      client.Credentials = Credential;
    }

    public Boolean SendMail(MemoryStream ms2, String fecha, String Invoice, String xml)
    {
      String body = String.Empty;
      String company = "Avance y Tecnologia en Plasticos S.A. de C.V.";
      try
      {
        MemoryStream memoStream = new MemoryStream();
        memoStream.SetLength(0);
        var stream2 = new StreamWriter(memoStream, Encoding.UTF8);
        stream2.Write(xml);//*******archivo xml************///
        stream2.Flush();
        memoStream.Position = 0;
        memoStream.Flush();
        DateTime dateFact = DateTime.Parse(fecha);
        body = "<html><head></head>";
        body += "<body><span style=\"font-size: 12px\"><font face=\"Consolas\">&nbsp; ";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">Estimado Cliente: </font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">&nbsp;</font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\"></font></span>&nbsp;</p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">Usted está<span style=\"mso-spacerun: yes\">&nbsp; </span>recibiendo un comprobante fiscal digital (Factura Electrónica) de " + company + "</font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">&nbsp;</font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">Dicha factura se entrega en archivo XML conforme lo marcan las disposiciones fiscales y usted podrá<span style=\"mso-spacerun: yes\">&nbsp; </span>visualizarlo en PDF e imprimirlo libremente para incluirlo en su contabilidad y/o resguardar la impresión y archivo XML </font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">&nbsp;</font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">Si usted desea que llegue el comprobante fiscal a otro correo electrónico distinto a aquel en el que estamos notificándole hasta hoy, por favor escriba su petición al siguiente correo electrónico: <B style=\"mso-bidi-font-weight: normal\"><u><a href=\"mailto:cuentafacturacion@avanceytec.com.mx\">cuentafacturacion@avanceytec.com.mx</a></u></b></font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><span style=\"mso-spacerun: yes\"><font face=\"Calibri\"></font></span></span>&nbsp;</p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><span style=\"mso-spacerun: yes\"><font face=\"Calibri\"></font></span></span>&nbsp;</p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"></span><font face=\"Calibri\">&nbsp;</font><span style=\"font-size: 12pt\"><font face=\"Calibri\">________________________________________</font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">Saludos cordiales. </font></span></p>";
        body += "<p class=\"MsoPlainText\" style=\"margin: 0cm 0cm 0pt\"><span style=\"font-size: 12px\"><font face=\"Calibri\">" + company + "</font></span></p></font></span></body></html>";
        mail.Subject = "Envío de Factura. Fecha de Factura: " + String.Format("{0:dd/MM/yyyy}", dateFact);
        mail.Body = body;
        mail.IsBodyHtml = true;
        System.Net.Mime.ContentType ct = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Pdf);
        System.Net.Mime.ContentType ctxml = new System.Net.Mime.ContentType(System.Net.Mime.MediaTypeNames.Application.Xml);
        System.Net.Mail.Attachment attach = new System.Net.Mail.Attachment(new MemoryStream(ms2.ToArray()), ct);
        System.Net.Mail.Attachment attach2 = new System.Net.Mail.Attachment(new MemoryStream(memoStream.ToArray()), ctxml);
        attach.ContentDisposition.FileName = Invoice + ".pdf";
        attach2.ContentDisposition.FileName = Invoice + ".xml";

        mail.From = new MailAddress("cfdi@facturasavance.com.mx", "Avance Facturación");
        mail.To.Add("larmando.armendariz@gmail.com");
        mail.Attachments.Add(attach);
        mail.Attachments.Add(attach2);
        client.Send(mail);
        mail.Attachments.Clear();
        return true;
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        return false;
      }
    }
  }
}
