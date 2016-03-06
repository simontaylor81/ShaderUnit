using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderUnit.Util
{
	// Represents a group of disposable resources that are disposed together.
	// Kind of re-implementation of the same thing from Rx.
	public sealed class CompositeDisposable : IDisposable
	{
		private List<IDisposable> _disposables;

		public CompositeDisposable()
		{
			_disposables = new List<IDisposable>();
		}
		public CompositeDisposable(int capacity)
		{
			_disposables = new List<IDisposable>(capacity);
		}
		public CompositeDisposable(params IDisposable[] disposables)
			: this((IEnumerable<IDisposable>)disposables)
		{ }
		public CompositeDisposable(IEnumerable<IDisposable> disposables)
		{
			_disposables = new List<IDisposable>(disposables);
		}

		public void Add(params IDisposable[] items)
		{
			_disposables.AddRange(items);
		}

		public void Dispose()
		{
			foreach (var disposable in _disposables)
			{
				disposable.Dispose();
			}
			_disposables.Clear();
		}
	}
}
