using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Wacom.Devices;
using Wacom.SmartPadCommunication;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;

namespace WillDevicesSampleApp
{
	public class AppObjects
	{
        private static ResourceLoader resourceLoader;

        public static readonly AppObjects Instance = new AppObjects();
		private static readonly string SaveFileName = "SavedData";

		private AppObjects()
		{
            resourceLoader = ResourceLoader.GetForCurrentView();

            AppId = new SmartPadClientId(0xFA, 0xAB, 0xC1, 0xE0, 0xF1, 0x77);
		}

		public IDigitalInkDevice Device
		{
			get;
			set;
		}

		public SmartPadClientId AppId
		{
			get;
		}

		public InkDeviceInfo DeviceInfo
		{
			get;
			set;
		}

		public static async Task SerializeDeviceInfoAsync(InkDeviceInfo deviceInfo)
		{
			try
			{
				StorageFile storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(SaveFileName, CreationCollisionOption.ReplaceExisting);

				using (Stream stream = await storageFile.OpenStreamForWriteAsync())
				{
					deviceInfo.ToStream(stream);
				}
			}
			catch (Exception)
			{
			}
		}

		public static async Task<InkDeviceInfo> DeserializeDeviceInfoAsync()
		{
			try
			{
				StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync(SaveFileName);

				using (Stream stream = await storageFile.OpenStreamForReadAsync())
				{
					return await InkDeviceInfo.FromStreamAsync(stream);
				}
			}
			catch (Exception)
			{
			}

			return null;
		}

		public static Matrix CalculateTransform(uint deviceWidth, uint deviceHeight, uint ptSizeInMicrometers, float scale)
		{
			float scaleFactor = ptSizeInMicrometers * micrometerToDip * scale;

			ScaleTransform st = new ScaleTransform();
			st.ScaleX = scaleFactor;
			st.ScaleY = scaleFactor;

//			RotateTransform rt = new RotateTransform();
            //			rt.Angle = 90;

			TranslateTransform tt = new TranslateTransform();
//			tt.X = deviceHeight * scaleFactor;
            //tt.X = 0;
            //tt.Y = 0;
            tt.X = deviceWidth * scaleFactor;
            tt.Y = deviceHeight * scaleFactor;

            TransformGroup tg = new TransformGroup();
			tg.Children.Add(st);
//			tg.Children.Add(rt);
//			tg.Children.Add(tt);

			return tg.Value;
		}

		public static string GetStringForDeviceStatus(DeviceStatus deviceStatus)
		{
			string text = string.Empty;

			switch (deviceStatus)
			{
				case DeviceStatus.Idle:
					break;

				case DeviceStatus.Reconnecting:
                    text =
                        resourceLoader.GetString("IDS_Connecting");
					break;

				case DeviceStatus.Syncing:
					text =
                         resourceLoader.GetString("IDS_Syncing");
					break;

				case DeviceStatus.CapturingRealTimeInk:
					text =
                         resourceLoader.GetString("IDS_RealtimeInkEnabled");
					break;

				case DeviceStatus.ExpectingConnectionConfirmation:
					text =
                          resourceLoader.GetString("IDS_TapButtonConfirm");
					break;

				case DeviceStatus.ExpectingReconnect:
					text =
                           resourceLoader.GetString("IDS_TabButtonRestore");
					break;

				case DeviceStatus.ExpectingUserConfirmationMode:
					text =
                           resourceLoader.GetString("IDS_PressAndHoldUserConfirm");
					break;

				case DeviceStatus.NotAuthorizedConnectionNotConfirmed:
					text =
                           resourceLoader.GetString("IDS_PeriodExpired");
					break;

				case DeviceStatus.NotAuthorizedDeviceInUseByAnotherHost:
					text =
                           resourceLoader.GetString("IDS_DeviceUseAnother");
					break;

				case DeviceStatus.NotAuthorizedGeneralError:
					text =
                           resourceLoader.GetString("IDS_AuthorizationFailed");
					break;
			}

			return text;
		}

		public async Task<bool> ShowPairingModeEnabledDialogAsync()
		{
            var dialog = new MessageDialog(string.Format(resourceLoader.GetString("IDS_DevicePairingMode"), DeviceInfo.DeviceName));
            dialog.Commands.Add(new UICommand(resourceLoader.GetString("IDS_KeepUsingDevice")) { Id = 0 });
			dialog.Commands.Add(new UICommand(resourceLoader.GetString("IDS_ForgetDevice")) { Id = 1 });
            
            var dialogResult = await dialog.ShowAsync();

			return ((int)dialogResult.Id == 0);
		}

		public const float micrometerToDip = 96.0f / 25400.0f;
	}
}