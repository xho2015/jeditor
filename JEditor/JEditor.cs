/*
 * 由SharpDevelop创建。
 * 用户： Bob (XuYong Hou) houxuyong@hotmail.com
 * 日期: 2018/4/13
 * 时间: 2:49
 * 
 * 
 */
using System;  
using System.Collections.Generic;
using System.Drawing;  
using System.Runtime.InteropServices;  
using System.Windows.Forms;
using HWND = System.IntPtr;  

namespace NGO.Pad.JEditor
{
	/// <summary>
	/// RichTextBox based ngo Editor which used for code demostration.
	/// </summary>
	public partial class JEditor : System.Windows.Forms.RichTextBox  
    {  
        private int line;  
        private Render render;
        private Spliter spliter;
       
        public string Path {set; get;}
        
        /// <summary>
        /// invoke this for preventing screen from twinkling during rendering
        /// </summary>
 		[DllImport("user32")]  
        private static extern int SendMessage(HWND hwnd, int wMsg, int wParam, IntPtr lParam);  
        private const int WM_SETREDRAW = 0xB;  
        private static Font DEFAULT_ASCII_FONT = new System.Drawing.Font("SimSun", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));		
        private static Font DEFAULT_FONT = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        
        public JEditor(Languages language)  
        {  
        	render = Render.Instance(language);
        	spliter = Spliter.Instance(language);
            WordWrap = false;
            Font = DEFAULT_FONT;
            AcceptsTab = true;
            AutoWordSelection = false;
        	ImeMode = System.Windows.Forms.ImeMode.Alpha; //set defautl Ime
        	Cursor = System.Windows.Forms.Cursors.IBeam;
        	BackColor = render.BackColor();
        	ForeColor = render.ForeColor();
        	this.KeyPress += OnKeyPress;
        	
        }
        
        private void OnKeyPress(object sender, KeyPressEventArgs e){
           if(e.KeyChar == '\t'){
             int ln = base.GetLineFromCharIndex(base.SelectionStart);
             int column = base.SelectionStart - base.GetFirstCharIndexFromLine(ln);

             string lineStr = base.Lines[ln];
             string onStr = lineStr.Substring(0, column);
              int i = onStr.LastIndexOf(' ');
              i = i<0 ? onStr.LastIndexOf('\t') : i;
              i = i<0 ? onStr.LastIndexOf('>') : i;
              i = i<0 ? 0 : i;
              //1. get candidate word
             string lastWord = string.Empty;
             lastWord = onStr.Substring(i==0? 0 : i+1).Trim();
             System.Diagnostics.Debug.WriteLine(lastWord);
             //2.backup candidate position
             int candidatePos = base.SelectionStart - lastWord.Length;
             //3.check auto-completion
             string toFill = render.AutoComplete(lastWord);
             if (toFill == null)
                 return;
  
             int curPos = base.SelectionStart + Int16.Parse(toFill.Split(':')[0]);
             base.SelectionColor = render.ForeColor();
             //AC1. add <
             base.SelectionStart = candidatePos;
             base.SelectedText="<";
             //AC2. complete the candidate
             base.SelectionStart = candidatePos+lastWord.Length + 1;
             base.SelectedText = toFill.Split(':')[1];
             base.SelectedText = string.Empty;
             //AC3. reset cursor pos
             base.SelectionStart = curPos;
             //AC4. ignore the tab
             e.Handled = true;
           }
        }
        
        /// <summary>
        /// persist to file
        /// </summary>
        public void SaveToFile() {
        	base.SaveFile(Path, RichTextBoxStreamType.UnicodePlainText);
        }
        
        public void LoadFromFile(string path) {
        	base.LoadFile(path, RichTextBoxStreamType.UnicodePlainText);
        	RenderAll();
        }
        
        private void RenderAll() {
        	int selectStart = base.SelectionStart; 
        	for(int line = 0; line <base.Lines.Length ; line++) {
        		string lineStr = base.Lines[line];  
                int lineStart = base.GetFirstCharIndexFromLine(line);  
  
                SendMessage(base.Handle, WM_SETREDRAW, 0, IntPtr.Zero);  
  
                base.SelectionStart = lineStart;  
                base.SelectionLength = lineStr.Length;  
                base.SelectionColor = render.ForeColor();
				base.SelectionFont = DEFAULT_FONT;                
                base.SelectionStart = 0;  
                base.SelectionLength = 0; 
                //System.Diagnostics.Debug.WriteLine(base.SelectionFont);
     			
                if (spliter.IsComment(lineStr)) {
                	render.Comment(lineStr, this, selectStart, lineStart);
                	//reset the selection status
	            	base.SelectionStart = selectStart;  
	            	base.SelectionLength = 0;  
	            	base.SelectionColor = render.ForeColor(); 
                } 
                else
                {
					List<Word> words = spliter.Split(lineStr);                 
	                for (int i = 0; i < words.Count; i++) {
						render.Coloring(words[i], this, selectStart, lineStart);
						//reset the selection status
	            		base.SelectionStart = selectStart;  
	            		base.SelectionLength = 0;  
	            		base.SelectionColor = render.ForeColor(); 
	                }                	
                }
                                
                SendMessage(base.Handle, WM_SETREDRAW, 1, IntPtr.Zero);  
                base.Refresh();
        	}
       	}
  
        protected override void OnTextChanged(EventArgs e)  
        {            
            if (base.Text.Length > 0)  
            {  
                int selectStart = base.SelectionStart;  
                line = base.GetLineFromCharIndex(selectStart);  
                string lineStr = base.Lines[line];  
                int lineStart = base.GetFirstCharIndexFromLine(line);  
  
                SendMessage(base.Handle, WM_SETREDRAW, 0, IntPtr.Zero);  
  
                base.SelectionStart = lineStart;  
                base.SelectionLength = lineStr.Length;  
                base.SelectionColor = render.ForeColor();
				base.SelectionFont = DEFAULT_FONT;                
                base.SelectionStart = selectStart;  
                base.SelectionLength = 0; 
                //System.Diagnostics.Debug.WriteLine(base.SelectionFont);
     			
                if (spliter.IsComment(lineStr)) {
                	render.Comment(lineStr, this, selectStart, lineStart);
                	//reset the selection status
	            	base.SelectionStart = selectStart;  
	            	base.SelectionLength = 0;  
	            	base.SelectionColor = render.ForeColor(); 
                } 
                else
                {
					List<Word> words = spliter.Split(lineStr);                 
	                for (int i = 0; i < words.Count; i++) {
						render.Coloring(words[i], this, selectStart, lineStart);
						//reset the selection status
	            		base.SelectionStart = selectStart;  
	            		base.SelectionLength = 0;  
	            		base.SelectionColor = render.ForeColor(); 
	                }                	
                }
                                
                SendMessage(base.Handle, WM_SETREDRAW, 1, IntPtr.Zero);  
                base.Refresh();  
            }  
            base.OnTextChanged(e);  
        }  
  
        public new bool WordWrap  
        {  
            get { return base.WordWrap; }  
            set { base.WordWrap = value; }  
        }  
  
        public enum Languages  
        {  
            SQL,  
            JAVA,  
            HTML,  
            CSS,
			JAVASCRIPT            
        }  
  
        private Languages language = Languages.CSS;  
  
        public Languages Language  
        {  
            get { return this.language; }  
            set { this.language = value; }  
        }  
    }  
}  
