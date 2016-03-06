using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using SRPCommon.Util;

namespace ShaderUnit.TestRenderer
{
	public class RenderTestBase
	{
		private Bitmap _imageResult;
		private RenderTestHarness _harness;

		private static readonly string _baseDir = Path.Combine(GlobalConfig.BaseDir, @"src\ShaderUnit\TestScripts");	// TODO!
		private static readonly string _expectedResultDir = Path.Combine(_baseDir, "ExpectedResults");

		[SetUp]
		public void Setup()
		{
		}

		[TearDown]
		public async Task TearDown()
		{
			// Dispose the test harness.
			_harness?.Dispose();
			_harness = null;

			// Report result.
			var context = TestContext.CurrentContext;
			await TestReporter.Instance.TestCompleteAsync(
				context.Test.FullName, context.Result.Outcome.Status == TestStatus.Passed, _imageResult);

			// Clear state ready for the next run (NUnit re-uses class instances).
			_imageResult = null;
		}

		protected IComputeHarness CreateComputeHarness()
		{
			if (_harness != null)
			{
				throw new ShaderUnitException("Can only create one harness per test run.");
			}
			_harness = new RenderTestHarness(new TestRenderer());
			return _harness;
		}

		protected IRenderHarness CreateRenderHarness(int width, int height)
		{
			if (_harness != null)
			{
				throw new ShaderUnitException("Can only create one harness per test run.");
			}
			_harness = new RenderTestHarness(new TestRenderer(width, height));
			return _harness;
		}

		protected void CompareImage(Bitmap result)
		{
			// Stash result for reporting.
			Assert.That(_imageResult, Is.Null, "Can only compare one image per test");
			_imageResult = result;

			// Load the image to compare against.
			var context = TestContext.CurrentContext;
			var expectedImageFilename = Path.Combine(_expectedResultDir, context.Test.FullName + ".png");
			Assert.That(File.Exists(expectedImageFilename), "No expected image to compare against.");
			var expected = new Bitmap(expectedImageFilename);

			// Compare the images.
			AssertEx.ImagesEqual(expected, result);
		}
	}
}
