using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;

namespace FixDocumentNames
{
    [Export(typeof(DockablePaneExtension))]

    public class FixDocumentNamesDockablePaneExtension: DockablePaneExtension
    {
        private readonly ILogService _logService;
        private readonly IBackgroundJobService bgService;
        private readonly IMessageBoxService msgService;
        private readonly IDockingWindowService dockingWindowService;
        public const string PaneId = "FixDocumentNames";

        [ImportingConstructor]
        public FixDocumentNamesDockablePaneExtension(ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService, IDockingWindowService dockingWindowService)
        {
            _logService = logService;
            this.bgService = bgService;
            this.msgService = msgService;
            this.dockingWindowService = dockingWindowService;
        }

        public override string Id => PaneId;

        public override DockablePaneViewModelBase Open()
        {
            return new FixDocumentNamesDockablePaneViewModel(WebServerBaseUrl, () => CurrentApp, _logService, bgService, msgService, dockingWindowService) { Title = "Fix Document Names" };
        }
    }
}
