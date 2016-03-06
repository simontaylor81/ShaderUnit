using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace ShaderUnit.Reporting
{
	// Add this attribute to you test assembly to enable test reporting.
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
	public class UseTestReporterAttribute : Attribute, ITestAction
	{
		public ActionTargets Targets => ActionTargets.Suite;

		public void BeforeTest(ITest test)
		{
			TestReporter.StaticInitAsync().Wait();
		}

		public void AfterTest(ITest test)
		{
			TestReporter.StaticDisposeAsync().Wait();
		}
	}
}
