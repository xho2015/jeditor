/*
 * 由SharpDevelop创建。
 * 用户： Bob (XuYong Hou) houxuyong@hotmail.com
 * 日期: 2018/4/13
 * 时间: 2:46
 * 
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;  
using System.IO;  
using System.Reflection;  
using System.Drawing;
using Snow.X.Algorithm; 

namespace NGO.Pad.JEditor
{
	/// <summary>
	/// Description of Parser.
	/// </summary>
	public abstract class Parser  
    {  
        protected ArrayList swords=null;
		protected ArrayList fwords=null;        
 		protected ArrayList scolors=null;	//static words
 		protected ArrayList fcolors=null; 	//fuzzy words
		protected ArrayList ccolors=null; 	//comment
		protected bool caseSensitive = false;  
		protected Color backColor;
		protected Color foreColor;
		protected Color attKeyColor;
		protected Color attValueColor;
		
 		
 		private static Dictionary<String, Parser> Parsers = new Dictionary<string, Parser>();
 		
 		public static Parser Instance(JEditor.Languages language){	
 			return Parsers[language.ToString()];
		}  
		
		static Parser()   
        {         
        	Parsers[JEditor.Languages.HTML.ToString()] = new HTMLParser();
        	Parsers[JEditor.Languages.JAVASCRIPT.ToString()] = new JavascriptParser();
        	Parsers[JEditor.Languages.CSS.ToString()] = new CSSParser();
        } 

		protected void LoadConfig(JEditor.Languages language) {
            Assembly asm = Assembly.GetExecutingAssembly();  
            string filename = "";
            switch(language) 
            {  
                case JEditor.Languages.JAVA:  
                    filename="java.xml";  
                    break;  
                case JEditor.Languages.HTML:  
                    filename="html.xml";  
                    break;               
                case JEditor.Languages.CSS:  
                    filename="css.xml";  
                    break;  
                case JEditor.Languages.JAVASCRIPT:  
                    filename="javascript.xml";  
                    break;  
                case JEditor.Languages.SQL:  
               	 	filename="sql.xml";  
                	break;  
                default:  
                    break;  
            }  
            StreamReader reader= new StreamReader(filename,  System.Text.Encoding.UTF8); 
  
            XmlDocument xdoc = new XmlDocument();  
            xdoc.Load(reader);  
  
            swords=new ArrayList();  
            fwords=new ArrayList();
			scolors=new ArrayList();  
            fcolors=new ArrayList();
            ccolors=new ArrayList();
            XmlElement root=xdoc.DocumentElement;  
            
            string colorName = null, value = null;
            XmlNodeList xnl=root.SelectNodes("/definition/sword");  
            this.caseSensitive = bool.Parse(root.Attributes["caseSensitive"].Value);  
            
            for(int i=0;i<xnl.Count;i++)
            {   
            	value = xnl[i].ChildNodes[0].Value;
            	swords.Add(this.caseSensitive ? value : value.ToLower());
                colorName = xnl[i].Attributes["color"].Value;
                scolors.Add(ParseColor(colorName));
            }
            
            xnl=root.SelectNodes("/definition/fword");  
            for(int i=0;i<xnl.Count;i++)  
            {    
            	value = xnl[i].ChildNodes[0].Value;
            	fwords.Add(this.caseSensitive ? value : value.ToLower());
                colorName = xnl[i].Attributes["color"].Value;
                fcolors.Add(ParseColor(colorName));                  
            }
            
            xnl=root.SelectNodes("/definition/comment");   
            colorName = xnl[0].Attributes["color"].Value;
            ccolors.Add(ParseColor(colorName));                  

            xnl=root.SelectNodes("/definition/background");  
            colorName = xnl[0].Attributes["color"].Value;
            backColor = ParseColor(colorName);
            
            xnl=root.SelectNodes("/definition/foreground");  
            colorName = xnl[0].Attributes["color"].Value;
            foreColor = ParseColor(colorName);

			xnl=root.SelectNodes("/definition/attribKey");  
          	if (xnl.Count > 0) {
            	colorName = xnl[0].Attributes["color"].Value;
            	attKeyColor = ParseColor(colorName); 
			} 

			xnl=root.SelectNodes("/definition/attribValue");
			if (xnl.Count > 0) {
            	colorName = xnl[0].Attributes["color"].Value;
            	attValueColor = ParseColor(colorName); 
			}
        }
		
		private Color ParseColor(string colorName) {
			if (colorName.StartsWith("#")) {
				string[] rgb = colorName.Remove(0,1).Split(',');
				Color rgbColor = Color.FromArgb(Int16.Parse(rgb[0]), Int16.Parse(rgb[1]), Int16.Parse(rgb[2]));
				return rgbColor;
			}
			
			return Color.FromName(colorName); 
		}
		
  
        public abstract Color IsKeyword(string word, ref bool fuzzy);
        
        public abstract Color CommentColor();
        
        public Color BackColor() {
        	return backColor;
        }
        
        public  Color ForeColor() {
			return foreColor;
		}
        
        public Color AttribKeyColor() {
        	return attKeyColor;
        }
        
        public Color AttribValueColor() {
        	return attValueColor;
        }
    }
	
	internal class HTMLParser : Parser
	{
		readonly TrieDict dict = new TrieDict();
		private int FUZZY = 0;
		
		public HTMLParser() {
			LoadConfig(JEditor.Languages.HTML);
			for(int i=0; i<this.swords.Count; i++)
				dict.Insert((string)this.swords[i], i);
			FUZZY = swords.Count;
			string spart = null;
			for(int i=0; i<this.fwords.Count; i++) {
				spart = (string)this.fwords[i];
				//'*' represents HTML attributes 
				spart = spart.Split('*')[0]; 
				dict.Insert(spart, FUZZY + i);
			}		
		}
		
		public override Color IsKeyword(string word, ref bool fuzzy) {
			fuzzy = false;
			string key = this.caseSensitive ? word : word.ToLower();
			int idx = dict.Scan(key);
			if (idx == -1)
				return Color.Empty;
			if (idx < FUZZY)
				return (Color)scolors[idx];
			//TODO: remove this for a not restrict check
			if (key.EndsWith(">", StringComparison.Ordinal)) {
				fuzzy = true;
				return (Color)fcolors[idx - FUZZY];				
			}
			else
				return Color.Empty;				
		}
		
		public override Color CommentColor()
		{
			return (Color)ccolors[0];
		}
	}
	
	
	internal class JavascriptParser : Parser
	{
		readonly TrieDict dict = new TrieDict();
		private int FUZZY = 0;
		
		public JavascriptParser() {
			LoadConfig(JEditor.Languages.JAVASCRIPT);
			for(int i=0; i<this.swords.Count; i++)
				dict.Insert((string)this.swords[i], i);
			FUZZY = swords.Count;
			string spart = null;
			for(int i=0; i<this.fwords.Count; i++) {
				spart = (string)this.fwords[i];
				//'*' represents plain string 
				spart = spart.Split('*')[0]; 
				dict.Insert(spart, FUZZY + i);
			}			
		}
		
		public override Color IsKeyword(string word, ref bool fuzzy) {
			string key = this.caseSensitive ? word : word.ToLower();
			var stopWatch = Stopwatch.StartNew();
			int idx = dict.Scan(key);
			stopWatch.Stop();
			System.Diagnostics.Debug.WriteLine(string.Format("javascript scan: {0}ms", stopWatch.Elapsed.TotalMilliseconds));
			if (idx == -1)
				return Color.Empty;
			if (idx < FUZZY)
				return (Color)scolors[idx];
			
			return (Color)fcolors[idx - FUZZY];
		}
		
		public override Color CommentColor()
		{
			return (Color)ccolors[0];
		}
	}
	
	
	internal class CSSParser : Parser
	{
		readonly TrieDict dict = new TrieDict();
		private int FUZZY = 0;
		
		public CSSParser() {
			LoadConfig(JEditor.Languages.CSS);
			for(int i=0; i<this.swords.Count; i++)
				dict.Insert((string)this.swords[i], i);
			FUZZY = swords.Count;
			string spart = null;
			for(int i=0; i<this.fwords.Count; i++) {
				spart = (string)this.fwords[i];
				//'*' represents plain string 
				spart = spart.Split('*')[0]; 
				dict.Insert(spart, FUZZY + i);
			}			
		}
		
		public override Color IsKeyword(string word, ref bool fuzzy) {
			string key = this.caseSensitive ? word : word.ToLower();
			var stopWatch = Stopwatch.StartNew();
			int idx = dict.Scan(key);
			stopWatch.Stop();
			System.Diagnostics.Debug.WriteLine(string.Format("css scan: {0}ms", stopWatch.Elapsed.TotalMilliseconds));
			if (idx == -1)
				return Color.Empty;
			if (idx < FUZZY)
				return (Color)scolors[idx];
			
			return (Color)fcolors[idx - FUZZY];
		}
		
		public override Color CommentColor()
		{
			return (Color)ccolors[0];
		}
	}
}
