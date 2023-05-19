using System;
using System.Windows.Controls;
using System.Collections;
using Wpf.Ui.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;

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
        private int maxBitSize;
        private BitArray? bitArray = null;
        private double largestBit = 0;

        public MainWindow()
        {
            InitializeComponent();
            maxBitSize = 8;
            CreateBitGrid();
        }

        #region Global Methods
        private void CreateBitGrid()
        {
            bitGrid.Children.Clear();

            bitArray = new BitArray(maxBitSize);
            largestBit = Math.Pow(2, bitArray.Length) / 2;

            double decrement = largestBit;
            int rowItemMax = 8;

            // This code will automatically create a new row
            // for a specified amount of items per row.

            int itemCount = 0;
            int cols = 0;
            int rows = 0;
            int bits = 0;

            // Create first initial row
            bitGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Create columns
            for (int i = 0; i < rowItemMax; i++)
            {
                bitGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }

            foreach (bool bit in bitArray)
            {
                // Using a square as an example of a UI element.
                StackPanel bitPanel = new StackPanel();
                bitPanel.HorizontalAlignment = HorizontalAlignment.Center;
                bitPanel.SetValue(Grid.RowProperty, 1);

                bitGrid.ColumnDefinitions.Add(new ColumnDefinition());
                bitPanel.SetValue(Grid.ColumnProperty, bits);

                // Label
                Label label = new Label { Content = decrement.ToString(), HorizontalAlignment = HorizontalAlignment.Center };
                decrement /= 2;

                bitPanel.Children.Add(label);

                // Textbox
                Wpf.Ui.Controls.TextBox bitBox = new Wpf.Ui.Controls.TextBox();
                bitBox.Name = "b" + bits;
                bitBox.Margin = new Thickness(10, 5, 10, 10);
                bitBox.ClearButtonEnabled = false;
                bitBox.Text = "0";
                bitBox.PreviewMouseDown += BitBox_Click;
                bitBox.TextChanged += BitBox_BitChanged;

                bitPanel.Children.Add(bitBox);
                bitGrid.Children.Add(bitPanel);

                bitPanel.SetValue(Grid.RowProperty, rows);
                bitPanel.SetValue(Grid.ColumnProperty, cols);

                cols++;
                itemCount++;
                bits++;

                // Every time rowItemMax is equal to itemCount
                // Create a new row and reset column count
                if (itemCount >= rowItemMax && itemCount % rowItemMax == 0)
                {
                    bitGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                    rows++;
                    cols = 0;
                }
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
        
        private void PasteBinary()
        {

        }
        #endregion

        #region UI Events
        private void UiWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.KeyDown += MainWindow_KeyDown;
        }


        private bool ctrlDown = false;
        private bool vDown = false;
        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if(ctrlDown && vDown)
            {
                ctrlDown = false;
                vDown = false;

                Debug.WriteLine(Clipboard.GetText(TextDataFormat.Text));
            }
            else
            {
                // Detect CTRL + V
                if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                {
                    ctrlDown = true;
                }

                if (e.Key == Key.V)
                {
                    vDown = true;
                }
            }
        }

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
                int bit = int.Parse(bitBox.Name.Replace("b", ""));

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
            maxBitSize = (int)slider.Value;
            
            if(bitGrid != null)
                CreateBitGrid();
        }

        #endregion

    }
}
