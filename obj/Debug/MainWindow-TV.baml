   M S B A M L   `   `   `  ����  DroneStation    DroneStation.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   $��462.822��6@   $��119.143��6Q     � 
ResizeMode$  NoResize=�6a    � Topmost . ���	#FF535353���	#FF2C2C2C � WindowStyle$ 
ToolWindow=�6�   ��.   � Title$ Control��6�    � WindowStartupLocation$ CenterScreen=�6�   .��5      � $��0,10,-160,-300q�6    � ColumnDefinitions 5   
   �� 5      $	��335 �6   .+�5   
   �� -   5        $��btnStartArdu��5      $��
Arducopter��6%   $
��Left=�6:   $��20��6U   $	��Top=�6a   $��84��6y   $��10,1,0,0q�6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5	   
   -   5        $��btnStopArdu��5	      $��	Reboot...��6$   $
��Left=�68   $��20��6S   $��
10,225,0,0q�6_   $	��Top=�6s   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5
   
   -   5        $��btnVideoHigh��5
      $��
High Video��6%   $
��Left=�6:   $��20��6U   $��	10,76,0,0q�6a   $	��Top=�6t   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5   
   -   5        $��btnVideoLow��5      $��	Low Video��6$   $
��Left=�68   $��20��6S   $��	10,26,0,0q�6_   $	��Top=�6r   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5   
   -   5        $��btnVideoMed��5      $��	Med Video��6$   $
��Left=�68   $��20��6S   $��	10,51,0,0q�6_   $	��Top=�6r   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5   
   -   5        $��btnPhoto��5      $��Photo��6!   $
��Left=�61   $��20��6L   $��
10,175,0,0q�6X   $	��Top=�6l   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5   
   -   5        $��btnMissionPlanner��5      $��Planner��6*   $
��Left=�6<   $��20��6W   $��
10,518,0,0q�6c   $	��Top=�6w   $��84��6�   6�   �� 5   
   -   5        $��btnStopVideo��5      $��
Stop Video��6%   $
��Left=�6:   $��20��6U   $��
10,101,0,0q�6a   $	��Top=�6u   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5   
   -	   5         ��Name$ btnOpenVideo��5      $��Open Player��6'   $
��Left=�6=   $��20��6X   $��
10,126,0,0q�6d   $	��Top=�6x   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5   
   -
   5        $ btnCloseVideo��5      $��Close Player��6(   $
��Left=�6?   $��20��6Z   $��
10,151,0,0q�6f   $	��Top=�6z   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   	� 5   
   -   5         	�Name$ rdbWiFi��5      $
��WiFi��6'   $
��Left=�66   $��
10,273,0,0q�6Q   $	��Top=�6e   $��14��6�   $	��106��6�   ���	#FF7A7A7A
����������	#FF4646466�   	� 5   
   -   5        $ rdb4G��5      $��Internet��6%   $
��Left=�68   $��
10,292,0,0q�6S   $	��Top=�6g   $��15��6�   $	��125��6�   ���	#FF7A7A7A
����������	#FF4646466�   �� 5   
   -   5        $ btnOpenPutty��5      $��Putty��6'   $
��Left=�67   $��20��6R   $��
10,200,0,0q�6^   $	��Top=�6r   $��84��6�   ���	#FF404040���	#FF818181
�������{�.  6�   �� 5   
   -   5         ��Name$ txtInfo��5      $
��Left=�6#   $��92��6>   $��
10,312,0,0q�6J   	 �� TextWrapping$
	 Wrap=�6^   $	��Top=�6r   $��84��6�   
���   ����	#FFFF3131$��11(�6�   
���   �6�   � 5   
   -   5        
 �Name$
 prgConnection��5      $
��Left=�6-   $��9��6H   $��168,382,0,0q�6S   $	��Top=�6h   $��31��6�    � Value$ 10Q�6�   
���   � J� RenderTransformOrigin$ 1.023,0.531&�6�   
���   �
�������z�5      V� 5      .~�5      �� �� 5      �� 5       �� Angle$	 -90Q�6&   T� 5       T� X$ -110.93Q�6)    T� Y$ -112.914Q�65   5      5      �� 5    
   -   5        $ btnSettings��5       $��Settings...��6&   $
��Left=�6<   $��20��6W   $��
10,250,0,0q�6c   $	��Top=�6w   $��84��6�   ���	#FF404040���	#FF818181
�������6�   5"      5#      