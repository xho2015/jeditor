/*
 * Created by SharpDevelop.
 * User: xho
 * Date: 2018-04-13
 * Time: 11:45 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NUnit.Framework;


namespace NGO.Pad.JEditor.UnitTest
{
	[TestFixture]
	public class TestSplitor
	{
		[Test]
		public void TestMethod_HTML1()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = "  	<p1>abc sdp</p> ";
			var words = sp.Split(line);
			
			Assert.AreEqual(5, words.Count);
			
		}
		
		[Test]
		public void TestMethod_HTML2()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = "<p1>abc sdp</p>";
			var words = sp.Split(line);
			
			Assert.AreEqual(3, words.Count);
			
		}
		
		[Test]
		public void TestMethod_HTML3()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = "<p1>abc< sdp</p>";
			var words = sp.Split(line);
			
			Assert.AreEqual(0, words.Count);
			
		}
		
		[Test]
		public void TestMethod_HTML4()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = "<p1>abc> sdp</p>";
			var words = sp.Split(line);
			
			Assert.AreEqual(0, words.Count);
			
		}
		
		[Test]
		public void TestMethod_HTML4_1()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = "<p>";
			var words = sp.Split(line);
			
			Assert.AreEqual(1, words.Count);
			
		}
		
		[Test]
		public void TestMethod_HTML5()
		{
			char[] SPLITER = new char[]{ ' ', '.', '\n', '(', ')', '}', '{', '"', '[', ']','\t' };

			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = " <p1> abcsdp </p>";
			string[] words = line.Split(SPLITER);
			
			Assert.AreEqual(4, words.Length);
			var words2 = sp.Split(line);
			
			Assert.AreEqual(4, words2.Count);
		}
		
		[Test]
		public void TestMethod_HTML6()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = "ad<p>da</p> ";
			var words = sp.Split(line);
			
			Assert.AreEqual(5, words.Count);
			
			Assert.AreEqual(0, words[0].Index);
			Assert.AreEqual(2, words[1].Index);
			Assert.AreEqual(5, words[2].Index);
			Assert.AreEqual(7, words[3].Index);
			Assert.AreEqual(11, words[4].Index);
		}
		
		[Test]
		public void TestMethod_HTML_IsComment_1()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.HTML);
			
			string line = "	 <!-";
			var comment = sp.IsComment(line);
			
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 <");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 <!");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 <!-");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 <!--");
			Assert.AreEqual(true, comment);
			
			comment = sp.IsComment("	1 <!--");
			Assert.AreEqual(false, comment);
		}
		
		
		[Test]
		public void TestMethod_JAVASCRIPT_1()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.JAVASCRIPT);
			
			string line = @" function abc() {alert('hello');	var a = 'my string' }";
			var words = sp.Split(line);
			
			Assert.AreEqual(8, words.Count);
			
			line = " var = '123'";
			words = sp.Split(line);
			
			Assert.AreEqual(3, words.Count);
			
			line = " var = \"123\"";
			words = sp.Split(line);
			
			Assert.AreEqual(3, words.Count);
			
		}
		
		[Test]
		public void TestMethod_JAVASCRIPT_2()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.JAVASCRIPT);
			
			string line = @" var a = '的shayanle的'";
			var words = sp.Split(line);
			
			Assert.AreEqual(4, words.Count);
			
		}
		
		[Test]
		public void TestMethod_Javascript_IsComment_1()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.JAVASCRIPT);
			
			string line = "	 /";
			var comment = sp.IsComment(line);
			
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 /1");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 //");
			Assert.AreEqual(true, comment);
			
			comment = sp.IsComment("	 ///");
			Assert.AreEqual(true, comment);
			
			comment = sp.IsComment("	 /*");
			Assert.AreEqual(true, comment);
			
			comment = sp.IsComment("	1 //1");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	1 /*");
			Assert.AreEqual(false, comment);
		}
		
		[Test]
		public void TestMethod_CSS_IsComment_1()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.CSS);
			
			string line = "	 /";
			var comment = sp.IsComment(line);
			
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 /1");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 //");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 ///");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	 /*");
			Assert.AreEqual(true, comment);
			
			comment = sp.IsComment("	1 //1");
			Assert.AreEqual(false, comment);
			
			comment = sp.IsComment("	1 /*");
			Assert.AreEqual(false, comment);
		}
		
		/// <summary>
		/// some test senario
		/// https://developer.mozilla.org/en-US/docs/Web/CSS/align-items
		/// </summary>
		[Test]
		public void TestMethod_CSS_1()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.CSS);
			
			string line = @" align-items: normal; ";
			var words = sp.Split(line);
			
			Assert.AreEqual(2, words.Count);
			
		}
		
  		[Test]
		public void TestMethod_CSS_2()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.CSS);
			
			string line = @"	 div > div { box-sizing: border-box;  border: 2px solid #8c8c8c;} ";
			var words = sp.Split(line);
			
			Assert.AreEqual(8, words.Count);
			
		}
		
		[Test]
		public void TestMethod_CSS_3()
		{
			Spliter sp = Spliter.Instance(JEditor.Languages.CSS);
			
			string line = @"	 #item1 {  background-color: #8cffa0;  min-height: 30px;} ";
			var words = sp.Split(line);
			
			Assert.AreEqual(5, words.Count);
			
		}
	}
}
