using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace StuManApp
{

    sealed partial class App : Application
    {

        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            ApplicationView.PreferredLaunchViewSize = new Size(695, 700);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;


        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            
            if (args.Kind == ActivationKind.VoiceCommand)
            {
                var commandArgs = args as VoiceCommandActivatedEventArgs;
                Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = commandArgs.Result;

                string voiceCommandName = speechRecognitionResult.RulePath[0];
                if (voiceCommandName == "Search")
                {
                    string item = speechRecognitionResult.SemanticInterpretation.Properties["Sid"][0];
                    int result=StudentList.FindItem(int.Parse(item));
                    if (result != -1)
                    {
                        var record = Item1.dataGrid.GetRecordAtRowIndex(result + 1);
                        Item1.dataGrid.SelectedItem = record;
                        Item1.dataGrid.ScrollInView(new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(result + 1, 3));
                    }
                    
                }
            }
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            try
            {
                var storageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///CortanaCommad.xml"));
                await VoiceCommandDefinitionManager.
                    InstallCommandDefinitionsFromStorageFileAsync(storageFile);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            Frame rootFrame = Window.Current.Content as Frame;
            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            CoreApplicationViewTitleBar coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            if (rootFrame == null)
            {

                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {

                }


                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {

                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                Window.Current.Activate();
            }
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            var saveTask = MainPage.ExportToExcelAsync(0);
            saveTask.ContinueWith(t => deferral.Complete());
        }
    }
}
