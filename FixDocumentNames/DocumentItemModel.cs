using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FixDocumentNames
{
    public record DocumentItemModel
    {
        [JsonConstructor]
        public DocumentItemModel(string id, string module, string currentDocumentName, string newDocumentName)
        {
            Id = id;
            Module = module;
            CurrentDocumentName = currentDocumentName;
            NewDocumentName = newDocumentName;
        }

        public DocumentItemModel(string module, string currentDocumentName)
            : this(Guid.NewGuid().ToString(), module, currentDocumentName) { }

        public DocumentItemModel(string module, string currentDocumentName, string newDocumentName)
            : this(Guid.NewGuid().ToString(), module, currentDocumentName, newDocumentName) { }

        public string Id { get; set; }
        public string Module { get; set; }
        public string CurrentDocumentName { get; set; }
        public string NewDocumentName { get; set; }
    }
}
