using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.MethodInjection
{
	public class MethodInjectionInAbstractMethodTest
	{
		private class Test
		{
		}
		
		private abstract class AbstractTest
		{
			public Test Test { get; protected set; }

			[Dependency]
			public abstract void SetDeps(Test test);
		}
		
		private class ConcreteTest : AbstractTest
		{
			public override void SetDeps(Test test)
			{
				Test = test;
			}
		}
		
		[Test]
		public void AbstractMethodInjectionTest()
		{
			var container = new GeckoContainer();
			var injection1 = new Test();

			container.RegisterInstance(injection1);

			var actual = container.BuildUpType<ConcreteTest>();
			
			Assert.IsNotNull(actual);
			Assert.AreEqual(injection1, actual.Test);
		}
	}
}