﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Backres.Models
{
	internal class ActionCopyFile : ActionAbstract, IAction
	{

		public ActionCopyFile(BackresAction bAction, ActionDirection bDirection): base(bAction, bDirection)
		{
		}

		protected override string ActionName { get; } = "CopyFile";

		public bool Run()
		{
			if (File.Exists(SrcPath))
			{
				(new FileInfo(DstPath)).Directory.Create();
				File.Copy(SrcPath, DstPath, Overwrite);
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
