using System;
using System.Windows.Controls;
using System.Collections;
using Wpf.Ui.Controls;
using System.Windows;

namespace BinaryConverter.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : UiWindow 
    {
        private BitArray bitArray = new BitArray(8);

        public MainWindow()
        {
            InitializeComponent();


            for (int i = 0; i < bitArray.Length; i++)
            {
                int currentBit = i + 1;

                ColumnDefinition newCol =  new ColumnDefinition();
                windowGrid.ColumnDefinitions.Add(newCol);

                // Textbox
                Wpf.Ui.Controls.TextBox bitBox = new Wpf.Ui.Controls.TextBox();
                bitBox.PlaceholderText = "Bit " + currentBit.ToString();
                bitBox.Margin = new Thickness(5);
                bitBox.ClearButtonEnabled = false;
                bitBox.Text = "0";
                bitBox.SetValue(Grid.ColumnProperty, i);
                bitBox.SetValue(Grid.RowProperty, 1);
                bitBox.TextChanged += BitBox_BitChanged;

                windowGrid.Children.Add(bitBox);
            }
        }

        private void CalculateDecimal()
        {

        }

        private void BitBox_BitChanged(object sender, TextChangedEventArgs e)
        {
            Wpf.Ui.Controls.TextBox bitBox = (Wpf.Ui.Controls.TextBox)sender;
            string bitValue = bitBox.Text;

            if (int.TryParse(bitValue, out _) 
                && int.Parse(bitValue) >= 0
                && int.Parse(bitValue) <= 1)
            {
                int bit = Grid.GetColumn(bitBox);

                bitArray[bit] = bitValue == "0" ? false : true;
            }
            else
            {
                System.Windows.MessageBox.Show("Bit numbers must be 1 or 0", "Non numeric value");
                bitBox.Text = "0";
            }
        }
    }
}
