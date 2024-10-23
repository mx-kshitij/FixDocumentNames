using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;

namespace FixDocumentNames
{
    public class FixDocumentNamesDockablePaneViewModel : WebViewDockablePaneViewModel
    {
        private readonly Uri _baseUri;
        private readonly Func<IModel?> _getCurrentApp;
        private readonly ILogService _logService;
        private readonly IBackgroundJobService bgService;
        private readonly IMessageBoxService msgService;

        public FixDocumentNamesDockablePaneViewModel(Uri baseUri, Func<IModel?> getCurrentApp, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService)
        {
            _baseUri = baseUri;
            _getCurrentApp = getCurrentApp;
            _logService = logService;
            this.bgService = bgService;
            this.msgService = msgService;
        }
        public override void InitWebView(IWebView webView)
        {

            webView.Address = new Uri(_baseUri, "index");

            webView.MessageReceived += (_, args) =>
            {
                var currentApp = _getCurrentApp();
                if (currentApp == null) return;

                if (args.Message == "FixDocuments") 
                {
                    new DocumentItemListHandler(currentApp, _logService, bgService, msgService).FixDocumentList(args.Data.ToJsonString());
                    try
                    {
                        webView.Address = new Uri(_baseUri, "index?searchKey=" + args.Data["searchKey"]);
                    }
                    catch (Exception ex)
                    {
                        _logService.Error("Error occurred", ex);
                    }
                }
            };
        }
    }
}
