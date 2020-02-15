using System.Collections.Generic;

namespace Contoso.Infrastructure
{
    public class MenuItem
    {
        public MenuItem()
        {
        }

        public int Id { get; set; }

        public MenuItemType ItemType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 字符图标,&#xE10F;
        /// </summary>
     //   public string FontIcon { get; set; } //<FontIcon Glyph="{Binding FontIcon}"/>

      //  public string Glyph { get; set; } //<FontIcon Glyph="{Binding FontIcon}"/>

        /// <summary>
        /// 图标,Admin
        /// </summary>
      //  public Symbol Icon { get; set; }  //<SymbolIcon Symbol="{Binding Icon}"/>

       // public char IconAsChar => (char)Icon; //<FontIcon Glyph="{Binding IconAsChar}"/>

        /// <summary>
        /// 所在层
        /// </summary>
        public int Level { get; set; }

        public string ModuleName { get; set; }

        public string ViewName { get; set; }

        public MenuItem Parent { get; set; } = null;

        /// <summary>
        /// 子节点
        /// </summary>
        public ICollection<MenuItem> SubMenus { get; } = new List<MenuItem>();

        //权限
        public string RequrePermissons { get; set; }

        public bool CanRun { get; set; }

        public bool IsSeparator { get; set; }


        public bool IsHome { get; set; }
    }

    public enum MenuItemType
    {
        Header,
        Item,
        Separator,
        AutoSuggestBox
    }
}
