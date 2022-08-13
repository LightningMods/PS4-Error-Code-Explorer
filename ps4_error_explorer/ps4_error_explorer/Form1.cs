using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Linq;


namespace ps4_error_explorer
{
	public partial class Form1 : Form
	{
		List<string> arr;
		public Form1()
		{
			InitializeComponent();
			TimerCallback cb = new TimerCallback(this.updateByTimer);
			this.timer = new System.Threading.Timer(cb, null, 0, 500);
			this.indexList = new IndexList(500000);
			this.rowList = new DataGridViewRow[622015];
			this.finished_ = true;
			regexOverride_ = false;

			if (!File.Exists("error_codes.csv"))
			{
				MessageBox.Show("No error_codes.csv file... exiting...");
				System.Environment.Exit(1);
			}

			arr = File.ReadAllLines("error_codes.csv").ToList();
			/*
			for (int i = 0; i < arr.Count; i++)
			{
				if(i % 5 == 0)
				Console.WriteLine(i + " " + arr[i].ToString());
			}
			*/

		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			finished_ = false;
			label2.Text = "Searching...";
			string text = textBox1.Text;
			if (!regexOverride_)
			{
				checkBox1.Checked = Regex.IsMatch(text, "[^A-Z a-z0-9_-]");
				regexOverride_ = false;
			}
			bool @checked = checkBox1.Checked;
			this.gridView1.Rows.Clear();
			this.goalCount = 0;
			this.indexList.Clear();
			int num = 0;
			if (text == "")
			{
				label2.Text = "";
				this.finished_ = true;
				return;
			}
			string[] array = null;
			array = text.Split(' ');
			for (int i = 0; i < array.Length; i++)
			{
				Match match = Regex.Match(array[i], "^-[1-9]\\d*$");
				while (match.Success)
				{
					int num2 = 0;
					try
					{
						num2 = int.Parse(match.Value);
					}
					catch (OverflowException)
					{
						match = match.NextMatch();
						continue;
					}
					array[i] = num2.ToString("x8");
					match = match.NextMatch();
				}
			}
			int num3 = 0;
			while ((long)num3 < arr.Count)
			{
				//Console.WriteLine(num3 + " " + arr.Count + " :" + arr[num3].ToString());
				bool flag = true;
				int j = 0;
				while (j < array.Length)
				{
					if (@checked)
					{
						try
						{
							if (!Regex.IsMatch(arr[num3].ToString(), array[j], RegexOptions.IgnoreCase))
							{
								flag = false;
							}
							goto label_1;
						}
						catch (SystemException ex)
						{
							label2.Text = ex.Message;
							return;
						}
					}
					goto label_2;
					label_1:
					j++;
					continue;
				label_2:
					//if (arr[num3].Length < 1) goto label_1;

					if (!arr[num3].ToLower().Contains(array[j].ToLower()))
					{
						flag = false;
						goto label_1;
					}
					goto label_1;
				}
				if (flag)
				{
					num++;
					num3 = num3 - num3 % 4;
					//Console.WriteLine(arr[num3].ToString() + " found ");
					this.indexList.Add(num3 / 4);
					this.goalCount++;
					num3 += 3;
				}
				num3++;
			}

			label2.Text = (num == 1) ? "1 result" : num.ToString() + " results";

			this.finished_ = true;
		}

		private void CopyTextToClipBoard()
		{
			ListViewItem listViewItem = new ListViewItem();
			string text = listViewItem.SubItems[0].Text;
			for (int i = 1; i < 5; i++)
			{
				text += " ";
				text += listViewItem.SubItems[i].Text;
			}
			Clipboard.SetText(text);
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			if (!e.Control || e.KeyCode != Keys.C)
			{
				return;
			}
			CopyTextToClipBoard();
		}

		private void OnClick(object sender, MouseEventArgs e)
		{
			Point mousePosition = Control.MousePosition;
			contextMenu1.Show(mousePosition);
		}

		private void OnMenuCopyClick(object sender, EventArgs e)
		{
			CopyTextToClipBoard();
		}

		private void OnRegexStateChanged(object sender, EventArgs e)
		{
			regexOverride_ = true;
		}

		private void gridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			e.Value = arr[this.indexList.Get(e.RowIndex) * 4 + e.ColumnIndex];
		}

		private void updateByTimer(object o)
		{

			try
			{
				if (base.IsHandleCreated)
				{
					base.Invoke(new Form1.Del(delegate ()
					{
						if (this.goalCount > this.gridView1.RowCount)
						{
							if (this.gridView1.RowCount + 100 > this.goalCount)
							{
								this.gridView1.RowCount = this.goalCount;
								return;
							}
							this.gridView1.RowCount += 100;
						}
					}));
				}
			}
            catch (Exception) {
			}
		}

		private const int numItem = 5;
		public bool finished_;
		public bool regexOverride_;
		private delegate void Del();

    }
}
