﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Backres.Models
{
	public class ActionCopyFolder : IAction
	{

		public ActionCopyFolder(BrAction bAction, ActionDirection bDirection)
		{
			if (bAction.Name != "CopyFolder")
				throw new Exception("Invalid argument for ActionCopy constructor");

			SrcPath = bAction.SrcPath.NormilizePath();
			DstPath = bAction.DstPath.NormilizePath();
			ActionDirection = bDirection;
			Overwrite = bAction.Overwrite;
		}

		private bool Overwrite { get; }

		private ActionDirection ActionDirection { get; }

		public string SrcPath { get; }

		public string DstPath { get; }

		public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target, bool overwrite)
		{
			foreach (DirectoryInfo dir in source.GetDirectories())
				CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name), overwrite);

			foreach (FileInfo file in source.GetFiles())
				file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);
		}

		public bool Run()
		{
			DirectoryInfo source = new DirectoryInfo(SrcPath);
			DirectoryInfo target = new DirectoryInfo(DstPath);
			if (source.Exists)
			{
				target.Create();
				CopyFilesRecursively(source, target, Overwrite);
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
