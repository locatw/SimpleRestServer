using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 を参照してください

namespace SampleApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private AppServiceConnection appServiceConnection = null;

        public MainPage()
        {
            InitializeComponent();
            Initialize();
        }

        private async void Initialize()
        {
            appServiceConnection = new AppServiceConnection();
            appServiceConnection.PackageFamilyName = Package.Current.Id.FamilyName;
            appServiceConnection.AppServiceName = "RestServerService";

            var result = await appServiceConnection.OpenAsync();
            if (result == AppServiceConnectionStatus.Success)
            {
                var message = new ValueSet();
                message.Add("Command", "Initialize");

                var response = await appServiceConnection.SendMessageAsync(message);
                if (response.Status != AppServiceResponseStatus.Success)
                {
                    throw new Exception("failed to send message");
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (appServiceConnection != null)
            {
                appServiceConnection.Dispose();
            }
        }
    }
}
