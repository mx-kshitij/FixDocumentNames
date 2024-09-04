using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;

namespace FixDocumentNames
{
    [Export(typeof(DockablePaneExtension))]

    public class FixDocumentNamesDockablePaneExtension: DockablePaneExtension
    {
        private readonly ILogService _logService;
        public const string PaneId = "FixDocumentNames";

        [ImportingConstructor]
        public FixDocumentNamesDockablePaneExtension(ILogService logService)
        {
            _logService = logService;
        }

        public override string Id => PaneId;

        public override DockablePaneViewModelBase Open()
        {
            return new FixDocumentNamesDockablePaneViewModel(WebServerBaseUrl, () => CurrentApp, _logService) { Title = "Fix Document Names" };
        }
    }
}
