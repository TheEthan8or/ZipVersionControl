   M S B A M L   `   `   `  ����X  TZipVersionControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e9baa711da40215a5clr-namespace:ZipVersionControlZipVersionControl  5   	   +    %ZipVersionControl.ProfileActionWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:ZipVersionControl     5   	   -   5          �Name$  wdwProfileAction��5   	    � Title$ Please wait...��5   	   $��80.333��6    $	��300��60    � 
ResizeMode$ CanMinimize=�6<    � WindowStartupLocation$ CenterScreen=�6U    � SizeToContent$ WidthAndHeight=�6z   .��5	      � $��	0,0,10,10q�6    � ColumnDefinitions 5
   
   �� 5      .+�5   
   � -   5         �Name$ pgbProgress��5      $
��Left=�6+   $��10��6F   $��	10,31,0,0q�6R   $	��Top=�6e   $	��274��6}    � IsIndeterminate .  � Maximum$ 3Q�6�   	 � SmallChange$		 0.5Q�6�   �� 5   
   -   5        
 ��Name$
 	tblStatus��5      $
��Left=�6'   $
��Wrap=�6B   $	��Top=�6V   $��	10,10,0,0q�6n   $	��274��6�   5      5      