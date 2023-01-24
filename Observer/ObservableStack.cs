using System;
using System.Collections.Generic;
using System.Text;

namespace Delegates.Observers
{
	public delegate void StackEventDelegate(object eventData);
	
	public class StackOperationsLogger
	{
		private readonly Observer _observer = new Observer();
		public void SubscribeOn<T>(ObservableStack<T> stack)
		{
			stack.StackEvent += _observer.HandleEvent;
		}

		public string GetLog()
		{
			return _observer.Log.ToString();
		}
	}

	public class Observer
	{
		public readonly StringBuilder Log = new StringBuilder();
		
		public void HandleEvent(object eventData)
		{
			Log.Append(eventData);
		}
	}


	public class ObservableStack<T>
	{
		// public event EventHandler<StackEventData<T>> StackEvent; 
		public event StackEventDelegate StackEvent;

		private readonly List<T> _data = new List<T>();

		public void Push(T obj)
		{
			_data.Add(obj);
			StackEvent?.Invoke(new StackEventData<T> { IsPushed = true, Value = obj });
		}

		public T Pop()
		{
			if (_data.Count == 0)
				throw new InvalidOperationException();
			var result = _data[_data.Count - 1];
			StackEvent?.Invoke(new StackEventData<T> { IsPushed = false, Value = result });
			return result;
		}
	}
}