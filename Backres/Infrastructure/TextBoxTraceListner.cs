using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Backres.Infrastructure
{
	public class TextBoxTraceListener : TraceListener
	{
		private TextBlock _target;

		private StringSendDelegate _invokeWrite;

		public TextBoxTraceListener(TextBlock target)
		{
			_target = target;
			_invokeWrite = new StringSendDelegate(SendString);
		}

		public override void Write(string message)
		{
			//_target.Dispatcher.Invoke(_invokeWrite, new object[] { message });
			_target.Dispatcher.Invoke(new Action(()=>{ _target.Text += message; }));
		}

		public override void WriteLine(string message)
		{
			//_target.Dispatcher.Invoke(_invokeWrite, new object[] { message + Environment.NewLine });
			_target.Dispatcher.Invoke(new Action(() => { _target.Text += message + Environment.NewLine; }));
		}

		private delegate void StringSendDelegate(string message);

		private void SendString(string message)
		{
			// No need to lock text box as this function will only 
			// ever be executed from the UI thread
			_target.Text += message;
		}
	}
}
