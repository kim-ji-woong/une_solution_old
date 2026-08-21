using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using GDK;

namespace GDK_tester
{
    public class text_box_numeric : TextBox
    {
        bool allow_space = false;

        // restricts the entry of characters to digits (including hex), the negative sign,
        // the decimal point, and editing keystrokes (backspace).
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo nfi = CultureInfo.CurrentCulture.NumberFormat;
            string sep_decimal = nfi.NumberDecimalSeparator;
            string sep_group = nfi.NumberGroupSeparator;
            string sign = nfi.NegativeSign;

            // workaround for sep_group equal to non-breaking space 
            if (sep_group == ((char)160).ToString())
            {
                sep_group = " ";
            }

            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // digits are OK
            }
            else if (keyInput.Equals(sep_decimal) || keyInput.Equals(sep_group) || keyInput.Equals(sign))
            {
                // decimal separator is OK
            }
            else if (e.KeyChar == '\b')
            {
                // backspace key is OK
            }
            //  else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0) 
            //  { 
            //  // Let the edit control handle control and alt key combinations 
            //  } 
            else if (this.allow_space && e.KeyChar == ' ')
            {

            }
            else
            {
                // consume this invalid key and beep
                e.Handled = true;
            }
        }

        public int value_int
        {
            get { return Int32.Parse(this.Text); }
        }
        public ushort value_ushort
        {
            get { return UInt16.Parse(this.Text); }
        }
        public decimal value_decimal
        {
            get { return Decimal.Parse(this.Text); }
        }

        public bool AllowSpace
        {
            set { this.allow_space = value; }
            get { return this.allow_space; }
        }
    }
}
