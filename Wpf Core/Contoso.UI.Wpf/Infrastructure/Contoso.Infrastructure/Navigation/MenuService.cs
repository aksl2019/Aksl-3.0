using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contoso.Infrastructure
{
    public class MenuService : IMenuService
    {
        private static  readonly ConcurrentDictionary<string, IMenuProvider> _menuProviders = new ConcurrentDictionary<string, IMenuProvider>();

        private IMenuProvider _menuProvider;

        public MenuService(IMenuProvider menuProvider)
        {
            _menuProvider = menuProvider;
        }

        public void Register(string name, IMenuProvider menuProvider)
        {
            if (!_menuProviders.TryAdd(name, menuProvider))
            {
                throw new InvalidOperationException($"menuProvider already registered '{name}'");
            }
        }

        public IEnumerable<MenuItem> GetMenus(string name)
        {
            if (_menuProviders.TryGetValue(name, out IMenuProvider  menuProvider))
            {
                var root = _menuProvider.Root;

                return root.SubMenus.ToArray();

            }

            throw new InvalidOperationException($"View not registered for MenuProvider '{name}'");
        }

        public async Task<IEnumerable<MenuItem>> BuildMenuAsync(string menuFilePath)
        {
            //var task = Task.Run(() =>
            //{
            //    var menus = new List<MenuItem>
            //    {
            //        new MenuItem() {Title = "Default",Icon=Symbol.Admin,ViewName = typeof(Views.DefaultView).ToString() },
            //        new MenuItem() { Title = "Locate Record Specific Profile",Icon=Symbol.SolidStar,ViewName = typeof(Views.SetRecordProfileView).ToString() },
            //        new MenuItem() { Title = "Query Profile for Concurrency",Icon=Symbol.SlideShow,ViewName = typeof(Views.ConcurrentProfileView).ToString() },
            //        new MenuItem() { Title = "Query Profile for HDR Support",Icon=Symbol.Play,ViewName = typeof(Views.EnableHdrProfileView).ToString() }
            //    };
            //    return menus.AsEnumerable();
            //});
            //return task;

            var root = await _menuProvider.BuildMenuAsync(menuFilePath);

            return root.SubMenus.ToArray();
        }
    }
}
