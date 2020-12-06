using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ContainerInheritConstructorInjectionTests
	{
		private class Test1
		{
			
		}
		
		private abstract class BaseTest
		{
			public Test1 Test1 { get; }

			public BaseTest(Test1 test1)
			{
				Test1 = test1;
			}
		}
		
		private class ChildTest : BaseTest
		{
			public ChildTest(Test1 test1) : base(test1)
			{
			}
		}

		[Test]
		public void TestInjectInInheritedConstructor()
		{
			var container = new GeckoContainer();
			var test1 = new Test1();

			container.RegisterInstance(test1);

			var child = container.BuildUpType<ChildTest>();
			
			Assert.IsNotNull(child);
			Assert.AreEqual(test1, child.Test1);
		}
	}
}