using NUnit.Framework;
using ShandyGecko.ShandyGeckoDI;

namespace ShandyGeckoDI.UnitTests.Container.ContainerRegistry
{
	public class ContainerRegistryParametersTests
	{
		private const string Parameter1 = "Test1";
		private const int Parameter2 = 9000;
		
		private class Test
		{
			public string Parameter1 { get; }
			public int Parameter2 { get; }

			public Test(string parameter1, int parameter2)
			{
				Parameter1 = parameter1;
				Parameter2 = parameter2;
			}
		}
		
		[Test]
		public void SetParameterWithNameTest()
		{
			var container = new GeckoContainer();
			container.RegisterSingletone<Test>().SetParameter(new Parameter(Parameter1, "Parameter1"));

			var instance = container.Resolve<Test>("", new Parameter(Parameter2, "Parameter2"));
			
			Assert.AreEqual(Parameter1, instance.Parameter1);
			Assert.AreEqual(Parameter2, instance.Parameter2);
		}
		
		[Test]
		public void SetParameterWithoutNameTest()
		{
			var container = new GeckoContainer();
			container.RegisterSingletone<Test>().SetParameter(new Parameter(Parameter1));

			var instance = container.Resolve<Test>("", new Parameter(Parameter2));
			
			Assert.AreEqual(Parameter1, instance.Parameter1);
			Assert.AreEqual(Parameter2, instance.Parameter2);
		}
	}
}