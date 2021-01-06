using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.ContainerRegistry
{
	public class ContainerRegistryObjectParametersTest
	{
		private interface IDummy
		{
		}
		
		private class Dummy : IDummy
		{
		}
		
		private class Test
		{
			public IDummy Dummy { get; }

			public Test(IDummy dummy)
			{
				Dummy = dummy;
			}
		}

		[Test]
		public void SetObjectParameterWithType()
		{
			var container = new GeckoContainer();
			var dummy = new Dummy();
			
			container.RegisterSingletone<Test>().SetParameter(new Parameter(dummy, typeof(IDummy)));
			var instance = container.Resolve<Test>();

			Assert.AreEqual(dummy, instance.Dummy);
		}
		
		[Test]
		public void SetObjectParameterWithoutType()
		{
			var container = new GeckoContainer();
			var dummy = new Dummy();
			
			container.RegisterSingletone<Test>().SetParameter(new Parameter(dummy));
			Assert.Throws<ContainerException>(() =>
			{
				container.Resolve<Test>();
			});
		}
	}
}