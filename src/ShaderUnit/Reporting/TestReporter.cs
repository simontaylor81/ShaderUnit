﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Reporting
{
	// Interface for a test reporter implementation.
	interface ITestReporter
	{
		Task InitialiseAsync();
		Task TestCompleteAsync(string name, bool bSuccess, Bitmap result);
		Task DisposeAsync();
	}

	// Fixture class for reporting test results. Doesn't do the actual reporting,
	// just hands off to the actual reporter implementation that is appropriate.
	class TestReporter : ITestReporter
	{
		private readonly ITestReporter _impl;

		public static TestReporter Instance { get; private set; }

		internal static Task StaticInitAsync()
		{
			Trace.Assert(Instance == null);
			Instance = new TestReporter();
			return Instance.InitialiseAsync();
		}

		internal static async Task StaticDisposeAsync()
		{
			if (Instance != null)
			{
				await Instance.DisposeAsync();
				Instance = null;
			}
		}

		public TestReporter()
		{
			if (CIHelper.IsCI)
			{
				// Write to dirty html file in CI.
				_impl = new HtmlReporter();
			}
			else
			{
				// Use simple file system writer when running locally.
				_impl = new FileSystemReporter();
			}
		}

		public async Task InitialiseAsync()
		{
			if (_impl != null)
			{
				await _impl.InitialiseAsync();
			}
		}

		public async Task DisposeAsync()
		{
			if (_impl != null)
			{
				await _impl.DisposeAsync();
			}
		}

		public async Task TestCompleteAsync(string name, bool bSuccess, Bitmap result)
		{
			if (_impl != null)
			{
				await _impl.TestCompleteAsync(name, bSuccess, result);
			}
		}
	}
}
