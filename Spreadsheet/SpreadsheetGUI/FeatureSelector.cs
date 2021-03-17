using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{
    public partial class FeatureSelector : Form
    {
        public FeatureSelector(string operationName)
        {
            InitializeComponent();
            operationLabel.Text = operationName;
        }

        public static Tuple<string, string, string> Show(string operationName)
        {
            using(FeatureSelector selector = new  FeatureSelector(operationName))
            {
                selector.Show();

                bool clicked = false;

                selector.finishButton.Click += (object sender, EventArgs e) => clicked = true;

                while (!clicked) { }

                Tuple<string, string, string> output = new Tuple<string, string, string>(
                    selector.upperLeftBox.Text,
                    selector.lowerRightBox.Text,
                    selector.outputCellBox.Text);

                return output;
            }
        }
    }
}
