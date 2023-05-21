using System;
using System.Windows.Controls;
using System.Collections;
using Wpf.Ui.Controls;
using System.Windows;
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
        private int MaxBitSize;
        private BitArray? BitArray = null;
        private double LargestBit = 0;

        public MainWindow()
        {
            InitializeComponent();
            // Start with 8 bits
            MaxBitSize = 8;
            CreateBitGrid();
        }

        #region Global Methods
        private void CreateBitGrid(BitArray bitArr = null)
        {
            bitGrid.Children.Clear();

            // TODO: This could be refactored
            if (bitArr != null)
            {
                BitArray = bitArr;
            }
            else
            {
                BitArray = new BitArray(MaxBitSize);
            }

            LargestBit = Math.Pow(2, BitArray.Length) / 2;

            double decrement = LargestBit;
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

            foreach (bool bit in BitArray)
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
            double increment = LargestBit;

            for (int i = 0; i < BitArray.Length; i++)
            {
                // If bit = 1 add bit decimalValue to total
                if (BitArray[i] == true)
                {
                    decimalValue += increment;
                }

                // Divide increment by two after each bit
                increment /= 2;
            }

            txtDecValue.Text = decimalValue.ToString();
        }

        private void UpdateBitBoxes(string binaryString)
        {
            bitSlider.Value = binaryString.Length;
            BitArray binaryNum = new BitArray(binaryString.Length);
            CreateBitGrid(binaryNum);

            for (int i = 0; i < bitGrid.Children.Count; i++)
            {
                StackPanel e = (StackPanel) bitGrid.Children[i];
                Wpf.Ui.Controls.TextBox bitBox = (Wpf.Ui.Controls.TextBox)e.Children[1];

                if (binaryString[i] == '1')
                {
                    bitBox.Text = "1";
                }
                else if (binaryString[i] == '0')
                {
                    bitBox.Text = "0";
                }
            }
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
            else
            {
                // Larger bits
                return "0000000000000000";
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

            return new string(binaryString).Replace("\0", "0");
        }

        private bool PasteBinary(string binaryString)
        {
            if (binaryString.Length % 8 == 0 && binaryString.Length >= 8)
            {
                UpdateBitBoxes(binaryString);
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
        private bool cDown = false;
        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Handle CTRL + V  & CTRL + C for pasting binary numbers
            if(ctrlDown && vDown)
            {
                ctrlDown = false;
                vDown = false;

                if(!PasteBinary(Clipboard.GetText(TextDataFormat.Text)))
                {
                    System.Windows.MessageBox.Show("Invalid binary number pasted", "Invalid input");
                }
            }
            if (ctrlDown && cDown)
            {
                ctrlDown = false;
                cDown = false;

                string binaryString = "";

                foreach (bool bit in BitArray)
                {
                    if(bit)
                    {
                        binaryString += "1";
                    }
                    else
                    {
                        binaryString += "0";

                    }
                }

                System.Windows.MessageBox.Show("Binary number copied !", "Copied to clipboard");
                Clipboard.SetData(DataFormats.Text, binaryString);
            }
            else
            {
                // Detect CTRL
                if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
                {
                    ctrlDown = true;
                }

                if (e.Key == Key.V)
                {
                    vDown = true;
                }

                if (e.Key == Key.C)
                {
                    cDown = true;
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

                BitArray[bit] = bitValue == "1" ? true : false;

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
            MaxBitSize = (int)slider.Value;
            
            if(bitGrid != null)
            {
                CreateBitGrid();
            }
        }
        private void btnDecToBin_Click(object sender, RoutedEventArgs e)
        {
            string text = txtDecValue.Text;

            if (int.TryParse(text, out _) && int.Parse(text) != 0)
            {
                PasteBinary(CalculateBinary(int.Parse(text)));
            }
        }
        #endregion
    }
}
