using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using Mendix.StudioPro.ExtensionsAPI.Model;
using Mendix.StudioPro.ExtensionsAPI.Services;
using Mendix.StudioPro.ExtensionsAPI.Model.Projects;
using Mendix.StudioPro.ExtensionsAPI.Model.Microflows;
using Mendix.StudioPro.ExtensionsAPI.UI.Services;
using Mendix.StudioPro.ExtensionsAPI.BackgroundJobs;

namespace FixDocumentNames
{
    public class DocumentItemListHandler
    {
        private readonly ILogService _logService;
        private readonly IModel _currentApp;
        private readonly IBackgroundJobService _bgService;
        private readonly IMessageBoxService _msgService;

        public DocumentItemListHandler(IModel currentApp, ILogService logService, IBackgroundJobService bgService, IMessageBoxService msgService)
        {
            _logService = logService;
            _currentApp = currentApp;
            _bgService = bgService;
            _msgService = msgService;
        }

        public List<DocumentItemModel> LoadDocumentList(string searchKey)
        {
            List<DocumentItemModel> documentItemList = [];
            if (searchKey != null)
            {
                IReadOnlyList<IModule> moduleList;
                try
                {
                    moduleList = _currentApp.Root.GetModules();
                    foreach (var module in moduleList)
                    {
                        module.GetDocuments().ToList()
                            .ForEach(document =>
                            {
                                if (document.Name.Contains(searchKey))
                                {
                                    DocumentItemModel documentItem = new(module.Name, document.Name, "");
                                    documentItemList.Add(documentItem);
                                }
                            });
                    }

                }
                catch (Exception ex)
                {
                    _logService.Error(ex.Message);
                }
            }
            return documentItemList;
        }

        public void FixDocumentList(string documentJSON)
        {
            DocumentFixModel documentFixModel = JsonSerializer.Deserialize<DocumentFixModel>(documentJSON)!;
            CheckedItem curItem;
            IModule module;
            IDocument documentToFix;
            string documentsFixFailed = "";
            string newDocumentName = "";

            BackgroundJob bgJob = new BackgroundJob("Fix documents");
            bgJob.AddStep("Fix documents", "Changing document names", () =>
            {
                using (var transaction = _currentApp!.StartTransaction("create microflow function"))
                {
                    for (int i = 0; i < documentFixModel.checkedItems.Count; i++)
                    {
                        curItem = documentFixModel.checkedItems[i];
                        if (curItem != null)
                        {
                            module = _currentApp.Root.GetModules().ToList().Find(item => item.Name == curItem.module);
                            documentToFix = module.GetDocuments().ToList().Find(doc => doc.Name == curItem.document);
                            newDocumentName = documentToFix.Name.Replace(documentFixModel.searchKey, documentFixModel.replacementText);
                            try
                            {
                                documentToFix.Name = newDocumentName;
                            }
                            catch (Exception ex)
                            {
                                _logService.Error("Could not rename document", ex);
                                if(documentsFixFailed == "")
                                {
                                    documentsFixFailed = documentToFix.Name;
                                }
                                else
                                {
                                    documentsFixFailed = documentsFixFailed + ", " + documentToFix.Name;
                                }
                            }
                        }
                    }
                    try
                    {
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        _logService.Error("Could not rename document", ex);
                    }
                }
                if(documentsFixFailed != "")
                {
                    _msgService.ShowError("Failed to fix documents : " + documentsFixFailed);
                }
                return true;
            });
            //IBackgroundJobService bgService;
            _bgService.Run(bgJob);
        }
    }
}
