/*
 * 由SharpDevelop创建。
 * 用户： Bob (XuYong Hou) houxuyong@hotmail.com
 * 日期: 2018/4/17
 * 时间: 12:35
 * 
 * 
 */
using System;
using System.Drawing;
using NUnit.Framework;

namespace NGO.Pad.JEditor.UnitTest
{
	[TestFixture]
	public class TestParser
	{
		[Test]
		public void TestMethod_html_1()
		{
			Parser p = Parser.Instance(JEditor.Languages.HTML);
			bool fuzzy = false;
			Color c = p.IsKeyword("<p>", ref fuzzy);
			Assert.AreNotEqual(Color.Empty, c);
			c = p.IsKeyword("</p>",ref fuzzy);
			Assert.AreNotEqual(Color.Empty, c);
		}
		
		[Test]
		public void TestMethod_html_2()
		{
			Parser p = Parser.Instance(JEditor.Languages.HTML);
			bool fuzzy = false;
			Color c = p.IsKeyword("<p id='pa1'>",ref fuzzy);
			Assert.AreEqual(true, fuzzy);
			Assert.AreNotEqual(Color.Empty, c);
			
			Color c1 = p.IsKeyword("<p id='pa1'",ref fuzzy);
			Assert.AreEqual(Color.Empty, c1);
			Assert.AreEqual(false, fuzzy);
		}
		
		[Test]
		public void TestMethod_javascript_1()
		{
			Parser p = Parser.Instance(JEditor.Languages.JAVASCRIPT);
			bool fuzzy = false;
			Color c = p.IsKeyword("function",ref fuzzy);
			Assert.AreEqual(Color.RoyalBlue, c);
			
			Color c1 = p.IsKeyword("alert",ref fuzzy);
			Assert.AreEqual(Color.RoyalBlue, c1);
		}

	}
}
