using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Text;


namespace Note
{
    class FileWork
    {
        // Метод "Создать новый документ"
        public static void CreateFile(ref TextBox note, ref bool tbChange, ref string docPath)
        {
            string newDocName = Properties.Settings.Default.newDocName;
            string programmName = Properties.Settings.Default.programmName;
            note.Clear();
            docPath = "";
            tbChange = false;
            MainForm.ActiveForm.Text = newDocName + " — " + programmName;
        }

        // Метод "Открыть документ"
        public static void OpenFile(ref TextBox note, ref bool tbChange, ref string docPath)
        {
            string programmName = Properties.Settings.Default.programmName;
            OpenFileDialog openDocument = new OpenFileDialog();
            openDocument.Title = "Open a text document";
            openDocument.Filter = "Text files|*.txt|All files|*.*";
            if (openDocument.ShowDialog() == DialogResult.OK)
            {
                FileStream file = new FileStream(openDocument.FileName, FileMode.Open, FileAccess.Read);
                StreamReader reader = new StreamReader(file, Encoding.Default);
                note.Text = reader.ReadToEnd();
                reader.Close();
                docPath = openDocument.FileName;
                tbChange = false;
                MainForm.ActiveForm.Text = openDocument.SafeFileName + " — " + programmName;
            }
        }

        // Метод "Сохранить документ"
        public static void SaveFile(ref TextBox note, ref bool tbChange, ref string docPath)
        {
            FileStream file = new FileStream(docPath, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(file, Encoding.Default);
            writer.Write(note.Text);
            writer.Close();
            tbChange = false;
        }

        // Метод "Сохранить документ как..."
        public static void SaveAsFile(ref TextBox note, ref bool tbChange, ref string docPath)
        {
            string programmName = Properties.Settings.Default.programmName;
            SaveFileDialog saveAsDocument = new SaveFileDialog();
            saveAsDocument.Title = "Save the document as...";
            saveAsDocument.FileName = "Text document";
            saveAsDocument.Filter = "RTF files|*.rtf|Text files|*.txt|All files|*.*";

            if (saveAsDocument.ShowDialog() == DialogResult.OK)     //Если пользователь подтвердил сохранение
            {
                //Создаем файл по пути, выбранному в окне сохранения
                FileStream file = new FileStream(saveAsDocument.FileName, FileMode.Create, FileAccess.Write);
                StreamWriter writer = new StreamWriter(file, Encoding.Default);
                writer.Write(note.Text);                   //записываем содержимое в файл
                writer.Close();                               //закрываем поток
                tbChange = false;
                docPath = saveAsDocument.FileName;
                MainForm.ActiveForm.Text = Path.GetFileName(saveAsDocument.FileName) + " — " + programmName;
            }
            else
            {
                tbChange = true;
                return;
            }
        }
    }
}
