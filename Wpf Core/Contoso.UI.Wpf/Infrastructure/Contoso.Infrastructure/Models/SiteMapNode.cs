using System.Collections.Generic;

namespace Contoso.Infrastructure.Models
{
    public class SiteMapNode
    {
        public SiteMapNode()
        {
            ChildNodes= new HashSet<SiteMapNode>();
        }

        public string Title { get; set; }

        public bool IsSeparator { get; set; } = false;

        public string ModuleName { get; set; }

        public string ViewTypeAssemblyQualifiedName { get; set; }

        public SiteMapNode Parent { get; set; } = null;

        public ICollection<SiteMapNode> ChildNodes { get;private set; }
    }
}
