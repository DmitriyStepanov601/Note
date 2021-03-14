using System;
using System.Linq;
using System.Windows.Forms;

namespace Note
{
    public partial class GoToForm : Form
    {
        public GoToForm()
        {
            InitializeComponent();

            butGo.Click += (s, a) => {
                if (this.Owner is MainForm main)
                {
                    int lineNumber = Convert.ToInt32(tbLineNum.Text);
                    if (lineNumber > 0 && lineNumber <= main.note.Lines.Count())
                    {
                        main.note.SelectionStart = main.note.GetFirstCharIndexFromLine(Convert.ToInt32(tbLineNum.Text) - 1);
                        main.note.ScrollToCaret();
                        this.Close();
                    }
                }
            };

            butCancel.Click += (s, a) => { this.Close(); };
        }
    }
}

