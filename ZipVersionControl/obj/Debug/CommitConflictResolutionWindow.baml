   M S B A M L   `   `   `  ����X  TZipVersionControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e9baa711da40215a5clr-namespace:ZipVersionControlZipVersionControl  5   	   6    0ZipVersionControl.CommitConflictResolutionWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:ZipVersionControl     5   	     � Title$  Resolve conflict��5   	   $	��238��6"   $��300         ��6/    � SizeToContent$ WidthAndHeight=�5	       � WindowStartupLocation$ CenterScreen=�6*    � 
ResizeMode$ CanMinimize=�6O   .��5
      � $��	0,0,10,10q�6   .+�5   
   �� $
��Left=�6   $��	10,10,0,0q�6/   $
��Wrap=�6B   $	��Top=�6V   �� 6o   $]�WBoth the git repository and the zip file has been changed outside of ZipVersionControl.��6s   �� 6�   �� 6�   �� 6�   �� 6�   $;�5Which files would you like ZipVersionControl to keep?��6�   66  �� 5   
   -   5        $��Files in git repository��5      $
��Left=�63   $	��Top=�6N   $	��274��6f   $��	10,95,0,0q�6r   6�   �� 5   
   -   5         ��Name$ btnZipFileFiles��5      $��Files in zip file��6*   $
��Left=�6F   $��
10,120,0,0q�6a   $	��Top=�6u   $	��274��6�   6�   �� 5   
   -   5        $��Files in both��5      $
��Left=�6)   $��
10,145,0,0q�6D   $	��Top=�6X   $	��274��6p   6|   �� 5   
   -   5        $��Abort operation��5      $
��Left=�6+   $��
10,180,0,0q�6F   $	��Top=�6Z   $	��274��6r    �� IsCancel . 6�   5      5      