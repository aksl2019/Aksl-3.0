using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contoso.Infrastructure
{
    public interface IMenuService
    {
        void Register(string name, IMenuProvider menuProvider);

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MenuItem>> BuildMenuAsync(string menuFilePath);

        //Task<MenuItem> CreatePrimayMenuAsync();

        //Task<MenuItem> CreateSecondaryMenuAsync();
    }
}
 