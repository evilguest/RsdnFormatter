using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

using NUnit.Framework;

namespace Rsdn.Framework.Formatting.Tests
{
	internal delegate void TestDelegate(string srcPath, string goldPath);

	[TestFixture]
	public class FormatterTest
	{
		[MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
		private void CallTest(TestDelegate testFunc)
		{
			CallTest(testFunc, new StackTrace().GetFrame(2).GetMethod().Name);
		}

		private void CallTest(TestDelegate testFunc, string testName)
		{
			var asmPath =
				Path.Combine(
					Path.GetDirectoryName(
						new Uri(GetType().Assembly.CodeBase).AbsolutePath),
						"../../TestData");
			testFunc(
				Path.Combine(asmPath, testName + ".txt"),
				Path.Combine(asmPath, testName + ".gold"));
		}

		private void TestFormat()
		{
			CallTest(TestHelper.TestFormat);
		}

		private void TestFormat(string testName)
		{
			CallTest(TestHelper.TestFormat, testName);
		}

		private void TestQuote()
		{
			CallTest(TestHelper.TestQuote);
		}

		[Test] public void SimpleFormatting() { TestFormat(); }
		[Test] public void Heading() { TestFormat(); }
		[Test] public void Quotation() { TestFormat(); }
		[Test] public void RsdnLink() { TestFormat(); }
		[Test] public void Smiles() { TestFormat(); }
		[Test] public void Urls() { TestFormat(); }
		[Test] public void XSS() { TestFormat(); }
		[Test] public void Sql() { TestFormat(); }
		[Test] public void Cut() { TestFormat(); }
		[Test] public void Cpp() { TestFormat(); }
		[Test] public void SubSup() { TestFormat(); }
		[Test] public void Msg2408361() { TestFormat(); }
		[Test] public void ObjC() { TestFormat(); }
		[Test] public void MakeQuote() { TestQuote(); }
		[Test] public void ExcessiveBrs() { TestFormat(); }

		[Test]
		public void ExcessiveBrsRelease()
		{
			TestFormat("ExcessiveBrs");
		}
	}
}