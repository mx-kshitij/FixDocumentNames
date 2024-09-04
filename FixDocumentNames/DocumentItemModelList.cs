using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace FixDocumentNames
{
    public record DocumentItemModelList
    {
        [JsonConstructor]
        public DocumentItemModelList(List<DocumentItemModel> documentItems) 
        {
            DocumentItems = documentItems;
        }
        public List<DocumentItemModel> DocumentItems { get; }
    }
}
