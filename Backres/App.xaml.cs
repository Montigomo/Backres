using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using Bacres.Native;

namespace Backres
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		static public readonly string Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
		static public readonly string AppTitle = "BACKup REStore app settings tool  - " + Version;

		internal static MemoryMappedFile sharedMemory;

		private static volatile Mutex _instanceMutex = null;
		private static string _appGuid = "{3332CD94-C0BC-45CC-B688-C3B714761385}";
		//static const GUID <<name>> = { 0x3332cd94, 0xc0bc, 0x45cc, { 0xb6, 0x88, 0xc3, 0xb7, 0x14, 0x76, 0x13, 0x85 } };
		private static string mutexName = $"mutex_{_appGuid}";
		public static string mmfName = $"mmf_{_appGuid}";

		//public IKernel Container { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{

			string[] commandLineArgs = System.Environment.GetCommandLineArgs();

			try
			{
				bool createdNew = false;

				_instanceMutex = new Mutex(true, mutexName, out createdNew);

				#region Open Existing App
				if (!createdNew)
				{
					IntPtr hWnd = System.IntPtr.Zero;
					lock (typeof(WindowMain))
					{
						using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(mmfName, MemoryMappedFileRights.Read))
						{
							using (MemoryMappedViewAccessor mmfReader = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
							{
								mmfReader.Read<IntPtr>(0, out hWnd);
							}
						}
					}
					WindowActivator.Activate(hWnd);
					Application.Current.Shutdown();
					return;
				}
				#endregion

				lock (typeof(WindowMain))
				{
					sharedMemory = MemoryMappedFile.CreateNew(mmfName, 8, MemoryMappedFileAccess.ReadWrite);
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace + "\n\n", "Exception thrown");
				Application.Current.Shutdown();
				return;
			}
			finally
			{

			}

			base.OnStartup(e);

		}
		
		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (sharedMemory != null)
				sharedMemory.Dispose();
		}
	}


	public class RectConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			double width = (double)values[0];
			double height = (double)values[1];
			return new Rect(0, 0, width, height);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
