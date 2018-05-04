/*
 * 由SharpDevelop创建。
 * 用户： Bob (XuYong Hou) houxuyong@hotmail.com
 * 日期: 2018/4/13
 * 时间: 2:42
 * 
 * 
 */
using System;
using System.Drawing;
using System.Windows.Forms;

namespace NGO.Pad.JEditor
{
	/// <summary>
	/// Description of Form1.
	/// </summary>
	public partial class JeditorForm : Form
	{
		readonly JEditor editor = new JEditor(JEditor.Languages.HTML);
		
		public JeditorForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			editor.Width = this.Width - 20;
			editor.Left = 5;
			editor.Height = this.Height - 80;
			editor.Top = 5;
			
			this.Controls.Add(editor);
		}
		void Button1Click(object sender, EventArgs e)
		{
			string text = @"<html>
<head>
<!--this is comment, js and css by default has no syntax highligh in HTML-->
<style type='text/css'>
  body {background-color: yellow}
  h1 {background-color: #00ff00}
  h2 {background-color: transparent}
  p {background-color: rgb(250,0,255)}
  p.no2 {background-color: gray; padding: 20px;}
</style>
<script>
function abc() {
	alert('hello');
	var a = 'my string'
}
</script>
</head>
<body>
    <h1>this is head line 1</h1>
    <h2>this is head line 2</h2>
    <p>this is paragrah</p>
    <p class='no2'> some class applied to paragrah</p>
    <div></div>
</body>
</html>";
			char[] splited = text.ToCharArray();
			
			foreach(var t in splited) {
				if (t != '\n')
					editor.AppendText(t.ToString());
			}
		}
	}
}
