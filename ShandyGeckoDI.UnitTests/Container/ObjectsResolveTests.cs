using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container
{
	public class ObjectsResolveTests
	{
		private class Dummy
		{
		}
		
		private class Test1
		{
			public Dummy Dummy { get; }

			public Test1(Dummy dummy)
			{
				Dummy = dummy;
			}
		}
		
		private class Test2
		{
			public Test1 Test1 { get; }

			public Test2(Test1 test1)
			{
				Test1 = test1;
			}
		}

		[Test]
		public void ObjectsResolveTest()
		{
			var dummy = new Dummy();
			
			var container = new GeckoContainer();

			container.RegisterInstance(dummy);

			container.RegisterSingletone<Test1>();
			container.RegisterSingletone<Test2>();

			var test2Instance = container.Resolve<Test2>();
			
			Assert.NotNull(test2Instance);
			Assert.NotNull(test2Instance.Test1);
			Assert.NotNull(test2Instance.Test1.Dummy);
			Assert.AreEqual(dummy, test2Instance.Test1.Dummy);
		}
	}
}