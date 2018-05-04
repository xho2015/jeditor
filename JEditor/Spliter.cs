/*
 * Created by SharpDevelop.
 * User: xho
 * Date: 2018-04-13
 * Time: 11:12 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Xml;  
using System.IO;  
using System.Collections;   
using System.Drawing; 

namespace NGO.Pad.JEditor
{
	public class Word {
		public int Index {get ;set;}
		public string Inner {get; set;}
		
		public Word(int idx, string str) {
			this.Index = idx;
			this.Inner = str;
		}
		
		public override string ToString()
		{
			return string.Format("[{0}:{1}]", Index, Inner);
		}

	}
	/// <summary>
	/// Description of Spliter.
	/// </summary>
	public abstract class Spliter
	{
		private static Dictionary<String, Spliter> Spliters = new Dictionary<string, Spliter>();
 		
 		public static Spliter Instance(JEditor.Languages language){	
 			return Spliters[language.ToString()];
		}          
	
 		public abstract List<Word> Split(string line);
 		
 		public abstract bool IsComment(string line);
 		
        static Spliter()   
        {         
        	Spliters[JEditor.Languages.HTML.ToString()] = new HTMLSplitor();
        	Spliters[JEditor.Languages.JAVASCRIPT.ToString()] = new JavascriptSplitor();
        	Spliters[JEditor.Languages.CSS.ToString()] = new CSSSplitor();
        }  
	}
	
	public class HTMLSplitor : Spliter {
		
		public HTMLSplitor() {
			
		}
		private List<Word> EMPTY = new List<Word>();
		public override List<Word>  Split(string line) {
			if (line.Length == 0)
				return EMPTY;
			var result = new List<Word>();
			var temp = new StringBuilder();
			char[] cArray = line.ToCharArray();
			bool opened = false;
			for( int i = 0; i < cArray.Length; i++) {
				if (cArray[i] == '<') {
					if (opened)	return EMPTY;
					if (temp.Length > 0 )
						result.Add(new Word(i - temp.Length, temp.ToString()));
					temp.Clear();
					temp.Append(cArray[i]);	
					opened = true;
					continue;
				} else if (cArray[i]=='>') {
					if (!opened) return EMPTY;
					temp.Append(cArray[i]);	
					result.Add(new Word(i + 1 - temp.Length, temp.ToString()));
					temp.Clear();
					opened = false;
					continue;
				}
				temp.Append(cArray[i]);
			}
			if (temp.Length > 0) {
				result.Add(new Word(cArray.Length - temp.Length, temp.ToString()));
			}
			return result;
		}
		
		public override bool IsComment(string line) {
			char[] cArray = line.ToCharArray();
			for( int i = 0; i < cArray.Length; i++) {
				switch (cArray[i]) {
				    case '\t' : {
						break;
	    			}
					case ' ' : {
						break;
	    			}
					case '<' : {
						if (cArray.Length > i+3 && cArray[i+1]=='!' && cArray[i+2]=='-' && cArray[i+3]=='-')
							return true;
						return false;
	    			}						
					default: {
						return false;
					}        
		    	}
			}
			return false;
		}
 		
	}
	
	public class JavascriptSplitor : Spliter {
		
		private List<Word> EMPTY = new List<Word>();
		
		public override List<Word>  Split(string line) {
			if (line.Length == 0)
				return EMPTY;
			var result = new List<Word>();
			var temp = new StringBuilder();
			bool sqopen = false, dqopen = false;
			char[] cArray = line.ToCharArray();
			for( int i = 0; i < cArray.Length; i++) {
				switch (cArray[i]) {
				    case '\'' : {
						temp.Append(cArray[i]);
						if (sqopen) {
							if ( temp.Length > 0)
								result.Add(new Word(i + 1 - temp.Length, temp.ToString()));
							sqopen = false;
							temp.Clear();
						} else {
							sqopen = true;
						}
						break;
	    			}
					case '"' : {
						temp.Append(cArray[i]);
						if (dqopen ) {
							if (temp.Length > 0)
								result.Add(new Word(i + 1 - temp.Length, temp.ToString()));
							dqopen = false;
							temp.Clear();
						} else {
							dqopen = true;	
						}
						break;
	    			}	
					case ' ' :{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '.' :{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
		    		case ';':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();					
	    				break;
	    			}
				    case '{':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
 					case '(':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case ')':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '}':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '[':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}						
					case ']':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '\t':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '\n':{
						if (dqopen || sqopen) {
							temp.Append(cArray[i]); break;
						}
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}						
					default: {
						temp.Append(cArray[i]);
				        break;
					}
				        
		    	}
			}
			return result;
		}
		
		public override bool IsComment(string line) {
			char[] cArray = line.ToCharArray();
			for( int i = 0; i < cArray.Length; i++) {
				switch (cArray[i]) {
				    case '\t' : {
						break;
	    			}
					case ' ' : {
						break;
	    			}
					case '/' : {
						if ((cArray.Length > i+1) && (cArray[i+1]=='/' || cArray[i+1]=='*'))
							return true;
						return false;
	    			}						
					default: {
						return false;
					}        
		    	}
			}
			return false;
		}
	}
	
	public class CSSSplitor : Spliter {
		
		private List<Word> EMPTY = new List<Word>();
		
		public override List<Word>  Split(string line) {
			if (line.Length == 0)
				return EMPTY;
			var result = new List<Word>();
			var temp = new StringBuilder();
			char[] cArray = line.ToCharArray();
			for( int i = 0; i < cArray.Length; i++) {
				switch (cArray[i]) {
				    case ' ' :{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '.' :{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
		    		case ';':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();					
	    				break;
	    			}
				    case '{':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
 					case '(':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case ')':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '}':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case ':':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}						
					case '\t':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '\n':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '#':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}
					case '>':{
						if (temp.Length > 0)
							result.Add(new Word(i - temp.Length, temp.ToString()));
						temp.Clear();
						break;
	    			}						
					default: {
						temp.Append(cArray[i]);
				        break;
					}
				        
		    	}
			}
			return result;
		}
		
		public override bool IsComment(string line) {
			char[] cArray = line.ToCharArray();
			for( int i = 0; i < cArray.Length; i++) {
				switch (cArray[i]) {
				    case '\t' : {
						break;
	    			}
					case ' ' : {
						break;
	    			}
					case '/' : {
							if ((cArray.Length > i+1) && cArray[i+1]=='*')
							return true;
						return false;
	    			}						
					default: {
						return false;
					}        
		    	}
			}
			return false;
		}
	}
}
