/*
 * 由SharpDevelop创建。
 * 用户： Bob (XuYong Hou) houxuyong@hotmail.com
 * 日期: 2018/4/16
 * 时间: 22:22
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NGO.Pad.JEditor
{
	/// <summary>
	/// Determine the color of the word and then render it.
	/// </summary>
	public abstract class Render
	{
		private static Dictionary<String, Render> Renders = new Dictionary<string, Render>();
 		
 		public static Render Instance(JEditor.Languages language){	
 			return Renders[language.ToString()];
		}  
		
		static Render()   
        {         
        	Renders[JEditor.Languages.HTML.ToString()] = new HTMLRender();
        	Renders[JEditor.Languages.JAVASCRIPT.ToString()] = new JavascriptRender();
        	Renders[JEditor.Languages.CSS.ToString()] = new CSSRender();
        } 
		
		public abstract Color BackColor();
		public abstract Color ForeColor();
		public abstract string AutoComplete(string candidate);
		
		public abstract void Coloring(Word word,RichTextBox tbase, int selectStart, int lineStart);
		
		public abstract void Comment(string line, RichTextBox tbase, int selectStart, int lineStart);
		
	}
	
	internal class HTMLRender : Render
	{
		private Parser parser = Parser.Instance(JEditor.Languages.HTML);

		public override Color BackColor() {
			return parser.BackColor();
		}
		
		public override Color ForeColor() {
			return parser.ForeColor();
		}
		
		public override string AutoComplete(string candidate) {
			//TODO: change to a efficient way for mapping fill Text
			if (candidate == "html") {
				return "2:></html>";
			}
			if (candidate == "head") {
				return "2:></head>";
			}
			if (candidate == "body") {
				return "2:></body>";
			}
			if (candidate == "script") {
				return "2:></script>";
			}
			return null;
		}
		
		
		public override void Coloring(Word word, RichTextBox tbase, int selectStart, int lineStart) {
			//System.Diagnostics.Debug.WriteLine("ls={0},idx={1}",lineStart,index);
			bool fuzzy = false;
			var color =  parser.IsKeyword(word.Inner, ref fuzzy);
           	if (color == Color.Empty)  
           		return;
           	
           	if (!fuzzy) {
           		tbase.SelectionStart = lineStart + word.Index;
            	tbase.SelectionLength = word.Inner.Length;  
            	tbase.SelectionColor = color;
            	return;
           	}
           	
           	//fuzzy matched, splite down the attributes
           	// an example: <a href="/tags/tag_ul.asp" title="HTML &lt;ul&gt; 标签">
           	char[] cArray = word.Inner.ToCharArray();
           	bool kwDone = false, quaOpen = false;
           	int startIndex = lineStart + word.Index;
           	int renderedPos = 0;
           	for(int i=0; i< cArray.Length; i++) {
           		if (cArray[i] == ' ' || cArray[i] == '\t') {
           			if (!kwDone) {
           				tbase.SelectionStart = startIndex;
			            tbase.SelectionLength = i; 
			            renderedPos = i;
			            tbase.SelectionColor = color;
			            kwDone = true;
           			} else if (!quaOpen){
           				renderedPos = i;
           			}
           			
           		} else if (cArray[i] == '=') {
           			tbase.SelectionStart = startIndex + renderedPos;
		            tbase.SelectionLength = i - renderedPos; 
		            renderedPos = i;
		            tbase.SelectionColor = parser.AttribKeyColor();
           		} else if (cArray[i] == '\'' || cArray[i] == '"') {
           			if (!quaOpen) {
           				quaOpen = true;
           				renderedPos = i;
           			} else {
           				tbase.SelectionStart = startIndex + renderedPos;
		            	tbase.SelectionLength = i - renderedPos + 1; 
		            	renderedPos = i;
		           	 	tbase.SelectionColor = parser.AttribValueColor();
		           	 	quaOpen = false;
           			}
           		} else if (cArray[i] == '>') {
       				tbase.SelectionStart = startIndex + i;
	            	tbase.SelectionLength = 1; 
	           	 	tbase.SelectionColor = color;
           		}
           	}    	
		}
		
		public override void Comment(string line, RichTextBox tbase, int selectStart, int lineStart) {
			var color =  parser.CommentColor();
			tbase.SelectionStart = lineStart + 0;
            tbase.SelectionLength = line.Length;  
            tbase.SelectionColor = color;
		}
	}
	
	internal class JavascriptRender : Render
	{
		private Parser parser = Parser.Instance(JEditor.Languages.JAVASCRIPT);
        
		public override Color BackColor() {
			return parser.BackColor();
		}
		
		public override Color ForeColor() {
			return parser.ForeColor();
		}
		
		public override string AutoComplete(string candidate) {
			//TODO: change to a efficient way for mapping fill Text
			return "2:XXXXXXXXXX";	
		}
		
		public override void Coloring(Word word, RichTextBox tbase, int selectStart, int lineStart) {
			//System.Diagnostics.Debug.WriteLine("ls={0},idx={1}",lineStart,index);
			bool fuzzy = false;
			var color =  parser.IsKeyword(word.Inner, ref fuzzy);
           	if (color == Color.Empty)  
           		return;
           	tbase.SelectionStart = lineStart + word.Index;
            tbase.SelectionLength = word.Inner.Length;  
            tbase.SelectionColor = color;
		}
		
		public override void Comment(string line, RichTextBox tbase, int selectStart, int lineStart) {
			var color =  parser.CommentColor();
			tbase.SelectionStart = lineStart + 0;
            tbase.SelectionLength = line.Length;  
            tbase.SelectionColor = color;
		}
	}
	
	internal class CSSRender : Render
	{
		private Parser parser = Parser.Instance(JEditor.Languages.CSS);
        
		public  override Color BackColor() {
			return parser.BackColor();
		}
		
		public override Color ForeColor() {
			return parser.ForeColor();
		}
		
		public override string AutoComplete(string candidate) {
			//TODO: change to a efficient way for mapping fill Text
			return "2:XXXXXXXXXX";	
		}
		
		public override void Coloring(Word word, RichTextBox tbase, int selectStart, int lineStart) {
			//System.Diagnostics.Debug.WriteLine("ls={0},idx={1}",lineStart,index);
			bool fuzzy = false;
			var color =  parser.IsKeyword(word.Inner, ref fuzzy);
           	if (color == Color.Empty)  
           		return;
           	tbase.SelectionStart = lineStart + word.Index;
            tbase.SelectionLength = word.Inner.Length;  
            tbase.SelectionColor = color;
		}
		
		public override void Comment(string line, RichTextBox tbase, int selectStart, int lineStart) {
			var color =  parser.CommentColor();
			tbase.SelectionStart = lineStart + 0;
            tbase.SelectionLength = line.Length;  
            tbase.SelectionColor = color;
		}
	}
}
