using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Backres.Models
{
	public class ActionCopyFile : IAction
	{

		public ActionCopyFile(BrAction bAction, ActionDirection bDirection)
		{
			if (bAction.Name != "CopyFile")
				throw new Exception("Invalid argument for ActionCopy constructor");

			SrcPath = bAction.SrcPath.NormilizePath();
			DstPath = bAction.DstPath.NormilizePath();
			Overwrite = bAction.Overwrite;
			ActionDirection = bDirection;
		}
		private ActionDirection ActionDirection { get; set; }

		private bool Overwrite { get; }

		public string SrcPath { get; }

		public string DstPath { get; }

		public bool Run()
		{
			if (File.Exists(SrcPath))
			{
				(new FileInfo(DstPath)).Directory.Create();
				File.Copy(SrcPath, DstPath, true);
			}
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
