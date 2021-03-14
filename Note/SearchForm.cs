using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Note
{
    public partial class SearchForm : Form
    {
        int findCutLength = 0; // На сколько символов обрезаем текст для поиска

        public SearchForm()
        {
            InitializeComponent();
            tbFind.TextChanged += (s, a) => { findCutLength = 0; };
            tbReplace.TextChanged += (s, a) => { findCutLength = 0; };
            cbReg.CheckStateChanged += (s, a) => { findCutLength = 0; };

            btFind.Click += (s, a) => {
                if (this.Owner is MainForm main)
                {
                    if (cbReg.CheckState == CheckState.Checked)
                    {
                        TextWork.FindTextBox(ref main.note, findText: tbFind.Text, findCutLength: ref findCutLength, register: true);
                    }
                    else
                    {
                        TextWork.FindTextBox(ref main.note, findText: tbFind.Text, findCutLength: ref findCutLength, register: false);
                    }
                }
            };

            btReplace.Click += (s, a) =>
            {
                if (this.Owner is MainForm main)
                {
                    if (cbReg.CheckState == CheckState.Checked)
                    {
                        TextWork.ReplaceTextBox(ref main.note, findText: tbFind.Text, replaceText: tbReplace.Text, findCutLength: ref findCutLength, register: true);
                    }
                    else
                    {
                        TextWork.ReplaceTextBox(ref main.note, findText: tbFind.Text, replaceText: tbReplace.Text, findCutLength: ref findCutLength, register: false);
                    }
                }
            };

            btReplaceAll.Click += (s, a) =>
            {
                if (this.Owner is MainForm main)
                {
                    if (cbReg.CheckState == CheckState.Checked)
                    {
                        TextWork.ReplaceAllTextBox(ref main.note, findText: tbFind.Text, replaceText: tbReplace.Text, register: true);
                    }
                    else
                    {
                        TextWork.ReplaceAllTextBox(ref main.note, findText: tbFind.Text, replaceText: tbReplace.Text, register: false);
                    }
                }
            };

            btCancel.Click += (s, a) => { this.Close(); };
        }

        void SearchForm_Shown(object sender, EventArgs e) // Событие при открытии формы поиска и замены
        {
            tbFind.Focus();
        }

        void SearchForm_FormClosing(object sender, FormClosingEventArgs e) // Событие при закрытии формы (до закрытия)
        {
            findCutLength = 0;
        }
    }
}
