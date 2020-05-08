using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilesName2CSV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        static bool ParseFileName(string sInputName, string sMode, out string sParsedRow)
        {
            string commasep = ",";
            string valwrap="\"";
            string decimalsep = ".";
            try
            {
                sParsedRow = valwrap+sInputName+valwrap;
                switch (sMode)
                {
                    case "R":
                        //sParsedRow
                        String mypattern = @"^([0-9]{8})[-]{0,1}([0-9]{0,8})_(\w{1,50})_([0-9]{1,12})E([0-9]{2})cent.pdf";
                        foreach (System.Text.RegularExpressions.Match m in
                        System.Text.RegularExpressions.Regex.Matches(sInputName, mypattern))
                        {
                            //Groups 1-3
                            for (int i = 1; i < 4; i++)
                            {
                                sParsedRow += commasep + valwrap + m.Groups[i].Value + valwrap;
                            }
                            //Groups 4,5 amount
                            sParsedRow += commasep + valwrap + m.Groups[4].Value +  decimalsep + m.Groups[5].Value + valwrap;
                        }
                            break;
                    default: // N and unmanaged cases
                        break;
                }
                return true;
            }
            catch (Exception er)
            {
                sParsedRow = "Unexpected Error" + er.Message;
                return false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = false;
            textBoxFolder.Text = "";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowserDialog1.SelectedPath;
                buttonStart.Enabled = true;
            }
            else
            {
                label1.Text = "Folder Selection Cancelled, please repeat.";
            };
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (textBoxFolder.Text!="")
            {
                if (!checkBoxAppend.Checked)
                    {
                    richTextBox1.ResetText(); //CLEANUP
                    }
                label1.Text = "Reading files from " + textBoxFolder.Text;

                try
                {
                    string sourceDirectory = textBoxFolder.Text;
                    string parsedVal="";
                    var txtFiles = Directory.EnumerateFiles(sourceDirectory, "*.*", SearchOption.AllDirectories);
                    string sParsingMode = "N"; //Default N = Only names
                    if (radioButton1.Checked)
                        {
                          sParsingMode = "R"; //R = Refound style documents
                        }
                    else
                        {
                          //TO DO: Manage further options
                        }

                    foreach (string currentFile in txtFiles)
                    {
                        string fileName = currentFile.Substring(sourceDirectory.Length + 1);
                        label1.Text = "Found " + fileName;
                        if (ParseFileName(fileName, sParsingMode, out parsedVal))
                            {
                            richTextBox1.AppendText(parsedVal);
                            richTextBox1.AppendText(Environment.NewLine);
                            }
                    }
                    System.Windows.Forms.Clipboard.SetText(richTextBox1.Text);
                }
                catch (Exception er)
                {
                    label1.Text="ERROR " + (er.Message);
                }
            }
        }
    }
}
