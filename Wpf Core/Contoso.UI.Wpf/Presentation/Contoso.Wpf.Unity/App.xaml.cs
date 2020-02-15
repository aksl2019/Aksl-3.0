using System.Windows;

using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;

using Contoso.Infrastructure;
using Contoso.Modules.Menu;
using Contoso.Modules.TreeBar;
using Contoso.Modules.Home;
using Contoso.Modules.Shell;
using Contoso.Modules.Shell.ViewModels;
using Contoso.Modules.Shell.Views;

namespace Contoso.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            ViewModelLocationProvider.Register(typeof(ShellView).ToString(), () => Container.Resolve<ShellViewModel>());
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton(typeof(IMenuProvider), typeof(MenuProvider));
            containerRegistry.RegisterSingleton(typeof(IMenuService), typeof(MenuService));
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule(typeof(HomeUIModule).Name, typeof(HomeUIModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            moduleCatalog.AddModule(typeof(TreeBarModule).Name, typeof(TreeBarModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);

            moduleCatalog.AddModule(typeof(MenuUIModule).Name, typeof(MenuUIModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
            moduleCatalog.AddModule(typeof(ShellModule).Name, typeof(ShellModule).AssemblyQualifiedName, InitializationMode.WhenAvailable, typeof(MenuUIModule).Name);
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }
    }
}
