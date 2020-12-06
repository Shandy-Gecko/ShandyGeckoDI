using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ContainerDependencyAttributeTests
	{
		private class Test1
		{
		}
		
		private class Test2
		{
			[Dependency] public Test1 Test1 { get; set; }
		}

		[Test]
		public void TestResolveDependency()
		{
			var container = new GeckoContainer();
			var test1 = new Test1();

			container.RegisterInstance(test1);
			
			var test2 = new Test2();
			container.BuildUp(test2);
			
			Assert.AreEqual(test1, test2.Test1);
		}
		
		//TODO дописать - нужно проверить инъекцию во все варианты доступности свойства
	}
}