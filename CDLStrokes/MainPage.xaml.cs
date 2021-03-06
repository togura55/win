﻿using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Wacom.Devices;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Radios;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Wacom.SmartPadCommunication;
using Windows.UI.ViewManagement;

namespace WillDevicesSampleApp
{
    public static partial class EnumExtend
    {
        public static uint GetOperationMode(this DeviceModel param)
        {
            uint ret = 0;
            switch (param)
            {
                case DeviceModel.Unknown:
                    ret &= ~(MainPage.MODE_FILETRANSFER | MainPage.MODE_REALTIME_INK);
                    break;
                case DeviceModel.BambooSlate:
                    ret |= (MainPage.MODE_FILETRANSFER | MainPage.MODE_REALTIME_INK);
                    break;
                case DeviceModel.BambooSpark:
                case DeviceModel.IntuosPro:
                case DeviceModel.SketchpadPro:
                    break;
                case DeviceModel.Phu111:
                    ret |= (MainPage.MODE_REALTIME_INK);
                    break;
            }
            return ret;
        }
    }

    public sealed partial class MainPage : Page
	{
        private ResourceLoader resourceLoader;

        CancellationTokenSource m_cts = new CancellationTokenSource();
		ObservableCollection<DevicePropertyValuePair> m_propertiesCollection;

        public const uint MODE_FILETRANSFER = 0b01;
        public const uint MODE_REALTIME_INK = 0b10;

        uint deviceSupportMode = 0;

        public MainPage()
		{
			this.InitializeComponent();

            resourceLoader = ResourceLoader.GetForCurrentView();

            Loaded += MainPage_Loaded;

//            buttonFileTransfer.Visibility = Visibility.Collapsed;
            buttonFileTransfer.IsEnabled = false;
			buttonRealTime.IsEnabled = false;
			buttonScan.IsEnabled = false;

            m_propertiesCollection = new ObservableCollection<DevicePropertyValuePair>()
            {
                new DevicePropertyValuePair(resourceLoader.GetString("IDS_DevicePropertyName")),
                new DevicePropertyValuePair(resourceLoader.GetString("IDS_DevicePropertyESN")),
                new DevicePropertyValuePair(resourceLoader.GetString("IDS_DevicePropertyWidth")),
                new DevicePropertyValuePair(resourceLoader.GetString("IDS_DevicePropertyHeight")),
                new DevicePropertyValuePair(resourceLoader.GetString("IDS_DevicePropertyPoint")),
                new DevicePropertyValuePair(resourceLoader.GetString("IDS_DevicePropertyBattery"))
            };

			gridViewDeviceProperties.ItemsSource = m_propertiesCollection;
		}

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
            var versionInfo = Windows.ApplicationModel.Package.Current.Id.Version;
            string version = string.Format(
                               "{0}.{1}.{2}.{3}",
                               versionInfo.Major, versionInfo.Minor,
                               versionInfo.Build, versionInfo.Revision);
            ApplicationView appView = ApplicationView.GetForCurrentView();
            appView.Title = version;

            buttonScan.Content = resourceLoader.GetString("IDS_ScanDevices");
            buttonFileTransfer.Content = resourceLoader.GetString("IDS_FileTransfer");
            buttonRealTime.Content = resourceLoader.GetString("IDS_RealTime");

            buttonScan.IsEnabled = false;
			buttonFileTransfer.IsEnabled = false;
			buttonRealTime.IsEnabled = false;

			if (AppObjects.Instance.DeviceInfo == null)
			{
				AppObjects.Instance.DeviceInfo = await AppObjects.DeserializeDeviceInfoAsync();
			}

			if (AppObjects.Instance.DeviceInfo == null)
			{
                textBlockDeviceName.Text = resourceLoader.GetString("IDS_NotConnectedDevice");
                buttonScan.IsEnabled = true;
				return;
			}

			InkDeviceInfo inkDeviceInfo = AppObjects.Instance.DeviceInfo;
            textBlockDeviceName.Text = 
                string.Format(resourceLoader.GetString("IDS_ReconnectDevice"), 
                inkDeviceInfo.DeviceName, inkDeviceInfo.TransportProtocol);

			try
			{
				if (AppObjects.Instance.Device == null)
				{
					AppObjects.Instance.Device = await InkDeviceFactory.Instance.CreateDeviceAsync(inkDeviceInfo, AppObjects.Instance.AppId, false, false, OnDeviceStatusChanged);
				}

				AppObjects.Instance.Device.Disconnected += OnDeviceDisconnected;
				AppObjects.Instance.Device.DeviceStatusChanged += OnDeviceStatusChanged;
				AppObjects.Instance.Device.PairingModeEnabledCallback = OnPairingModeEnabledAsync;
			}
			catch (Exception ex)
			{
                textBlockDeviceName.Text =
                    string.Format(resourceLoader.GetString("IDS_CannotInitDevice"), inkDeviceInfo.DeviceName, ex.Message);
				buttonScan.IsEnabled = true;
				return;
			}

			textBlockDeviceName.Text =
                string.Format(resourceLoader.GetString("IDS_CurrentDevice"), inkDeviceInfo.DeviceName);

//            buttonFileTransfer.Visibility = Visibility.Collapsed;
            //			buttonFileTransfer.IsEnabled = true;
//            buttonRealTime.IsEnabled = true;
			buttonScan.IsEnabled = true;

			textBlockStatus.Text = AppObjects.GetStringForDeviceStatus(AppObjects.Instance.Device.DeviceStatus);

			await DisplayDevicePropertiesAsync();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			IDigitalInkDevice device = AppObjects.Instance.Device;

			if (device != null)
			{
				device.PairingModeEnabledCallback = null;
				device.DeviceStatusChanged -= OnDeviceStatusChanged;
				device.Disconnected -= OnDeviceDisconnected;
			}

			m_cts.Cancel();
		}

        private async Task DisplayDevicePropertiesAsync()
		{
			IDigitalInkDevice device = AppObjects.Instance.Device;

			try
			{
				m_propertiesCollection[0].PropertyValue = (string)await device.GetPropertyAsync(SmartPadProperties.DeviceName, m_cts.Token);
				m_propertiesCollection[1].PropertyValue = (string)await device.GetPropertyAsync(SmartPadProperties.SerialNumber, m_cts.Token);
				m_propertiesCollection[2].PropertyValue = ((uint)await device.GetPropertyAsync(SmartPadProperties.Width, m_cts.Token)).ToString();
				m_propertiesCollection[3].PropertyValue = ((uint)await device.GetPropertyAsync(SmartPadProperties.Height, m_cts.Token)).ToString();
				m_propertiesCollection[4].PropertyValue = ((uint)await device.GetPropertyAsync(SmartPadProperties.PointSize, m_cts.Token)).ToString();
				m_propertiesCollection[5].PropertyValue = ((int)await device.GetPropertyAsync(SmartPadProperties.BatteryLevel, m_cts.Token)).ToString() + "%";

                // Enabule buttons in terms of the supporting mode
                deviceSupportMode = device.DeviceModel.GetOperationMode();
                if ((deviceSupportMode & MODE_FILETRANSFER) > 0)
                    buttonFileTransfer.IsEnabled = true;
                if ((deviceSupportMode & MODE_REALTIME_INK) > 0)
                    buttonRealTime.IsEnabled = true;
            }
			catch (Exception ex)
			{
				textBlockStatus.Text = $"Exception: {ex.Message}";
				buttonFileTransfer.IsEnabled = false;
				buttonRealTime.IsEnabled = false;
				buttonScan.IsEnabled = true;
			}
		}

		private void ButtonScan_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(ScanAndConnectPage));
		}

		private void ButtonFileTransfer_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(FileTransferPage));
		}

		private void ButtonRealTime_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(RealTimeInkPage));
		}

		private void OnDeviceStatusChanged(object sender, DeviceStatusChangedEventArgs e)
		{
			var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				switch (e.Status)
				{
					case DeviceStatus.Idle:
						textBlockStatus.Text = AppObjects.GetStringForDeviceStatus(e.Status);
//						buttonFileTransfer.IsEnabled = true;
//						buttonRealTime.IsEnabled = true;
						break;

					case DeviceStatus.ExpectingConnectionConfirmation:
						textBlockStatus.Text = AppObjects.GetStringForDeviceStatus(e.Status);
						buttonFileTransfer.IsEnabled = false;
						buttonRealTime.IsEnabled = false;
						break;

					case DeviceStatus.NotAuthorizedConnectionNotConfirmed:
						await new MessageDialog(AppObjects.GetStringForDeviceStatus(e.Status)).ShowAsync();
						Frame.Navigate(typeof(ScanAndConnectPage));
						break;

					default:
						textBlockStatus.Text = AppObjects.GetStringForDeviceStatus(e.Status);
						break;
				}
			});
		}

		private async Task<bool> OnPairingModeEnabledAsync(bool authorizedInThisSession)
		{
			if (!authorizedInThisSession)
				return true;

			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

			var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				bool keepUsingDevice = await AppObjects.Instance.ShowPairingModeEnabledDialogAsync();

				tcs.SetResult(keepUsingDevice);

				if (!keepUsingDevice)
				{
					Frame.Navigate(typeof(ScanAndConnectPage));
				}
			});

			return await tcs.Task;
		}

		private void OnDeviceDisconnected(object sender, EventArgs e)
		{
			AppObjects.Instance.Device = null;

			var ignore = this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
			{
				await new MessageDialog(
                    string.Format(resourceLoader.GetString("IDS_DeviceDisconnected"), AppObjects.Instance.DeviceInfo.DeviceName)).ShowAsync();
                Frame.Navigate(typeof(ScanAndConnectPage));
			});
		}
	}
}
