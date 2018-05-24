using Windows.UI.Xaml.Controls;
using Windows.System.Profile;
using Windows.Storage.Pickers;
using Windows.Storage;
using System;
using Windows.ApplicationModel.Core;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace StuManApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public SplitView MainSpiltView { get; set; }

        public StudentList StudentList = new StudentList();

        public static bool isSotred = false;
        public MainPage()
        {
            this.InitializeComponent();



            SpiltViewPaneListBox.SelectedIndex = 0;

            MyFrame.Navigate(typeof(Item1));



        }

        private void MySplitView_PaneClosed(SplitView sender, object args)
        {
            MySplitView.Content.Opacity = 1;
            MySplitView.PaneBackground = this.Resources["SystemControlAltHighAcrylicWindowBrush"] as Brush;
        }

        private void HamburgerButton_Click()
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;

            if (MySplitView.IsPaneOpen)
            {
                MySplitView.PaneBackground = new SolidColorBrush(Windows.UI.Colors.White);
                MySplitView.Content.Opacity = 0.4;
            }

        }

        private void SpiltViewPaneListBox_ItemClick(object sender, ItemClickEventArgs e)
        {

            StackPanel item = (StackPanel)e.ClickedItem;
            if (item.Name.Equals("OpenFile"))
            {
                OpenFileAsync();
                MySplitView.IsPaneOpen = false;

            }
            else if (item.Name.Equals("Export"))
            {
                var task = ExportToExcelAsync(1);
                MySplitView.IsPaneOpen = false;
            }
            else if (item.Name.Equals("Exit"))
            {
                var task = ExportToExcelAsync(0);
                task.ContinueWith(t => CoreApplication.Exit());
                MySplitView.IsPaneOpen = false;
            }
            else if (item.Name.EndsWith("Menu"))
            {
                HamburgerButton_Click();
            }

        }
        private async void OpenFileAsync()
        {
            isSotred = false;
            FileOpenPicker openPicker = new FileOpenPicker();

            openPicker.FileTypeFilter.Add(".xlsx");

            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;

            StorageFile file = await openPicker.PickSingleFileAsync();

            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            if (file == null)
            {
                return;
            }
            IWorkbook workbook = await application.Workbooks.OpenAsync(file);
            workbook.Version = ExcelVersion.Excel2016;
            IWorksheet worksheet = workbook.Worksheets[0];
            IRange[] ranges = worksheet.Rows;
            StudentList._stuList.Clear();
            for (int i = 2; i <= ranges.Length; ++i)
            {
                StudentList._stuList.Add(new Student(worksheet[i, 1].DisplayText, worksheet[i, 2].DisplayText, worksheet[i, 3].DisplayText, worksheet[i, 4].DisplayText, worksheet[i, 5].DisplayText));

            }
            Item1.stuViewModel.Init();
            workbook.Close();
            excelEngine.Dispose();




        }

        public static async System.Threading.Tasks.Task ExportToExcelAsync(int state)
        {
            var options = new ExcelExportingOptions();

            options.ExcelVersion = ExcelVersion.Excel2013;

            var excelEngine = Item1.dataGrid.ExportToExcel(Item1.dataGrid.View, options);

            IWorkbook workBook = excelEngine.Excel.Workbooks[0];

            if (state == 1)
            {
                var savePicker = new FileSavePicker
                {
                    SuggestedStartLocation = PickerLocationId.Desktop,
                    SuggestedFileName = "Sample"
                };

                if (workBook.Version == ExcelVersion.Excel97to2003)
                    savePicker.FileTypeChoices.Add("Excel File (.xls)", new List<string>() { ".xls" });

                else
                    savePicker.FileTypeChoices.Add("Excel File (.xlsx)", new List<string>() { ".xlsx" });

                var storageFile = await savePicker.PickSaveFileAsync();

                if (storageFile != null)
                    await workBook.SaveAsAsync(storageFile);
                workBook.Close();

                excelEngine.Dispose();
            }
            else
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file = await folder.CreateFileAsync("data.xlsx", CreationCollisionOption.ReplaceExisting);
                if (file != null)
                    await workBook.SaveAsAsync(file);
                workBook.Close();

                excelEngine.Dispose();

            }
        }

    }



    public static class DeviceFamily
    {
        public static Devices GetDeviceFamily()
        {
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                return Devices.Mobile;
            return Devices.Desktop;
        }
    }

    public enum Devices
    {
        Desktop,
        Mobile
    }
}
