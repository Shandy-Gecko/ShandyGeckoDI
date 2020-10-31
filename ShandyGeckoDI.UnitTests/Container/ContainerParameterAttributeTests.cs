using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ContainerParameterAttributeTests
	{
		private class Test1
		{
		}
		
		private class Test2
		{
			[Parameter("1")] public Test1 Test1_1 { get; set; }
			[Parameter("2")] public Test1 Test1_2 { get; set; }
		}

		[Test]
		public void ParameterBuildUpTest()
		{
			var container = new ShandyGecko.ShandyGeckoDI.Container();
			var test2 = new Test2();

			var test1_1 = new Test1();
			var test1_2 = new Test1();

			container.BuildUp(test2, new Parameter(test1_1, "1"), new Parameter(test1_2, "2"));
			
			Assert.AreEqual(test1_1, test2.Test1_1);
			Assert.AreEqual(test1_2, test2.Test1_2);
		}
	}
}