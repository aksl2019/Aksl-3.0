//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;


//using Prism.Mvvm;
//using Prism.Events;
//using Prism.Logging;
//using Prism.Regions;
//using Prism.Modularity;
//using Prism.Unity;
//using Prism.Unity.Properties;
//using Prism.Unity.Regions;


//using Tree.UI.Modules.LayoutUI;

//namespace Contoso.UI.Wpf
//{
//   public  class Bootstrapper: UnityBootstrapper
//    {
//        protected override ILoggerFacade CreateLogger()
//        {
//            return base.CreateLogger();
//        }

//        //protected override IModuleCatalog CreateModuleCatalog()
//        //{
//        //    return base.CreateModuleCatalog();
//        //}

//        protected override void ConfigureModuleCatalog()
//        {
//            ModuleCatalog moduleCatalog = (ModuleCatalog)this.ModuleCatalog;
//            moduleCatalog.AddModule(typeof(LayoutUIModule).Name, typeof(LayoutUIModule).AssemblyQualifiedName, InitializationMode.WhenAvailable);
//        }

//        //protected override IUnityContainer CreateContainer()
//        //{
//        //    return base.CreateContainer();
//        //}

//        //protected override void ConfigureContainer()
//        //{
//        //    base.ConfigureContainer();
//        //}

//        //protected override void ConfigureServiceLocator()
//        //{
//        //    base.ConfigureServiceLocator();
//        //}

//        protected override void ConfigureViewModelLocator()
//        {
//            base.ConfigureViewModelLocator();

//            ViewModelLocationProvider.Register(typeof(ShellView).ToString(), () => Container.TryResolve<ShellViewModel>());
//           // ViewModelLocationProvider.Register(typeof(ConfirmView).ToString(), () => Container.Resolve<ConfirmViewModel>());
//        }

//        //protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
//        //{
//        //    return base.ConfigureRegionAdapterMappings();
//        //}

//        //protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
//        //{
//        //    return base.ConfigureDefaultRegionBehaviors();
//        //}

//        //protected override void RegisterFrameworkExceptionTypes()
//        //{
//        //    base.RegisterFrameworkExceptionTypes();
//        //}

//        protected override DependencyObject CreateShell()
//        {
//            return Container.TryResolve<ShellView>();
//        }

//        protected override void InitializeShell()
//        {
//            App.Current.MainWindow = (Window)this.Shell;
//            App.Current.MainWindow.Show();
//        }

//        //protected override void InitializeModules()
//        //{
//        //    base.InitializeModules();
//        //}
//    }
//}
