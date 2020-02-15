using System.Collections.Generic;

namespace Contoso.Infrastructure.Models
{
    public class SiteMapNodeInfo
    {
        public SiteMapNodeInfo()
        {
            ChildNodes = new HashSet<SiteMapNodeInfo>();
        }

        public string Title { get; set; }
   
        public string ModuleName { get; set; }

        public string ViewTypeAssemblyQualifiedName { get; set; }

        public SiteMapNodeInfo Parent { get; set; } = null;

        public ICollection<SiteMapNodeInfo> ChildNodes { get; private set; }
    }
}
