   M S B A M L   `   `   `  ����X  TZipVersionControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e9baa711da40215a5clr-namespace:ZipVersionControlZipVersionControl  5   	   +    %ZipVersionControl.CommitMessageWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:ZipVersionControl     5   	     � Title$  Commit changes��5   	   $��96.509��6    $	��300��60    � WindowStartupLocation$ CenterScreen=�6<    � 
ResizeMode$ CanMinimize=�6a    � SizeToContent$ WidthAndHeight=�6z   .��5	      � $��	0,0,10,10q�6   .+�5
   
   �� -   5         ��Name$ txtCommitMessage��5
      $
��Left=�6,   $��23��6G   $��	10,10,0,0q�6S   $	��Top=�6f   $	��274��6~    �� 	IsEnabled . $��Center=�6�   �� 5   
   -   5         ��Name$ 	btnCommit��5      $��Commit��6$   $
��Left=�65   $��	10,38,0,0q�6P   $	��Top=�6c   $��75��6{    �� 	IsDefault . {�.  6�   �� 5   
   -   5        $��Abort��5      $
��Left=�6!   $��
209,38,0,0q�6<   $	��Top=�6P   $��75��6h    �� IsCancel . 6�   �� 5   
   -   5        $ btnNoCommit��5      $��Don't commit��6&   $
��Left=�6=   $��	90,38,0,0q�6X   $	��Top=�6k   $	��114��6�   6�   5      5      