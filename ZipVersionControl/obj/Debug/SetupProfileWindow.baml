   M S B A M L   `   `   `  ����X  TZipVersionControl, Version=1.0.0.0, Culture=neutral, PublicKeyToken=e9baa711da40215a5clr-namespace:ZipVersionControlZipVersionControl  5   	   *    $ZipVersionControl.SetupProfileWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:ZipVersionControl     5   	   -   5          �Name$  wdwSetupProfile��5   	   $��201.856��5      $	��300��6+    � SizeToContent$ WidthAndHeight=�67    � WindowStartupLocation$ CenterScreen=�6V   .��5      � $��	0,0,10,10q�6   .+�5	   
   �� $
��Left=�6   $
��Wrap=�6/   $	��Top=�6C   $��	10,10,0,0q�6[   $��Profile Name��6n   �� 5
   
   -   5         ��Name$ txtProfileName��5
      $
��Left=�6*   $��23��6E   $��	10,26,0,0q�6Q   $	��Top=�6d   $	��272��6|   $��Center=�6�   
�������
���   �6�   �� 5   
   -   5        $��	Choose...��5      $
��Left=�6%   $��208,114,0,0q�6@   $	��Top=�6U   $��74��6m   ���	#FFABADB3$��23��6�   �� 5   
   -   5         ��Name$ btnDone��5      $
��Done��6"   $
��Left=�61   $��127,142,0,0q�6L   $	��Top=�6a   $��75��6y    �� 	IsDefault . 6�   �� 5   
   -   5        $��Cancel��5      $
��Left=�6"   $��207,142,0,0q�6=   $	��Top=�6R   $��75��6j    �� IsCancel . 6�   �� 5   
   $
��Left=�6   $��	10,54,0,0q�6/   $
��Wrap=�6B   $	��Top=�6V   $��Zip File��6n   �� 5   
   -   5        $ 
txtZipFile��5      $
��Left=�6&   $��23��6A   $��	10,70,0,0q�6M   $	��Top=�6`   $	��199��6x    �� 
IsReadOnly . $��Center=�6�   �� 5   
   -   5        $��	Choose...��5      $
��Left=�6%   $��
208,70,0,0q�6@   $	��Top=�6T   $��74��6l   $��23��6�   ���	#FFABADB36�   �� 5   
   $
��Left=�6   $��	10,98,0,0q�6/   $
��Wrap=�6B   $��Local Git Repository��6V   $	��Top=�6r   �� 5   
   -   5        $ txtGitRepository��5      $
��Left=�6,   $��23��6G   $��
10,114,0,0q�6S   $	��Top=�6g   $	��199��6    . $��Center=�6�   �� 5   
   -	   5        $ btnRemoveProfile��5      $��Remove Profile��6+   $
��Left=�6D   $��
10,142,0,0q�6_   $	��Top=�6s   $	��112��6�   6�   5      5      