   M S B A M L   `   `   `  ����   	ManySyncX    ManySyncX.SettingWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Advanced Settings��$	��380��$	��700�� � WindowStartupLocation$ CenterOwner=� � AllowsTransparency .  � WindowStyle$
 None=� � 
ResizeMode$ CanResizeWithGrip=���Z� .��� .+��� 
���   �
�������$��1q� �� CornerRadius$ 	0,20,0,20��._�� .+��� -   $��WindowControlStackPanel��$��	0,15,25,0q�$	��Top=�$��Right=� �� Orientation$ 
Horizontal=�.+��� -   $��ConfirmButton��$��Confirm��$��25��$��85��$��0q�$��14(�$
��Bold"���.  ��Z� ��Z�  � ToolTip �� Save and Close�� -   $��CloseButton��$��Cancel��$��25��$��85��$��0q�$��14(�$
��Bold"���.   J� RenderTransformOrigin$ 0.5,0.5&���Z� ��Z�  �� Close without Saving�� $��20,30,20,20q�.@��� $
��Main��.��� 	 � ColumnDefinitions	 �� $��1* ��� $��1* �.+��� $��0��$��0��$��10q�.+��� $��Rules of Synchronization��.���� $
��5,12q�.+��� -   $��copyNewCheckBox��$!�� Copy new files and folders��$��3q��� -   $��replaceOldCheckBox��$�� Replace older files��$��3q��� -   $��deleteNonCheckBox��$+��% Delete non-existing files or folders��$��3q��� -   $��recycleCheckBox��$"�� Delete files to recycle bin��$��3q��� -	   $��enableExcludeCheckbox��$��3q�$�� Enable Exclusion List�� �� �� $)�#For details see 'Advanced Settings'���� -
   $��enableEditorCheckbox��$��3q�$�� Enable Editor's List�� �� �� $)�#For details see 'Advanced Settings'���� -   $��forceReplaceCheckBox��$,��& (Caution) Force replacing newer files��$��3q��� $��User Interface Style��.���� $��5q�.+��� -   $��ghostCheckBox��$�� Ghost mode��$��3q��� $��1��$��10q�.+��� $��Automatic Synchronization��.���� $��3q�.+��� -   $��FreqComboBox��$��Normal"�$	��0,5q���Z� �� -   $��label1��$5��/Click the 'Eye' to enable or disable auto sync ��$��20��$��Normal"�$��0q��� -   $��label2��$)��#of THIS task (one to one mode only)��$��20��$��Normal"�$��0q�� 
 � RowDefinitions
 �� $��7* ��� $��1* �	 �� $��5* ��� $��3* �.+��� $��1��$��10q�.+��� $��Date of the��$
��Bold"�$��12(�$��0q��� $��Latest Sync��$
��Bold"�$��12(�$��0q�&   System.Windows.Controls.Calendar  -   $��Calendar��$��5q�$
��Left=�   IsTodayHighlighted .  $��2���� -   $��prefDefaultButton��$��Default��$��1��$��1��$��75��$��23��$��10q�$��Bottom=��� $��Exclusion List��.��� 
 �� $��3* ��� $��8* �	 �� $��1* ��� $��1* �.+��� -   $!��excludeDiscriptionTextBlock��$��0��$��2��$��6q�$
��Wrap=��� .�What is Exclusion List?�� ��If you need to skip or ignore certain folders or files, please add them (no matter in source or target side, with full paths, wildcards expressions, or regular expressions) into this list. As a result, these items will not be copied, updated or deleted.�� -   $��excludeListBox��$��0��$��1��$��5q�$��2���� $��1��$��1��$��5q�$��2��.+��� -   $��excludeFileTextBox��$��23��$(��"Double click here to select a file���� -   $��excludeAddFileButton��$��Add to list��$��85��$	��0,5q�$��Right=��� -   $��excludeFolderTextBox��$��23��$*��$Double click here to select a folder���� -   $��excludeAddFolderButton��$��Add to list��$��85��$	��0,5q�$��Right=��� -   $��excludeRemoveButton��$��Remove��$��85��$	��0,6q�$
��Left=��� -   $��excludeExampleTextBlock��$��8(�$	��0,3q�
Examples :��  D:\Softwares\Lab sharing\Games�� -+M:\Family joy center\photos\Mama No See.jpg�� *\2014????\experiment_*.tif�� [^\s]+(\.(?i)(mp3|acc|wma))$�� $��Editor's List��.��� 	 �� $��4* ��� $
��0.7* ��� $	��10* ��� $
��0.7* ��� $��4* �
 �� $
��1.7* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� -   $ ��editorDiscriptionTextBlock��$��0��$��5,5,4.8,4.6q�$��5��$
��Wrap=��� .�What is Editor's List?�� ��If you are writing or editing something, especially in a cooprative way, you probably want to sync the latest file into certain folder in which previous version can also be preserved. This can be done by adding the file into this list and chose a rule of tagging (i.e. prefix and/or suffix).�� -   $��prefixComboBox��$��1��$��0��$��23��$��6q� �� SelectedIndex$ 0��.@��� $
��None���� $��Date & Time���� $��Counting Up���� $��1��$��1��$��+��$��Center=�$��Center=��� -   $��editorPathTextBox��$��1��$��2��$(��"Double click here to select a file��$��23��$��6q��� $��1��$��3��$��+��$��Center=�$��Center=��� -   $��suffixComboBox��$��1��$��4��$��23��$��6q�$ 0��.@��� $
��None���� $��Date & Time���� $��Counting Up���� -   $��editorAddButton��$��2��$��4��$��6q�$��Add To List���� -   $��editorRemoveButton��$��3��$��4��$��6q�$��Remove���� -    $��editorListBox��$��2��$��3��$��4��$��	5,4.8,5,5q��� $��5��$��0��$��5��$ 
Horizontal=�.+��� $��	Preview :��$��15(�$
��Bold"�$
��Left=�$	��6,0q�$��Center=��� -!   $��PreviewLabel��$��15(�$
��Bold"�$0��*Year-Month-Day HrMinSec__FileName__Counter��$
��Left=�$	��6,0q�$��Center=�