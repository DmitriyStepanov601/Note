using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;
using System.Globalization;

namespace Note
{
    public partial class MainForm : Form
    {
        bool tbChange = false;
        string docPath = "";
        bool isTextChangedHandlerRunning = false;

        public MainForm()
        {
            InitializeComponent();
            this.Text = Properties.Settings.Default.newDocName + " - " + Properties.Settings.Default.programmName;

            note.TextChanged += (s, a) =>
            {
                tbChange = true;                // Индикатор изменения текста
                TextWork.EditEnableds(ref note, ref copy, ref cut, ref delete, ref findReplace, ref go);
                TextWork.StatusAnalize(ref note, ref statusLinesCount, ref statusWordsCount, ref statusCharSpaceCount, ref statusCharCount);
            };

            // -----------------------  Меню File -----------------------------------------------
            newFile.Click += (s, a) =>
            {
                if (tbChange == true)
                {
                    DialogResult message = MessageBox.Show("Сохранить текущий документ перед созданием нового?", "Создание документа", MessageBoxButtons.YesNoCancel);
                    if (message == DialogResult.Yes)
                    {
                        if (docPath != "")
                        {
                            FileWork.SaveFile(ref note, ref tbChange, ref docPath);
                            FileWork.CreateFile(ref note, ref tbChange, ref docPath);
                        }
                        else if (docPath == "")
                        {
                            FileWork.SaveAsFile(ref note, ref tbChange, ref docPath);
                            FileWork.CreateFile(ref note, ref tbChange, ref docPath);
                        }
                    }
                    else if (message == DialogResult.No)
                    {
                        FileWork.CreateFile(ref note, ref tbChange, ref docPath);
                    }
                }
                else
                {
                    FileWork.CreateFile(ref note, ref tbChange, ref docPath);
                }
            };

            open.Click += (s, a) =>
            {
                if (tbChange == true)
                {
                    DialogResult message = MessageBox.Show("Сохранить текущий документ перед открытием нового?", "Открытие документа", MessageBoxButtons.YesNoCancel);
                    if (message == DialogResult.Yes)
                    {
                        if (docPath != "")
                        {
                            FileWork.SaveFile(ref note, ref tbChange, ref docPath);
                            FileWork.OpenFile(ref note, ref tbChange, ref docPath);
                        }
                        else if (docPath == "")
                        {
                            FileWork.SaveAsFile(ref note, ref tbChange, ref docPath);
                            FileWork.OpenFile(ref note, ref tbChange, ref docPath);
                        }
                    }
                    else if (message == DialogResult.No)
                    {
                        FileWork.OpenFile(ref note, ref tbChange, ref docPath);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    FileWork.OpenFile(ref note, ref tbChange, ref docPath);
                }
            };

            save.Click += (s, a) =>
            {
                if (docPath != "")
                {
                    FileWork.SaveFile(ref note, ref tbChange, ref docPath);
                }
                else
                {
                    FileWork.SaveAsFile(ref note, ref tbChange, ref docPath);
                }
            };

            saveAs.Click += (s, a) => { FileWork.SaveAsFile(ref note, ref tbChange, ref docPath); };

            pageParameters.Click += (s, a) =>
            {
                if (pageSetupDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.DefaultPageSettings = pageSetupDialog.PageSettings;
                }
            };

            print.Click += (s, a) =>
            {
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        printDocument.Print();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Ошибка параметров печати.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            };

            // Отправка документа на печать
            printDocument.PrintPage += (s, a) => { a.Graphics.DrawString(note.Text, note.Font, Brushes.Black, 10, 10); };
            exit.Click += (s, a) => { Application.Exit(); };

            // -----------------------  Меню Edit -----------------------------------------------
            undo.Click += (s, a) => { note.Undo(); };
            cut.Click += (s, a) => { if (note.SelectionLength > 0) { note.Cut(); } };
            copy.Click += (s, a) => { if (note.SelectionLength > 0) { note.Copy(); } };
            paste.Click += (s, a) => { note.Paste(); };
            delete.Click += (s, a) => { if (note.SelectionLength > 0) { note.SelectedText = ""; } };

            findReplace.Click += (s, a) =>
            {
                SearchForm findText = new SearchForm
                {
                    Owner = this
                };
                findText.Show();
            };

            go.Click += (s, a) =>
            {
                GoToForm gotoform = new GoToForm
                {
                    Owner = this
                };
                gotoform.tbLineNum.Minimum = 0;
                gotoform.tbLineNum.Maximum = note.Lines.Count();
                gotoform.ShowDialog();
            };

            giveAll.Click += (s, a) => { note.SelectAll(); };
            timeDate.Click += (s, a) => { note.AppendText(Environment.NewLine + Convert.ToString(System.DateTime.Now)); };

            // -----------------------  Меню Format -----------------------------------------------
            wordWrap.Click += (s, a) => {
                if (wordWrap.CheckState == CheckState.Checked)
                {
                    note.WordWrap = true;
                    note.ScrollBars = ScrollBars.Vertical;
                    go.Enabled = false;
                    statusLab1.Visible = false;
                    statusLinesCount.Visible = false;
                }
                else
                {
                    note.WordWrap = false;
                    note.ScrollBars = ScrollBars.Both;
                    go.Enabled = true;
                    statusLab1.Visible = true;
                    statusLinesCount.Visible = true;
                }
            };

            font.Click += (s, a) => {
                fontDialog.Font = note.Font;
                DialogResult = fontDialog.ShowDialog();
                if (DialogResult == DialogResult.OK) { note.Font = fontDialog.Font; }
            };

            // -----------------------  Меню View -----------------------------------------------
            statusBar.Click += (s, a) => {
                if (statusBar.CheckState == CheckState.Checked) { statusBar.Visible = true; }
                else { statusBar.Visible = false; }
            };
        }

        public MainForm(string fileName)         // Открытие программы документом
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
            {
                try
                {
                    string programmName = Properties.Settings.Default.programmName;
                    FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(file, Encoding.Default);
                    note.Text = reader.ReadToEnd();
                    reader.Close();
                    docPath = fileName;
                    tbChange = false;
                    this.Text = Path.GetFileName(fileName) + " — " + programmName;
                    note.Select(0, 0);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void MainForm_Load(object sender, EventArgs e)
        {
            //Задаем сохраненные (или стандартные) настройки
            this.Width = Properties.Settings.Default.formWidth;
            this.Height = Properties.Settings.Default.formHeight;
            note.Font = Properties.Settings.Default.textFont;
            if (Properties.Settings.Default.statusBarVisible == true)
            { statusBar.CheckState = CheckState.Checked; }
            else
            { statusBar.CheckState = CheckState.Unchecked; }
            if (Properties.Settings.Default.textWordWrap == true)
            { wordWrap.CheckState = CheckState.Checked; }
            else
            { wordWrap.CheckState = CheckState.Unchecked; }
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e) // Событие ДО выхода из программы
        {
            Properties.Settings.Default.formWidth = this.Width;
            Properties.Settings.Default.formHeight = this.Height;
            Properties.Settings.Default.textWordWrap = note.WordWrap;
            Properties.Settings.Default.textFont = note.Font;
            Properties.Settings.Default.statusBarVisible = statusBar.Visible;
            Properties.Settings.Default.Save();
            if (tbChange == true)
            {
                DialogResult message = MessageBox.Show("Сохранить текущий документ перед выходом?", "Выход из программы", MessageBoxButtons.YesNoCancel);
                if (message == DialogResult.Yes)
                {
                    if (docPath != "")
                    {
                        FileWork.SaveFile(ref note, ref tbChange, ref docPath);
                        Application.Exit();
                    }
                    else if (docPath == "")
                    {
                        FileWork.SaveAsFile(ref note, ref tbChange, ref docPath);
                        Application.Exit();
                    }
                }
                else if (message == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}

