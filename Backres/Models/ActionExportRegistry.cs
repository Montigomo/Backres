using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Backres.Models
{

	public class ActionExportRegistry : IAction
	{
		private ActionDirection ActionDirection { get; set; }

		public ActionExportRegistry(BrAction bAction, ActionDirection bDirection)
		{
			if (bAction.Name != "ExportRegistry")
				throw new Exception("Invalid argument for ActionCopy constructor");

			DstFilePath = bAction.DstPath.NormilizePath();

			RegistryKey = bAction.RegistryKey;

			ActionDirection = bDirection;
		}

		public string DstFilePath { get; }

		public string RegistryKey { get; set; }


		public bool Run()
		{
			if (File.Exists(DstFilePath))
			{
				//
			}
			(new FileInfo(DstFilePath)).Directory.Create();
			BrRegistryHelper.ExportRegistry(RegistryKey, DstFilePath);
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
