
using RFConfig.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
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
using System.Windows.Threading;

namespace RFConfig
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string ComboBoxItem = "";

        Window StandardWindow = Application.Current.MainWindow;

        SettingPage settingPage;
        RFSettingPage rFSettingPage;

        public static SerialPort _Serial;

        public static SolidColorBrush LightGray = new SolidColorBrush { Color = Colors.Transparent, Opacity = 0.5 };
        public static SolidColorBrush DarkGray = new SolidColorBrush { Color = Colors.DarkGray, Opacity = 0.5 };

        //#FFD3D3D3

        public MainWindow()
        {
            InitializeComponent();

            _Serial = new SerialPort();

            SettingPage settingPage = new SettingPage();

            MainFrame.Navigate(settingPage);

            ConnectButton.Background = DarkGray;
        }



        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectLabel.Visibility == Visibility.Collapsed)
            {
                ConnectLabel.Visibility = Visibility.Visible;
                SendLabel.Visibility = Visibility.Visible;

                //MainMenu.Width += 80;
            }
                
            else
            {
                ConnectLabel.Visibility = Visibility.Collapsed;
                SendLabel.Visibility = Visibility.Collapsed;

                //MainMenu.Width -= 80;
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            SendButton.Background = LightGray;
            ConnectButton.Background = DarkGray;

            settingPage = new SettingPage();
            MainFrame.Navigate(settingPage);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectButton.Background = LightGray;
                SendButton.Background = DarkGray;

                rFSettingPage = new RFSettingPage(_Serial);
                MainFrame.Navigate(rFSettingPage);
            }catch(ArgumentNullException ane)
            {
                Debug.WriteLine("시리얼 포트가 열리지 않았습니다.");
            }catch(Exception ex)
            {

            }
            
        }
    }
}
