using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Backres.Models
{

	internal class ActionExportRegistry : ActionAbstract, IAction
	{

		public ActionExportRegistry(BackresAction bAction, ActionDirection bDirection) : base(bAction, bDirection)
		{

		}

		protected override string ActionName { get; } = "ExportRegistry";


		public bool Run()
		{
			(new FileInfo(DstPath)).Directory.Create();
			BackresRegistryHelper.ExportRegistry(RegistryKey, DstPath);
			return true;
		}

		public Task<bool> RunAsync()
		{
			var tcs = new TaskCompletionSource<bool>();
			Task.Factory.StartNew(/*async*/ () =>
			{

				tcs.SetResult(Run());
			}, TaskCreationOptions.LongRunning);
			return tcs.Task;
		}
	}
}
