﻿using System;
using System.Windows.Controls;
using System.Collections;
using Wpf.Ui.Controls;
using System.Windows;
using System.Diagnostics;

namespace BinaryConverter.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow 
    {
        // bitArray[0] = 128
        // ...
        // bitArray[7] = 1
        private int bitSize = 8;
        private BitArray? bitArray = null;
        private double largestBit = 0;
        private Grid bitGrid = new Grid();

        public MainWindow()
        {
            InitializeComponent();

            windowGrid.Children.Add(bitGrid);
            CreateBitGrid();
        }

        #region Global Methods
        private void CreateBitGrid()
        {
            bitGrid.Children.Clear();
            bitGrid.ColumnDefinitions.Clear();

            bitArray = new BitArray(bitSize);
            largestBit = Math.Pow(2, bitArray.Length) / 2;

            double decrement = largestBit;
            short rowCount = 0;

            for (int j = 0; j < bitArray.Length; j++)
            {
                StackPanel bitPanel = new StackPanel();
                bitPanel.SetValue(Grid.RowProperty, 1);

                bitGrid.ColumnDefinitions.Add(new ColumnDefinition());
                bitPanel.SetValue(Grid.ColumnProperty, j);

                int currentBit = j + 1;

                // Label
                Label label = new Label { Content = decrement.ToString(), HorizontalAlignment = HorizontalAlignment.Center };
                decrement /= 2;

                bitPanel.Children.Add(label);

                // Textbox
                Wpf.Ui.Controls.TextBox bitBox = new Wpf.Ui.Controls.TextBox();
                bitBox.Margin = new Thickness(10, 5, 10, 10);
                bitBox.ClearButtonEnabled = false;
                bitBox.Text = "0";
                bitBox.PreviewMouseDown += BitBox_Click;
                bitBox.TextChanged += BitBox_BitChanged;

                bitPanel.Children.Add(bitBox);
                bitGrid.Children.Add(bitPanel);

            }

            bitGrid.SetValue(Grid.RowProperty, 2);
        }

        private void CalculateDecimal()
        {
            double decimalValue = 0;
            double increment = largestBit;

            for (int i = 0; i < bitArray.Length; i++)
            {
                // If bit = 1 add bit decimalValue to total
                if (bitArray[i] == true)
                {
                    decimalValue += increment;
                }

                // Divide increment by two after each bit
                increment /= 2;
            }

            txtDecValue.Text = decimalValue.ToString();
        }
        #endregion

        #region UI Events
        private void BitBox_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Wpf.Ui.Controls.TextBox bitBox = (Wpf.Ui.Controls.TextBox)sender;
            bitBox.SelectAll();
        }

        private void BitBox_PreviewGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            Wpf.Ui.Controls.TextBox bitBox = (Wpf.Ui.Controls.TextBox)sender;
            bitBox.SelectAll();
        }

        private void BitBox_BitChanged(object sender, TextChangedEventArgs e)
        {
            Wpf.Ui.Controls.TextBox bitBox = (Wpf.Ui.Controls.TextBox)sender;
            string bitValue = bitBox.Text;

            // Integer validation and validation of binary digit 
            if (int.TryParse(bitValue, out _) 
                && int.Parse(bitValue) >= 0
                && int.Parse(bitValue) <= 1)
            {
                // bitBox parent stack panel column number is equivalent to bit.
                int bit = Grid.GetColumn((StackPanel)bitBox.Parent);

                bitArray[bit] = bitValue == "1" ? true : false;

                CalculateDecimal();
            }
            else
            {
                System.Windows.MessageBox.Show("Bit numbers must be 1 or 0", "invalid value");
                bitBox.Text = "0";
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = (Slider)sender; 

            lblBits.Text = "Bits " + slider.Value;
            bitSize = (int)slider.Value;
            CreateBitGrid();
        }

        #endregion
    }
}
