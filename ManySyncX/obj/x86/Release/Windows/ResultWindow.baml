   M S B A M L   `   `   `  ����   	ManySyncX    ManySyncX.ResultWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  ResultWindow��$	��170��$	��700��$	��500�� � WindowStartupLocation$ CenterScreen=�$	��170�� � WindowStyle$
 None=� � AllowsTransparency .  � 
ResizeMode$ CanResizeWithGrip=���Z� .���� -   $��WindowBorder��$��1q�
���   � �� CornerRadius$ 	0,15,0,15��
�������._�� .+��� -    ��Name$ ListBox1��$��15,160,15,15q�$
��Bold"�� -    �Name$ NumbersGrid��$��50��$��
15,60,15,0q�$	��Top=� � RowDefinitions �� $��1* ��� $��1* �	 � ColumnDefinitions	 �� $��1* ��� $��1* ��� $��1* �.+��� -   
 ��Name$
 FileAddLabel��$��# Files Copied��$��0��$��0��$��0q�$��Center=�$��Center=�$
��Bold"�
��� ���
�������$��1q�$
��-0.5q��� -   $
 FileDelLabel��$��# Files Deleted��$��1��$��0��$��0q�$��Center=�$��Center=�$
��Bold"�
���<��
�������$��1q�$
��-0.5q��� -   $
 FileUnchangeLabel��$��# Files Unchanged��$��1��$��0q�$��Center=�$��Center=�$��1��$
��Bold"�
����h{�
�������$��1q�$
��-0.5q��� -   $
 FileUpdateLabel��$��# Files Updated��$��1��$��1��$��0q�$��Center=�$��Center=�$��1��$
��Bold"�
��� � �
�������$��1q�$
��-0.5q��� -	   $
 ItemSkipLabel��$��# Items Skipped��$��Center=�$��0q�$��Center=�$��2��$
��Bold"�
�������
�������$��1q�$
��-0.5q��� -
   $
 ItemFailLabel��$��# Items Failed��$��Center=�$��0q�$��Center=�$��2��$��1��$
��Bold"�
�������$��1q�$
��-0.5q��� -    ��Name$ ExpRecordsButton��$��Export Records��$��
0,125,90,0q�$��23��$	��Top=�$
��Bold"�$��Right=�$	��120����Z�  � ToolTip �� �� $=�7Save records to a TXT file in ...\My Document\ManySyncX���� -   $
 label1��$(��"Click a category above for details��$��26��$��
16,118,0,0q�$	��Top=�
�������$
��Left=�$��12(��� -    ��Name$ ColorBarBorder��$��
15,15,15,0q�$	��Top=�$��30��
���iii�$	��0.5q�._�� -   $ ColorBarGrid��	 �� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* ��� $��1* �.+��� -    ��Name$ FileAddRect��$��0������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$
 Gold�� � Offset$ 0Q�� $ 	#FFAAA816��$ 1Q��� -   $ FileDelRect��$��1������ $ 0.5,1&�$ 0.5,0&�.��� $	 Red��$ 0Q�� $ DarkRed��$ 1Q��� -   $ FileUpdateRect��$��2������ $ 0.5,1&�$ 0.5,0&�.��� $ 	LimeGreen��$ 0Q�� $ SeaGreen��$ 1Q��� -   $ FileUnchangeRect��$��3������ $ 0.5,1&�$ 0.5,0&�.��� $ LightSkyBlue��$ 0Q�� $ MediumSlateBlue��$ 1Q��� -   $ FileSkipRect��$��4������ $ 0.5,1&�$ 0.5,0&�.��� $ 
WhiteSmoke��$ 0Q�� $ DarkGray��$ 1Q��� -   $ 
rectangle1��$��5������ $ 0.5,1&�$ 0.5,0&�.��� $ DimGray��$ 0Q�� $ Black��$ 1Q��� -   $ button1��$��Close��$
��Bold"�$��23��$��Right=�$��
0,125,15,0q�$	��Top=�$��60����Z�  �� �� $=�7Save records to a TXT file in ...\My Document\ManySyncX��