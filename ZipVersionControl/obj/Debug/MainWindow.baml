   M S B A M L   `   `   `  ����X  TZipVersionControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e9baa711da40215a5clr-namespace:ZipVersionControlZipVersionControl  5   	   "    ZipVersionControl.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:ZipVersionControl     5   	     � Title$  ZipVersionControl��5   	   $	��114��6#   $	��300��60    � 
ResizeMode$ CanMinimize=�6<    � SizeToContent$ WidthAndHeight=�6U    � WindowStartupLocation$ CenterScreen=�6t   .��5	      � $��0q�6   .+�5
   
   � $��
0,18,10,10q�6   .+�5      �� -   5         ��Name$ cbxProfileSelector��5      $
��Left=�63   $��	10,10,0,0q�6N   $	��Top=�6a   $	��274��6y    �� SelectedIndex$ 0��6�   ��6�   �� ��5      �� 5      .R�5      �� ��6$   �� *ProfileName+5      5      �� 5      -   5        $ cbxSync��5      $
��Left=�6(   $��	10,37,0,0q�6C   $��1��6V   $	��Top=�6c   $��91��6{   $��20��6�   ���	#FF707070$��Center=�6�   $��Center=�6�   ���	#FFDDDDDD{�.  .@�5      �� -   5        $��Commit��5      60   5      �� 5      -   5         ��Name$ btnSync��5      $
��Sync��6&   $
��Left=�65   $��	10,37,0,0q�6P   $	��Top=�6c   $��75��6{    �� 	IsDefault . {�.  6�   �� 5      -   5        $ btnSettings��5      $��Settings��6*   $
��Left=�6=   $��
209,37,0,0q�6X   $	��Top=�6l   $��75��6�   {�.  6�   5      o� 5   
   $
��Left=�6   $��18��6*   $	��Top=�66   $	��294��6N   .@�5      m� $
��File��6   .@�5      m� -   5        $��New profile...��5      63   m� 5      -   5        $��
Options...��5      6/   5      5      5      5      