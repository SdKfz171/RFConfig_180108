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

namespace RFConfig.Views
{
    /// <summary>
    /// RFSettingPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class RFSettingPage : Page
    {
        SerialPort _Serial;

        public int RFMode { get; set; }

        public RFSettingPage(SerialPort serial)
        {
            InitializeComponent();

            Debug.WriteLine("IN RFSEttingPage");

            _Serial = serial;

            _Serial.DataReceived += _Serial_DataReceived;

            if (_Serial.PortName == MainWindow.ComboBoxItem)
                OutputTextBlock.Text = App.SavedData;

            R1.IsChecked = true;
            ChannelCombo.SelectedIndex = 0;
        }

        private void _Serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string ReceivedData;
            ReceivedData = _Serial.ReadExisting();

            Debug.WriteLine("데이터를 받았습니다.");
            Debug.WriteLine(ReceivedData);

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                 OutputTextBlock.Text += ReceivedData;
            }));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] convertedString = Encoding.Default.GetBytes(string.Format("CMODE({0})\r", RFMode));

                _Serial.Write(convertedString, 0, convertedString.Length);

                Debug.WriteLine("전송 성공!");
            }
            catch
            {
                string Message = "포트가 닫겨있습니다.\n포트 연결 페이지로 가시겠습니까?";
                string Title = "경고";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage image = MessageBoxImage.Warning;
                
                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, Message,Title,button,image);
                
                if(result == MessageBoxResult.Yes)
                {
                    SettingPage settingPage = new SettingPage();

                    NavigationService.Navigate(settingPage);
                }         

                Debug.WriteLine("포트가 닫겨있습니다.");

                ((MainWindow)App.Current.MainWindow).SendButton.Background = MainWindow.LightGray;
                ((MainWindow)App.Current.MainWindow).ConnectButton.Background = MainWindow.DarkGray;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] convertedString = Encoding.Default.GetBytes("CSTOP\r");

                _Serial.Write(convertedString, 0, convertedString.Length);

                Debug.WriteLine("전송 성공!");
            }
            catch
            {
                string Message = "포트가 닫겨있습니다.\n포트 연결 페이지로 가시겠습니까?";
                string Title = "경고";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage image = MessageBoxImage.Warning;

                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, Message, Title, button, image);

                if (result == MessageBoxResult.Yes)
                {
                    ///////////////////////////////////////////////////////
                    ((MainWindow)App.Current.MainWindow).SendButton.Background = MainWindow.LightGray;
                    ((MainWindow)App.Current.MainWindow).ConnectButton.Background = MainWindow.DarkGray;

                    SettingPage settingPage = new SettingPage();

                    NavigationService.Navigate(settingPage);
                    ////////////////////////////////////////////////////////
                }

                Debug.WriteLine("포트가 닫겨있습니다.");
            }
        }

        private void CFSendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] convertedString = Encoding.Default.GetBytes(string.Format("CHSET({0})\r",ChannelCombo.SelectedIndex));

                _Serial.Write(convertedString, 0, convertedString.Length);

                Debug.WriteLine("전송 성공!");
            }
            catch
            {
                string Message = "포트가 닫겨있습니다.\n포트 연결 페이지로 가시겠습니까?";
                string Title = "경고";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage image = MessageBoxImage.Warning;

                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, Message, Title, button, image);

                if (result == MessageBoxResult.Yes)
                {
                    ((MainWindow)App.Current.MainWindow).SendButton.Background = MainWindow.LightGray;
                    ((MainWindow)App.Current.MainWindow).ConnectButton.Background = MainWindow.DarkGray;

                    SettingPage settingPage = new SettingPage();

                    NavigationService.Navigate(settingPage);
                }

                Debug.WriteLine("포트가 닫겨있습니다.");
            }
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (InputTextBox.Text.ToLower() == "%clear%")
                    OutputTextBlock.Text = "";
                else if(InputTextBox.Text.ToLower() == "%save%")
                    App.SavedData = OutputTextBlock.Text;
                else
                {
                    try
                    {
                        byte[] convertedString = Encoding.Default.GetBytes(InputTextBox.Text + "\r");

                        byte[] STX = new byte[1] { 0x02 };
                        byte[] ETX = new byte[1] { 0x03 };
                        byte[] CR = new byte[1] { 0x0D };
                        byte[] LF = new byte[1] { 0x0A };

                        IEnumerable<byte> result = STX.Concat(convertedString).Concat(CR).Concat(LF).Concat(ETX);

                        _Serial.Write(result.ToArray(), 0, result.ToArray().Length);

                        Debug.WriteLine("전송 성공!");
                    }
                    catch
                    {
                        string Message = "포트가 닫겨있습니다.\n포트 연결 페이지로 가시겠습니까?";
                        string Title = "경고";
                        MessageBoxButton button = MessageBoxButton.YesNo;
                        MessageBoxImage image = MessageBoxImage.Warning;

                        MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, Message, Title, button, image);

                        if (result == MessageBoxResult.Yes)
                        {
                            ((MainWindow)App.Current.MainWindow).SendButton.Background = MainWindow.LightGray;
                            ((MainWindow)App.Current.MainWindow).ConnectButton.Background = MainWindow.DarkGray;

                            SettingPage settingPage = new SettingPage();

                            NavigationService.Navigate(settingPage);
                        }

                        Debug.WriteLine("포트가 닫겨있습니다.");
                    }
                }
                
            }
        }

        private void DefaultSendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] convertedString = Encoding.Default.GetBytes(InputTextBox.Text + "\r");

                byte[] STX = new byte[1] { 0x02 };
                byte[] ETX = new byte[1] { 0x03 };
                byte[] CR = new byte[1] { 0x0D };
                byte[] LF = new byte[1] { 0x0A };

                IEnumerable<byte> result = STX.Concat(convertedString).Concat(CR).Concat(LF).Concat(ETX);

                _Serial.Write(result.ToArray(), 0, result.ToArray().Length);

                Debug.WriteLine("전송 성공!");
            }
            catch
            {
                string Message = "포트가 닫겨있습니다.\n포트 연결 페이지로 가시겠습니까?";
                string Title = "경고";
                MessageBoxButton button = MessageBoxButton.YesNo;
                MessageBoxImage image = MessageBoxImage.Warning;

                MessageBoxResult result = MessageBox.Show(Application.Current.MainWindow, Message, Title, button, image);

                if (result == MessageBoxResult.Yes)
                {
                    ((MainWindow)App.Current.MainWindow).SendButton.Background = MainWindow.LightGray;
                    ((MainWindow)App.Current.MainWindow).ConnectButton.Background = MainWindow.DarkGray;

                    SettingPage settingPage = new SettingPage();

                    NavigationService.Navigate(settingPage);
                }

                Debug.WriteLine("포트가 닫겨있습니다.");
            }
            

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton)sender).Name)
            {
                case "R1":
                    RFMode = 1;
                    InputTextBox.Text = "CMODE(1)\r";
                    break;

                case "R2":
                    RFMode = 2;
                    InputTextBox.Text = "CMODE(2)\r";
                    break;

                case "R3":
                    RFMode = 3;
                    InputTextBox.Text = "CMODE(3)\r";
                    break;

                case "R4":
                    RFMode = 4;
                    InputTextBox.Text = "CMODE(4)\r";
                    break;
            }
        }

        private void ChannelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputTextBox.Text = string.Format("CHSET({0})\r",ChannelCombo.SelectedIndex);
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBlock.Text = "";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            App.SavedData = OutputTextBlock.Text;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("RF Loaded!");
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _Serial.Close();

            Debug.WriteLine("RF UnLoaded!");
        }
    }
}
