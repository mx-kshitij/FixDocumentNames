using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;
using Mendix.StudioPro.ExtensionsAPI.UI.WebView;

namespace FixDocumentNames
{
    public class FixDocumentNamesDockablePaneViewModel : WebViewDockablePaneViewModel
    {
        private readonly Uri _baseUri;
        private readonly Func<IModel?> _getCurrentApp;
        private readonly ILogService _logService;

        public FixDocumentNamesDockablePaneViewModel(Uri baseUri, Func<IModel?> getCurrentApp, ILogService logService)
        {
            _baseUri = baseUri;
            _getCurrentApp = getCurrentApp;
            _logService = logService;
        }
        public override void InitWebView(IWebView webView)
        {
            webView.Address = new Uri(_baseUri, "index");

            webView.MessageReceived += (_, args) =>
            {
                var currentApp = _getCurrentApp();
                if (currentApp == null) return;

                if (args.Message == "setDocumentName")
                {
                    var currentDocumentName = args.Data["currentDocumentName"]?.GetValue<string>();
                    var newDocumentName = args.Data["newDocumentName"]?.GetValue<string>();
                    //ChangeDocumentName(currentApp, currentDocumentName, newDocumentName);
                    webView.PostMessage("RefreshList");
                }

            };
        }

        //private void ChangeDocumentName(IModel currentApp, string currentName, string newName)
        //{
        //    DocumentHandler
        //}
    }
}
