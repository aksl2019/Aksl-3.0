   M S B A M L   `   `   `  ����S  OContoso.Modules.Customer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullO,clr-namespace:Contoso.Modules.Customer.ViewsContoso.Modules.Customer.Views  5      Y1clr-namespace:Contoso.Modules.Customer.ViewModels#Contoso.Modules.Customer.ViewModels  5	      9    3Contoso.Modules.Customer.Views.CustomerGroupHubView   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      D @Prism.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullJ FPrism.Unity.Wpf, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&prismhttp://prismlibrary.com/   5      :views,clr-namespace:Contoso.Modules.Customer.Views   5      D
viewmodels1clr-namespace:Contoso.Modules.Customer.ViewModels   5	      !  Prism.Mvvm.ViewModelLocator    AutoWireViewModel$
  True��5
       � 	Resources 5         inTransition �� 
TargetName �� TargetProperty W� From	 W� To g� Duration  customerListDataTemplate �� DataType@   :Contoso.Modules.Customer.ViewModels.CustomerGroupViewModel6   0Contoso.Modules.Customer.Views.CustomerGroupView  customerDataTemplate;   5Contoso.Modules.Customer.ViewModels.CustomerViewModel1   +Contoso.Modules.Customer.Views.CustomerViewG   AContoso.Modules.Customer.ViewModels.WorkspaceDataTemplateSelector ! workspaceDataTemplateSelector   CustomerGroupTemplate	   CustomerTemplate  closableTabItemTemplate
 � Cursor �� Path  workspacesTemplate$ �� IsSynchronizedWithCurrentItem �� ContentTemplateSelector��%�  &	        &	 �     &	 �     &	 �     7  7  &	     &	 �    7  7  �� 5   
   .��5      W� $ 
waitingBar��6   $ Value�5       05       1005      $ 0:0:5D�5      5      �� 5   
   "  5      .R�5        5      �� 5   
   "  5      .R�5        5        5!   
   8   8	  5#   3   �� 5%   
   .R�5&      ]� $	��120��6   .+�5'      �� $��X��5(      $

 Hand��5)      $|�False��5*      $��Courier)�5+      $��9(�5,      $
��Bold"�5-      $��Right=�5.      $��16��5/      $��16��50      $��0,1,0,0q�51      $��0q�52      $��Bottom=�53      ��5'      �� $ CloseCommand�53      �� 54      $��Center=�55   #   ��54   #   �� $ DisplayName�55   #   56      57      �� 59   
   .R�5:      �� $
 TrueY�6   8��  8  $��4q�5>      ��5;      �� 5>      5?      5@      .��5B      �  J� SnapsToDevicePixels$
 True��6    � RowDefinitions 5C   
   �� 5D      $��30 �6   �� 5E      $��2 �6   �� 5F      $��* �6   .+�5I   
   �� $��0��6   $��NewCustomer��5J      $��80��5L      $��26��5M      $
��Left=�5N      $��Center=�5O      $��2q�5P      ��5K      �� $ NewCustomerCommand�5P      �� 5R   
   $��2��6   $��2q�5S      #��[ 5U      ��5T      �� $ 
Workspaces�5U      � 5W   
   -   5        $��
waitingBar��5W       � Value$	 100Q�5X      $	��200��5Y      $��30��5Z      $��3��5[      y�5\      �� $ 	IsLoading� �� Mode$ OneWay=�   booleanToVisibilityConverter �� 	Converter# [  � Triggers 5]      :� 5^       :� RoutedEvent$ FrameworkElement.Loaded��6   .[�5_      �� #��[  6&   5`      5b      5c      5e      