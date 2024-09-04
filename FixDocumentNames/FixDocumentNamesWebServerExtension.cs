using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebServer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FixDocumentNames
{
    [Export(typeof(WebServerExtension))]

    public class FixDocumentNamesWebServerExtension : WebServerExtension
    {
        private readonly IExtensionFileService _extensionFileService;
        private readonly ILogService _logService;

        [ImportingConstructor]
        public FixDocumentNamesWebServerExtension(IExtensionFileService extensionFileService, ILogService logService)
        {
            _extensionFileService = extensionFileService;
            _logService = logService;
        }

        public override void InitializeWebServer(IWebServer webServer)
        {
            webServer.AddRoute("index", ServeIndex);
            webServer.AddRoute("main.js", ServeMainJs);
            webServer.AddRoute("documents", ServeDocuments);
            webServer.AddRoute("theme", ServeTheme);
            webServer.AddRoute("fixDocuments", FixDocuments);
        }

        private async Task ServeIndex(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            var indexFilePath = _extensionFileService.ResolvePath("wwwroot", "index.html");
            await response.SendFileAndClose("text/html", indexFilePath, ct);
        }

        private async Task ServeMainJs(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            var indexFilePath = _extensionFileService.ResolvePath("wwwroot", "main.js");
            await response.SendFileAndClose("text/javascript", indexFilePath, ct);
        }

        private async Task ServeDocuments(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            if (CurrentApp == null)
            {
                response.SendNoBodyAndClose(404);
                return;
            }

            var searchKey = request.QueryString["searchKey"] ?? "";

            var documentList = new DocumentItemListHandler(CurrentApp, _logService).LoadDocumentList(searchKey);
            var jsonStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(jsonStream, documentList, cancellationToken: ct);

            response.SendJsonAndClose(jsonStream);
        }

        private async Task ServeTheme(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            var theme = Configuration.Theme.ToString();
            var jsonStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(jsonStream, theme, cancellationToken: ct);
            response.SendJsonAndClose(jsonStream);
        }

        private async Task FixDocuments(HttpListenerRequest request, HttpListenerResponse response, CancellationToken ct)
        {
            if (CurrentApp == null)
            {
                response.SendNoBodyAndClose(404);
                return;
            }

            if (!request.HasEntityBody)
            {
                response.SendNoBodyAndClose(400);
                return;
            }

            System.IO.Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

            string requestBody = "";

            try
            {
                requestBody = reader.ReadToEnd();
                var jsonStream = new MemoryStream();
                await JsonSerializer.SerializeAsync(jsonStream, "", cancellationToken: ct);

                response.SendJsonAndClose(jsonStream);
            }
            catch (Exception ex)
            {
                _logService.Error("Error parsing ", ex);
            }

            new DocumentItemListHandler(CurrentApp, _logService).FixDocumentList(requestBody);

            body.Close();
            reader.Close();


            
        }
    }
}
