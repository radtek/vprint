   M S B A M L   `   `   `  ����  ModernBrowserApp!    ModernBrowserApp.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  My Modern Browser��5   	   $	��600��6#   $	��800��60   .��5      �  � RowDefinitions 5   
   �� 5      $	��30* �6   �� 5      $��8* �6   �� 5	      $
��320* �6    � ColumnDefinitions 5   
   �� 5      $	��30* �6   �� 5      $	��30* �6   �� 5      $
��480* �6   �� 5      $	��30* �6   .+�5   
   �� -   5        $��TxbAddressBar��5      $��2��6'   $��0��67   6D   �� 5   
   -   5        $��BtnPrev��5      $��0��6    $��0��60      ModernPrevButton#��[  6S   �� 5   
   -   5        $��BtnNext��5      $��1��6    $��0��60     ModernNextButton#��[ 6S   �� 5   
   -   5        $��BtnGo��5      $��3��6   $��0��6.     ModernGoButton#��[ 6Q   � 5   
   -   5        $��PgbFrameLoading��5      $��4��6-   $��1��6A    � 5   
   -   5        $��	MainFrame��5      $��2��6!   $��4��6.     � NavigationUIVisibility$ Hidden=�5      5      5      5      