using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
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
        private readonly IDockingWindowService dockingWindowService;

        public FixDocumentNamesDockablePaneViewModel(Uri baseUri, Func<IModel?> getCurrentApp, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService, IDockingWindowService dockingWindowService)
        {
            _baseUri = baseUri;
            _getCurrentApp = getCurrentApp;
            _logService = logService;
            this.bgService = bgService;
            this.msgService = msgService;
            this.dockingWindowService = dockingWindowService;
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
                else if (args.Message == "OpenDocument")
                {
                    bool documentOpened = false;
                    string documentName = "";
                    IDocument documentToOpen = null;
                    try
                    {
                        documentName = (string)args.Data["documentName"];
                        documentToOpen = FindDocument(documentName, currentApp);
                        documentOpened = dockingWindowService.TryOpenEditor(documentToOpen);
                    }
                    catch (Exception ex)
                    {
                        _logService.Error($"Error: {ex}");
                    }
                };
            };
        }

        public IDocument FindDocument(string name, IModel currentApp)
        {
            IDocument document = null;
            if (currentApp == null)
            {
                return null;
            }
            string moduleName = name.Split('.')[0];
            string documentName = name.Split('.')[1];
            IReadOnlyList<IModule> moduleList = currentApp.Root.GetModules();
            IModule module = moduleList.FirstOrDefault(m => m.Name == moduleName);

            module.GetDocuments().ToList()
                .ForEach(item =>
                {
                    if (item.Name == documentName)
                    {
                        document = item;
                        return;
                    }
                });
            return document;
        }
    }
}
