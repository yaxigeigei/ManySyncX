   M S B A M L   `   `   `  ���� D  @ManySyncX, Version=3.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:ManySyncX	ManySyncX      ManySyncX.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     )localclr-namespace:ManySyncX     -   $	��815��$	��360��$	��360��$	��815��$	��815��  � WindowStartupLocation$  CenterScreen=� � Icon$+ %/ManySyncX;component/Images/Icon1.ico�� � WindowStyle$
 None=� � AllowsTransparency .  � 
ResizeMode$ CanResizeWithGrip=���Z�  � 	Resources ,   &ManySyncX.EyeBooleanToOpacityConverter    EyeBooleanToOpacityConverter��%   &	          .���� -   $��windowFrame�����#395984$��1q� �� CornerRadius$ 	0,15,0,15��
�������._�� -   $��MainWindowGrid��.+��� -   $��BackgroundArrow��$+��%/ManySyncX;component/Images/Arrow.png��$	��595��$	��417��$
��Fill=�$
��Left=�$	��Top=�$��	0,0,0,-70q� J� Opacity$	 0.3Q��� -   $��LeftSideStackPanel��$��
20,120,0,0q�$
��Left=�$	��Top=�.+��� -   $��TasksStackPanel��$��3q� �� Orientation$ 
Horizontal=�.+��� -   $��	TasksList��$	��145��$	��140��$ 1Q�$��14(�$��Normal"���.  	 � ToolTip	 �� Organize your tasks here�� *(Each task has its OWN auto sync settings�� $��Bottom=�.+��� -   $��NewTask��$��New Task��$��30��$��65��$	��Top=�$
��Left=���.  ��Z� 	 �� Create a new task�� -	   $��
RenameTask��$��Rename��$��65��$��30��$	��Top=�$
��Left=���.  ��Z� 	 �� Rename the selected task�� -
   $��
RemoveTask��$��Remove��$��30��$��65��$	��Top=�$
��Left=���.  ��Z� 	 �� &$Remove the selected task permanently�� -   $��EnableWatchCheckbox��$��3,3,0,0q�$�� Allow Auto Sync��
 d� 	IsChecked
 �� *enableWatch+ �� Mode$ TwoWay=�	 �� �� $8�2The global switch of auto sync (affects all tasks)���� -   $��TaskListCoverLabel��$	��   ��$	��190(�$
��Left=�$	��179��$��
10,100,0,0q�$	��Top=�$	��145��$y�	Collapsed=��� -   $��ModeTab��$	��179��$	��550��$��
0,120,16,0q�$	��Top=�$��Right=���Z� ��Z� .@��� -   $��
One2OneTab��$��Sync Pairs       ��$��-2,-2,-7,-1q�$
��Left=�$	��105����Z� .��� -   $��
o2oTabGrid��$��-6,0,0,0q� J� 	AllowDrop .  � RowDefinitions �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� -   $��o2oStackPanel0��$��0q�$ 
Horizontal=�$��0��.+��� -    ��Name$ sourceDirText1��$��2q�$��Center=�$	��247��$��26���� $0��*/ManySyncX;component/Images/SmallArrow.png��$��5,9,3,9q�$
 0.75Q�$��12���� -   $ targetDirText1��$��2q�$��Center=�$	��247��$��26���� -    ��Name$ 
SwapImage1��$��Uniform=�$*��$/ManySyncX;component/Images/Swap.png��$	��6,4q�$	 0.7Q� .  $��13��$��13��	 �� �� $7�1Swap paths between source and target. Be careful!���� -   $��o2oStackPanel1��$��0q�$ 
Horizontal=�$��1��.+��� -   $ sourceDirText2��$��2q�$��Center=�$	��247��$��26���� $0��*/ManySyncX;component/Images/SmallArrow.png��$��5,9,3,9q�$
 0.75Q�$��12���� -   $ targetDirText2��$��2q�$��Center=�$	��247��$��26���� -   $ 
SwapImage2��$	��6,4q�$*��$/ManySyncX;component/Images/Swap.png��$��Uniform=�$	 0.7Q� .  $��13��$��13��	 �� �� $7�1Swap paths between source and target. Be careful!���� -   $��o2oStackPanel2��$��0q�$ 
Horizontal=�$��2��.+��� -   $ sourceDirText3��$��2q�$��Center=�$	��247��$��26���� $0��*/ManySyncX;component/Images/SmallArrow.png��$��5,9,3,9q�$
 0.75Q�$��12���� -   $ targetDirText3��$��2q�$��Center=�$	��247��$��26���� -   $ 
SwapImage3��$	��6,4q�$*��$/ManySyncX;component/Images/Swap.png��$��Uniform=�$	 0.7Q� .  $��13��$��13��	 �� �� $7�1Swap paths between source and target. Be careful!���� -   $��o2oStackPanel3��$��0q�$ 
Horizontal=�$��3��.+��� -   $ sourceDirText4��$��2q�$��Center=�$	��247��$��26���� $0��*/ManySyncX;component/Images/SmallArrow.png��$��5,9,3,9q�$
 0.75Q�$��12���� -   $ targetDirText4��$��2q�$��Center=�$	��247��$��26���� -   $ 
SwapImage4��$	��6,4q�$*��$/ManySyncX;component/Images/Swap.png��$��Uniform=�$	 0.7Q� .  $��13��$��13��	 �� �� $7�1Swap paths between source and target. Be careful!���� -    $��o2oStackPanel4��$��0q�$ 
Horizontal=�$��4��.+��� -!   $ sourceDirText5��$��2q�$��Center=�$	��247��$��26���� $0��*/ManySyncX;component/Images/SmallArrow.png��$��5,9,3,9q�$
 0.75Q�$��12���� -"   $ targetDirText5��$��2q�$��Center=�$	��247��$��26���� -#   $ 
SwapImage5��$	��6,4q�$*��$/ManySyncX;component/Images/Swap.png��$��Uniform=�$	 0.7Q� .  $��13��$��13��	 �� �� $7�1Swap paths between source and target. Be careful!���� -$   $	��Eye��$��24��$��0,120,468,0q�$	��Top=�$+��%/ManySyncX;component/Images/Watch.png��$��Right=�$��24�� �� *
o2oWatch+ �� 	Converter# [  $ TwoWay=�	 �� (&Turn on/off the auto sync of this task9� -%   $��	expander1��$�� Records�� 9� 
IsExpanded .  $
��Bold"�$��
10,330,0,0q���.  .��� -&   $��grid1��.+��� -'   $��RecordListBox��$��-11,5,-1,-1q�$	 0.8Q��� -(   $��	ExpRecord��$��Summary��$��23��$��	0,15,25,0q�$	��Top=�$��Right=�$��80��
���������.  	 �� 86Show statistics of the latest preview or sync (if any)�� -)   $��ClearRecordsButton��$��Clear��$��23��$��
0,15,120,0q�$��Right=�$	��Top=�$��80��
���������.  	 �� Clear all lines shown here�� -*   $��WindowControlStackPanel��$��0,5,7,0q�$	��Top=�$��Right=�$ 
Horizontal=�.+��� -+   $��MinimizeButton��$	��—��$��25��$��0q�$��25��$
��Bold"���.  ��Z� ��Z� 	 �� Minimize the window�� -,   $��Minimize2TrayButton��$��V��$
��Bold"�$��25��$��0q�$��25����.  ��Z� ��Z� 	 �� !Hide the program to system tray�� --   $��CloseButton��$��X��$��25��$��25��$��0q�$
��Bold"���.  ��Z� ��Z� 	 �� Exit�� -.   $��currentTaskLabel��$��45(�$��Normal"�$��	ManySyncX��$	��Top=�$��	22,28,0,0q�$��0q�$
��Left=�$	��Top=�����  �� EndPoint$ 1,0.5&� �� 
StartPoint$ 0,0.5&�.���  � Color$ Black�� � Offset$ 0Q�� $ White��$ 1Q�� $ 	#FF7F7F7F��$ 0.934Q��� $��30��$��	0,58,30,0q�$��Right=�$	��Top=�$ 
Horizontal=�.+��� -/   $��settingsButton��$��Settings��$��16(�$
��Bold"�$��85��$��0,0,15,0q�
���**����.  ��Z� �� -0   $��Analyze��$��Preview��$��16(�$��0,0,15,0q�$��95��$
��Bold"���.  ��Z� �� -1   $
��Sync��$��Synchronize��$��16(�$
��Bold"�$	��120����.  ��Z� �� -2   $��basedSyncCheckBox��$��0,92,15.8,0q�$��Based on Preview Result��$��10(�$	��Top=�$��Right=��� $��24��$��
0,320,20,0q�$��Right=�$	��Top=�$ 
Horizontal=�.+��� -3   $��aboutButton��$��65��$��About�����	#FF1592C3$��Center=���Z� 