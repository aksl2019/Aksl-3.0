using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Contoso.Infrastructure
{
    public class MenuProvider : IMenuProvider
    {
        #region Members
        private bool _isInitialized = false;
        #endregion

        #region Constructors
        public MenuProvider()
        {
        }
        #endregion

        #region Properties
        public MenuItem Root { get; private set; }
        #endregion

        #region Build Menu Method
        public  Task<MenuItem> BuildMenuAsync(string menuFilePath)
        {
            //if (!_isInitialized)
            //{
            //    using (Stream stream = await GetResourceStreamAsync(menuFilePath))
            //    {
            //        using (XmlReader xmlRdr = XmlReader.Create(stream))
            //        {
            //            XElement rootElement = XDocument.Load(xmlRdr).Element("MenuItem");
            //            Root = CreateMenuItemFromXml(rootElement);

            //            if (rootElement.Elements("MenuItem").Count() > 0)
            //            {
            //                foreach (var child in rootElement.Elements("MenuItem"))
            //                {
            //                    AddSubMenus(child, Root);
            //                }
            //            }
            //            _isInitialized = true;
            //        }
            //    }
            //}

            //return Root;

            #region MemoryMenu
            if (!_isInitialized)
            {
                XElement rootElement = CreateMemoryMenu();
                Root = CreateMenuItemFromXml(rootElement);

                if (rootElement.Elements("MenuItem").Count() > 0)
                {
                    foreach (var child in rootElement.Elements("MenuItem"))
                    {
                        AddSubMenus(child, Root);
                    }
                }

                _isInitialized = true;
            }

            return Task.FromResult(  Root);
            #endregion
        }
        #endregion

        #region Add Sub Menu Methods
        private void AddSubMenus(XElement currentElement, MenuItem parent)
        {
            var newMenuItem = CreateMenuItemFromXml(currentElement);
            AddSubMenu(newMenuItem, parent);

            foreach (var child in currentElement.Elements("MenuItem"))
            {
                AddSubMenus(child, newMenuItem);
            }
        }

        private void AddSubMenu(MenuItem menuItem, MenuItem parentMenu)
        {
            parentMenu.SubMenus.Add(menuItem);
            menuItem.Parent = parentMenu;
        }

        private MenuItem CreateMenuItemFromXml(XElement element)
        {
            var menuItem = new MenuItem
            {
                Title = element.Attribute("title")?.Value
            };

            //menuItem.FontIcon = element.Attribute("fontIcon")?.Value;
            //if (element.Attribute("icon") != null)
            //{
            //    var icon = element.Attribute("icon").Value;
            //    menuItem.Icon = (Symbol)Enum.Parse(typeof(Symbol), icon);
            //}
            //if (element.Attribute("glyph") != null)
            //{
            //    try
            //    {
            //        var glyphHex = element.Attribute("glyph").Value;
            //        var glyphInt = Convert.ToInt32(glyphHex, 16);
            //        menuItem.Glyph = Char.ConvertFromUtf32(glyphInt).ToString();
            //    }
            //    catch
            //    {
            //        menuItem.Glyph = Char.ConvertFromUtf32(0x0000);
            //    }
            //}

            if (element.Attribute("itemType")?.Value != null)
            {
                var type = element.Attribute("itemType").Value;
                if (Enum.TryParse(type, out MenuItemType result))
                {
                    menuItem.ItemType = result;
                }
                else
                {
                    menuItem.ItemType = MenuItemType.Item;
                }
            }

            // var fontIcon = element.Attribute("fontIcon")?.Value;
            menuItem.Level = int.Parse(element.Attribute("level")?.Value);
            //  var isSeparator = (element.Attribute("isSeparator")?.Value) == null ? false : bool.Parse(element.Attribute("isSeparator")?.Value);
            // var navServiceName = element.Attribute("navigationServiceName")?.Value;
            menuItem.ModuleName = element.Attribute("moduleName")?.Value;
            menuItem.ViewName = element.Attribute("viewName")?.Value;
            menuItem.RequrePermissons = element.Attribute("requrePermissons")?.Value;
            //  return new MenuItem() { Title = title, Icon = fontIcon, Level = level, ViewName = viewName };
            if (element.Attribute("isHome") != null)
            {
                var isHome = element.Attribute("isHome").Value;
                menuItem.IsHome = bool.Parse(isHome);
            }
            //  menuItem.IsHome = bool.Parse(element.Attribute("isHome")?.Value);
            return menuItem;
        }
        #endregion

        #region Get ResourceStream Method
        //private Task<Stream> GetResourceStreamAsync(string menuFilePath)
        //{
        //    // Uri uri = new Uri("ms-appx:///Data/MenuItems.xml");
        //    Uri uri = new Uri(menuFilePath);
        //    // Uri uri = new Uri("pack://application:,,,/TreeRI.Modules.LayoutAwarer;Component/Data/SiteMap.xml");
        //    //StreamResourceInfo info = Application.GetResourceStream(uri);

        //    //if (info == null || info.Stream == null)
        //    //{
        //    //    throw new ApplicationException($"Missing resource file:{menuFilePath}");
        //    //}

        //    //return Task.FromResult(info.Stream);
        //    return Task.FromResult(null);
        //}
        #endregion

        #region Create MemoryMenu Method
        private XElement CreateMemoryMenu()
        {
            XElement menu =
                 new XElement("MenuItem", new XAttribute("id", "0"), new XAttribute("title", "根"), new XAttribute("level", "0"),
                   new XElement("MenuItem", new XAttribute("id", "1"), new XAttribute("title", "主页"), 
                                            new XAttribute("level", "1"), new XAttribute("fontIcon", "Admin"), 
                                            new XAttribute("icon", "Admin"),
                                            new XAttribute("moduleName", "HomeUIModule"),
                                            new XAttribute("viewName", "Contoso.Modules.Home.Views.HomeView,Contoso.Modules.Home"), 
                                            new XAttribute("requrePermissons", "Admin"),
                                            new XAttribute("isHome", "true")),
                   new XElement("MenuItem", new XAttribute("id", "2"), 
                                            new XAttribute("title", "WebSocket Listener"), 
                                            new XAttribute("level", "1"), 
                                            new XAttribute("fontIcon", "SlideShow"), 
                                            new XAttribute("icon", "SlideShow"),
                                            new XAttribute("moduleName", "Contoso.Modules.TreeBar.TreeBarModule"),
                                            new XAttribute("viewName", "Contoso.Modules.TreeBar.Views.TreeBarHubView,Contoso.Modules.TreeBar"),
                                              new XElement("MenuItem", new XAttribute("id", "2"),
                                                new XAttribute("title", "客户"),
                                                new XAttribute("level", "1"),
                                                new XAttribute("fontIcon", "SlideShow"),
                                                new XAttribute("icon", "SlideShow"),
                                                new XAttribute("moduleName", ""),
                                                new XAttribute("viewName", ""),
                                                new XAttribute("requrePermissons", "Admin"),
                                                new XAttribute("isHome", "false"),
                                                   new XElement("MenuItem", new XAttribute("id", "2"),
                                                   new XAttribute("title", "所有"),
                                                   new XAttribute("level", "1"),
                                                   new XAttribute("fontIcon", "SlideShow"),
                                                   new XAttribute("icon", "SlideShow"),
                                                   new XAttribute("moduleName", "HomeUIModule"),
                                                   new XAttribute("viewName", "Contoso.Modules.Home.Views.HomeView,Contoso.Modules.Home"),
                                                   new XAttribute("requrePermissons", "Admin"),
                                                   new XAttribute("isHome", "false")),
                                                   new XElement("MenuItem", new XAttribute("id", "2"),
                                                   new XAttribute("title", "新增"),
                                                   new XAttribute("level", "1"),
                                                   new XAttribute("fontIcon", "SlideShow"),
                                                   new XAttribute("icon", "SlideShow"),
                                                   new XAttribute("moduleName", "HomeUIModule"),
                                                   new XAttribute("viewName", "Contoso.Modules.Home.Views.HomeView,Contoso.Modules.Home"),
                                                   new XAttribute("requrePermissons", "Admin"),
                                                   new XAttribute("isHome", "false")))),
                   new XElement("MenuItem", new XAttribute("id", "3"), new XAttribute("title", "Locate Record Specific Profile"), new XAttribute("level", "1"), new XAttribute("fontIcon", "SolidStar"), new XAttribute("icon", "SolidStar"), new XAttribute("viewName", "Insurance.Management.Uwp.Views.SetRecordProfileView,Insurance.Management.Uwp")),
                   new XElement("MenuItem", new XAttribute("id", "4"), new XAttribute("title", "Query Profile for HDR Support"), new XAttribute("level", "1"), new XAttribute("fontIcon", "Play"), new XAttribute("icon", "Play"), new XAttribute("viewName", "Insurance.Management.Uwp.Views.EnableHdrProfileView,Insurance.Management.Uwp"))
                               );

            return menu;
        }
        #endregion
    }
}
