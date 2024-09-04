using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixDocumentNames
{
    public class DocumentFixModel
    {
            public required string searchKey { get; set; }
            public required string replacementText { get; set; }
            public required List<CheckedItem> checkedItems { get; set; }
    }
    
    public class CheckedItem
    {
        public required string module { get; set; }
        public required string document { get; set; }
    }
}
