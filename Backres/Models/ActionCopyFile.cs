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
		private ActionDirection ActionDirection { get; set; }

		public ActionCopyFile(BrAction bAction, ActionDirection bDirection)
		{
			if (bAction.Name != "CopyFile")
				throw new Exception("Invalid argument for ActionCopy constructor");

			SrcFilePath = bAction.SrcFile.NormilizePath();
			DstFilePath = bAction.DstFile.NormilizePath();

			ActionDirection = bDirection;
		}

		public string SrcFilePath { get; }

		public string DstFilePath { get; }

		public Task<bool> Run()
		{
			var tcs = new TaskCompletionSource<bool>();
			Task.Factory.StartNew(/*async*/ () =>
			{
				if (File.Exists(SrcFilePath))
				{
					(new FileInfo(DstFilePath)).Directory.Create();
					File.Copy(SrcFilePath, DstFilePath, true);
				}
				tcs.SetResult(true);
			}, TaskCreationOptions.LongRunning);
			return tcs.Task;
		}
	}
}
