using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Backres.Controls
{
	/// <summary>
	/// Interaction logic for BrItem.xaml
	/// </summary>
	public partial class BrItem : UserControl, INotifyPropertyChanged
	{
		public BrItem()
		{
			InitializeComponent();
		}


		#region Constructor & Properties



		#endregion

		#region PropertyChanges

		[field: NonSerializedAttribute()]
		public event PropertyChangedEventHandler PropertyChanged;

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region Methods

		//private void ProcessKey(KeyEventArgs e)
		//{
		//	if (e == null)
		//		return;

		//	if (e.Key == Key.Scroll || e.Key == Key.CapsLock || e.Key == Key.NumLock)
		//	{
		//		return;
		//	}
		//	else if (e.Key == Key.Escape)
		//	{
		//		KeyStroke.Clear();
		//		NotifyPropertyChanged("KeyStroke");
		//	}
		//	else if (!KeyStroke.IsComplete())
		//	{
		//		KeyStroke.Add(e.Key);
		//		NotifyPropertyChanged("KeyStroke");
		//	}
		//	e.Handled = true;
		//}

		//public void KeyPressedEventHandler(object sender, KeyPressedEventArgs e)
		//{
		//	this.Dispatcher.Invoke(new System.Action(() =>
		//	{
		//		ListBox parent = this.FindParent<ListBox>();
		//		if (parent.Items.Contains(this))
		//			parent.SelectedItem = this;
		//		this.OnMouseLeftButtonDown(null);
		//	}
		//	));
		//}

		#endregion

		private void txtKey_KeyDown(object sender, KeyEventArgs e)
		{
			//if (!e.IsRepeat)
				//ProcessKey(e);
		}

	}


	//[ValueConversion(typeof(KeyStroke), typeof(String))]
	//public class KeyStrokeConverter : IValueConverter
	//{
	//	public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	//	{
	//		return value == null ? String.Empty : ((KeyStroke)value).ToString();
	//	}

	//	public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
	//	{
	//		throw new NotSupportedException("The method or operation is not implemented.");
	//	}
	//}



}