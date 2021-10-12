using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using InvoiceSender365_Core_2._0.TestHelper;
using iTextSharp.text;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Security;
using RestSharp;

namespace InvoiceSender365_Core_2._0
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        [Obsolete]
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
      while (!stoppingToken.IsCancellationRequested)
      {
        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //List<String> Invoices = new List<String>() { "FV1000875099", "FV1000744636", "FV1000736599", "FV1000445310", "FV1000444910", "FV1000444974" };
        List<String> Invoices = new List<String>() { "FV1000736599"};
        DateTime inicio = DateTime.Now;
        Console.WriteLine("inicio: "+ inicio);
        var tasks = Invoices.Select(async inv => { 
                  Invoice InvoiceClass = new Invoice(inv);
                  await InvoiceClass.SetInvoiceData(inv);
                  String template = await InvoiceClass.GetTemplate();
                  StringReader sr = new StringReader(template);
                  using (MemoryStream ms2 = new MemoryStream())
                  {
                    Document document = new Document(PageSize.LETTER, 5, 5, 15, 15);
                    PdfWriter writer2 = PdfWriter.GetInstance(document, ms2);
                    document.Open();
                    HtmlPipelineContext htmlContext = new HtmlPipelineContext(new CssAppliersImpl());
                    htmlContext.SetTagFactory(Tags.GetHtmlTagProcessorFactory());
                    ICSSResolver cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
                    IPipeline pipeline = new CssResolverPipeline(cssResolver, new HtmlPipeline(htmlContext, new PdfWriterPipeline(document, writer2)));
                    XMLWorker worker = new XMLWorker(pipeline, true);
                    XMLParser p = new XMLParser(true, worker, Encoding.UTF8);
                    p.Parse(stringToStream(template));

                    document.Close();
                    writer2.Close();
                    Mail mail = new Mail();
                    mail.SendMail(ms2, InvoiceClass.FechaFactura, inv, InvoiceClass.invRes.XmlString);
                  }
                });
                await Task.WhenAll(tasks);
        DateTime fin = DateTime.Now;
        Console.WriteLine("fin: "+fin);
        Console.WriteLine("Diferencia: "+ (fin - inicio).TotalSeconds );
                await Task.Delay(1000, stoppingToken);
            }
        }
    public Stream stringToStream(String txt)
    {
      var stream = new MemoryStream();
      var w = new StreamWriter(stream);
      w.Write(txt);
      w.Flush();
      stream.Position = 0;
      return stream;
    }
  }
}
