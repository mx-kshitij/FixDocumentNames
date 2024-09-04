using Mendix.StudioPro.ExtensionsAPI.UI.Menu;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mendix.StudioPro.ExtensionsAPI.UI.DockablePane;

namespace FixDocumentNames
{
    [Export(typeof(Mendix.StudioPro.ExtensionsAPI.UI.Menu.MenuExtension))]
    public class FixDocumentNamesMenuExtension : MenuExtension
    {
        private readonly IDockingWindowService _dockingWindowService;

        [ImportingConstructor]
        public FixDocumentNamesMenuExtension(IDockingWindowService dockingWindowService)
        {
            _dockingWindowService = dockingWindowService;
        }

        public override IEnumerable<MenuViewModel> GetMenus()
        {
            yield return new MenuViewModel("Fix document names", () => _dockingWindowService.OpenPane(FixDocumentNamesDockablePaneExtension.PaneId));
        }
    }
}
