   M S B A M L   `   `   `  ����G  CDroneStation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:DroneStationDroneStation  5   	       DroneStation.ScriptWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:DroneStation     5   	     � Title$9  3Save this script o your Raspberry Pi on your drone:��5   	   $��388.348��6E   $��545.136��6V    � WindowStartupLocation$ CenterScreen=�6f    � 
ResizeMode$ NoResize=�6�   .��5	      � $��0,0,0,-2q�6   .+�5
   
   �� -   5         ��Name$ 	txtScript��5
      $
��Left=�6%   $	��286��6@   $��	21,10,0,0q�6M    �� TextWrapping$
 Wrap=�6`   $��TextBox��6t   $	��Top=�6�   $	��488��6�   $��9(�6�   " �� VerticalScrollBarVisibility$
 Auto=�6�   �� 5   
   -   5         ��Name$ btnOk��5      $��Close��6    $
��Left=�60   $��399,314,0,0q�6K   $	��Top=�6`   $	��110��6x   $��23��6�   �� 5   
   -   5        $ btnSave��5      $��Save to file...��6"   $
��Left=�6<   $��
21,314,0,0q�6W   $	��Top=�6k   $	��110��6�   $��23��6�   �� 5   
   -   5        $ btnCopy��5      $��Copy to clipboard��6"   $
��Left=�6>   $��136,314,0,0q�6Y   $	��Top=�6n   $	��110��6�    J� RenderTransformOrigin$ -0.421,0.43&�6�   $��23��6�   5      5      