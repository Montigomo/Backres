using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Backres.Models
{

	internal class ActionImportRegistry :ActionAbstract, IAction
	{

		public ActionImportRegistry(BackresAction bAction, ActionDirection bDirection) : base(bAction, bDirection)
		{

		}

		protected override string ActionName { get; } = "ImportRegistry";

		public bool Run()
		{
			(new FileInfo(DstPath)).Directory.Create();
			BackresRegistryHelper.ImportRegistry(DstPath);
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
