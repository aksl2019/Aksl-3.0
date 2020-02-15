using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.Infrastructure
{
    public interface IMenuProvider
    {
        MenuItem Root{ get; }

        //ms-appx:///Data/MenuItems.xml
        Task<MenuItem> BuildMenuAsync(string menuFilePath);
    }
}
