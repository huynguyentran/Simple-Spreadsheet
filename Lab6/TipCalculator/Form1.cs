using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace TipCalculator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void ComputeTip()
        {
            string billStr = totalBillBox.Text;

            if (Double.TryParse(billStr, out double d))
            {
                if (Double.TryParse(tipBox.Text, out double tip))
                {
                    double tipTotal = d * (tip/100d);
                    billStr = tipTotal.ToString();
                    totalWithTipBox.Text = (d + tipTotal).ToString();
                }
                else
                {
                    billStr = "Invalid Tip";
                }
            }
            else
                billStr = "Invalid Bill";

            bottomTextBox.Text = billStr;
        }

        private void totalBillBox_TextChanged(object sender, EventArgs e)
        {
            UpdateInput("bill", Regex.IsMatch(totalBillBox.Text, @"^[\d]+(.[\d]{1,2})?$"));
        }

        private void tipBox_TextChanged(object sender, EventArgs e)
        {
            UpdateInput("tip", Regex.IsMatch(tipBox.Text, @"^[\d]{1,2}$"));
        }

        private Dictionary<string, bool> inputsValid;

        private void UpdateInput(string inputName, bool status)
        {
            if (ReferenceEquals(inputsValid, null))
            {
                inputsValid = new Dictionary<string, bool>();
                inputsValid["bill"] = false;
                inputsValid["tip"] = false;
            }

            inputsValid.Remove(inputName);
            inputsValid[inputName] = status;

            if (CheckAllInputsReady())
                ComputeTip();
        }

        private bool CheckAllInputsReady()
        {
            bool ready = false;

            foreach(bool status in inputsValid.Values)
            {
                if (status && !ready)
                    ready = true;
                else if (!status)
                    return false;
            }

            return ready;
        }

        private void bottomTextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
