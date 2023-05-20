using System;
using System.Windows.Controls;
using System.Collections;
using Wpf.Ui.Controls;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Input;
using System.Linq;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Text;

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
        private void CreateBitGrid(BitArray bitArr = null)
        {
            bitGrid.Children.Clear();

            // TODO: This could be refactored
            if (bitArr != null)
            {
                bitArray = bitArr;
            }
            else
            {
                bitArray = new BitArray(maxBitSize);
            }

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

        private List<Wpf.Ui.Controls.TextBox> FindBitBoxes()
        {
            List<Wpf.Ui.Controls.TextBox> bitBoxes = new List<Wpf.Ui.Controls.TextBox>();
            IEnumerable<StackPanel> bitPanels = bitGrid.Children.OfType<StackPanel>();

            foreach (StackPanel panel in bitPanels)
            {
                Wpf.Ui.Controls.TextBox bitBox = panel.Children.OfType<Wpf.Ui.Controls.TextBox>().ToList()[0];
                bitBoxes.Add(bitBox);
            }

            return bitBoxes;
        }

        private string CalculateBinary(int decNum)
        {
            int bitSize = 0;

            if (decNum <= byte.MaxValue)
            {
                // 8 Bit
                bitSize = 8;
            }
            else if (decNum <= ushort.MaxValue)
            {
                // 16 Bit 
                bitSize = 16;

            }

            char[] binaryString = new char[bitSize];
            int count = 1;

            // 8 bit
            while (decNum != 0)
            {
                ValueTuple<nint, nint> divD = Math.DivRem(decNum, 2);
                decNum = (int)divD.Item1;
                binaryString[binaryString.Length-count] = (int)divD.Item2 == 1 ? '1' : '0';
                count++;
            }

            return new string(binaryString);
        }

        private bool PasteBinary(string binaryString)
        {
            binaryString = binaryString.Replace("\0", "0");

            if (binaryString.Length % 8 == 0 && binaryString.Length >= 8)
            {
                BitArray binaryNum = new BitArray(binaryString.Length);
                List<Wpf.Ui.Controls.TextBox> bitBoxes = FindBitBoxes();

                for (int i = 0; i < binaryString.Length; i++)
                {
                    // TODO: Need to find Bitbox for each bit and then update text to binary value 
                    if (binaryString[i] == '1')
                    {
                        binaryNum[i] = true;
                        bitBoxes[i].Text = "1";
                    }
                    else if (binaryString[i] == '0')
                    {
                        binaryNum[i] = false;
                        bitBoxes[i].Text = "0";
                    }
                    else
                    {
                        return false;
                    }
                }

                //CalculateDecimal();

                bitArray = binaryNum;
                CreateBitGrid();
                return true;
            }
            else
            {
                return false;
            }
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

                if(!PasteBinary(Clipboard.GetText(TextDataFormat.Text)))
                {
                    System.Windows.MessageBox.Show("Invalid binary number pasted", "Invalid input");
                }
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
            {
                CreateBitGrid();
            }
        }

        private void txtDecValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            string text = txtDecValue.Text;

            if (int.TryParse(text, out _) && int.Parse(text) != 0 && txtDecValue.IsFocused)
            {
                PasteBinary(CalculateBinary(int.Parse(text)));
            }
        }
        #endregion
    }
}
