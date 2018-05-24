using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StuManApp
{
    public sealed partial class Item1 : Page
    {
        public static StuViewModel stuViewModel = new StuViewModel();
        public static SfDataGrid dataGrid = new SfDataGrid();

        public Item1()
        {

            this.InitializeComponent();
            grid.Children.Add(dataGrid);
            Grid.SetRow(dataGrid, 1);
            dataGrid.AutoGenerateColumns = false;
            dataGrid.SelectionMode = GridSelectionMode.Extended;
            dataGrid.ItemsSource = stuViewModel.Students;
            dataGrid.AllowEditing = true;
            dataGrid.ShowRowHeader = true;
            dataGrid.AllowDeleting = true;
            dataGrid.AllowSorting = false;



            dataGrid.Columns.Add(new GridTextColumn() { HeaderText = "姓名", MappingName = "Name" });
            dataGrid.Columns.Add(new GridTextColumn() { HeaderText = "性别", MappingName = "Sex" });
            dataGrid.Columns.Add(new GridTextColumn() { HeaderText = "学号", MappingName = "Stuid" });
            dataGrid.Columns.Add(new GridTextColumn() { HeaderText = "班级", MappingName = "Clazz" });
            dataGrid.Columns.Add(new GridTextColumn() { HeaderText = "专业", MappingName = "Major" });
        }

        private void addRow_Click(object sender, RoutedEventArgs e)
        {

            var select = dataGrid.SelectionController.SelectedRows;
            if (select.Count == 0)
            {
                Student newStudent = new Student();
                stuViewModel.Students.Insert(stuViewModel.Students.Count, newStudent);
                StudentList._stuList.Add(newStudent);

                return;
            }
            foreach (GridRowInfo info in select)
            {
                Student newStudent = new Student();
                stuViewModel.Students.Insert(info.RowIndex, newStudent);
                StudentList._stuList.Insert(info.RowIndex, newStudent);
            }

        }
        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            List<int> temp = new List<int>();
            var select = dataGrid.SelectionController.SelectedRows;
            foreach (GridRowInfo info in select)
            {
                temp.Add(info.RowIndex - 1);

            }
            for (int i = 0; i < temp.Count; ++i)
            {
                stuViewModel.Students.RemoveAt(temp[i] - i);
                StudentList._stuList.RemoveAt(temp[i] - i);
            }
        }

        private void sortButton_Click(object sender, RoutedEventArgs e)
        {
            StudentList.Sort();
            stuViewModel.Init();
        }

        private void findButton_Click(object sender, RoutedEventArgs e)
        {
            int result = -1;
            var task = InputTextDialogAsync();

            var task2 = task.ContinueWith(t =>
            {
                try { result = StudentList.FindItem(int.Parse(t.Result)); }
                catch (FormatException fe)
                {
                    NotFindItemAsync();
                    return;
                }


            });
            var awaiter = task2.GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                if (result == -1)
                {
                    NotFindItemAsync();
                    return;
                }
                var record = dataGrid.GetRecordAtRowIndex(result + 1);
                dataGrid.SelectedItem = record;
                dataGrid.ScrollInView(new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(result + 1, 3));

            });



        }
        private async Task NotFindItemAsync()
        {
            var dialog = new MessageDialog("数据表中无此元素", "消息提示");

            dialog.Commands.Add(new UICommand("确定", cmd => { }, commandId: 0));
            dialog.DefaultCommandIndex = 0;
            await dialog.ShowAsync();
        }
        private async System.Threading.Tasks.Task<string> InputTextDialogAsync()
        {
            TextBox inputTextBox = new TextBox();
            inputTextBox.AcceptsReturn = false;
            inputTextBox.Height = 32;
            ContentDialog dialog = new ContentDialog();
            dialog.Content = inputTextBox;
            dialog.Title = "查找";
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "查找";
            dialog.SecondaryButtonText = "取消";
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "";
        }
    }


}
