using Syncfusion.XlsIO;
using System;
using System.Collections;
using System.IO;
using Windows.Storage;

namespace StuManApp
{
    public class StudentList
    {
        private static int M = 15;    //混合查找的切换标志，元素数大于M采用快排，低于M采用直插排序
        public static ArrayList _stuList = new ArrayList(); //储存数据的数组


        public StudentList()
        {
            readLocalFileAsync();  //读入存在缓存中的数据文件

        }
        //读取Excel文件
        private async void readLocalFileAsync()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file;
            //判断文件是否存在，不存在则新建
            if (File.Exists(Path.Combine(folder.Path, "data.xlsx")))
            {
                file = await folder.GetFileAsync("data.xlsx");
            }
            else
            {
                file = await folder.CreateFileAsync("data.xlsx");
                ExcelEngine excelEngine1 = new ExcelEngine();

                IApplication application1 = excelEngine1.Excel;

                application1.DefaultVersion = ExcelVersion.Excel2013;


                IWorkbook workbook1 = application1.Workbooks.Create(1);


                IWorksheet sheet = workbook1.Worksheets.Create();

                await workbook1.SaveAsAsync(file);

                workbook1.Close();

                excelEngine1.Dispose();
            }
            //打开缓存路径的文件
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
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
        //排序
        public static void Sort()
        {
            try
            {
                hybridSort(0, _stuList.Count - 1);
                MainPage.isSotred = true;
            }
            catch (FormatException e)//处理当需排序元素非数字的情况
            {
                return;
            }

        }
        //查找
        public static int FindItem(int id)
        {
            if (MainPage.isSotred)
            {
                return InsertValueSearch(0, _stuList.Count - 1, id);
            }
            else
            {
                return Search(id);
            }
        }
        //顺序查找，但元素未排序时使用
        static int Search(int key)
        {
            for (int i = 0; i < _stuList.Count; ++i)
            {
                if (int.Parse(((Student)_stuList[i]).Stuid) == key)
                {
                    return i;
                }

            }
            return -1;
        }
        //综合查找
        static int InsertValueSearch(int left, int right, int key)
        {
            while (left < right)
            {
                if (int.Parse(((Student)_stuList[left]).Stuid) == key)
                {
                    return left;
                }
                if (int.Parse(((Student)_stuList[right]).Stuid) == key)
                {
                    return right;
                }
                int middle = (left + right) / 2;
                //int middle = left + (right - left) * ((key -int.Parse(((Student)_stuList[left]).Stuid) ) / (int.Parse(((Student)_stuList[right]).Stuid) - int.Parse(((Student)_stuList[left]).Stuid)));
                if (int.Parse(((Student)_stuList[middle]).Stuid) == key)
                {
                    return middle;
                }
                if (key < int.Parse(((Student)_stuList[middle]).Stuid))
                {
                    right = middle - 1;
                }
                else
                {
                    left = middle + 1;
                }
            }
            return -1;
        }

        static void hybridSort(int left, int right)
        {
            QuickSort(left, right);
            InsertSort(left, right);
        }
        static void QuickSort(int left, int right)
        {
            if (right - left > M)
            {
                int pos = Partition(left, right);
                QuickSort(left, pos - 1);
                QuickSort(pos + 1, right);
            }
        }
        //把rd的值赋给ld
        static void Replace(int ld, int rd)
        {
            ((Student)_stuList[ld]).Name = ((Student)_stuList[rd]).Name;
            ((Student)_stuList[ld]).Sex = ((Student)_stuList[rd]).Sex;
            ((Student)_stuList[ld]).Stuid = ((Student)_stuList[rd]).Stuid;
            ((Student)_stuList[ld]).Major = ((Student)_stuList[rd]).Major;
            ((Student)_stuList[ld]).Clazz = ((Student)_stuList[rd]).Clazz;

        }
        //交换rd和ld的值
        static void Swap(int ld, int rd)
        {
            Student temp = new Student(((Student)_stuList[ld]).Name, ((Student)_stuList[ld]).Sex, ((Student)_stuList[ld]).Stuid, ((Student)_stuList[ld]).Clazz, ((Student)_stuList[ld]).Major);
            ((Student)_stuList[ld]).Name = ((Student)_stuList[rd]).Name;
            ((Student)_stuList[ld]).Sex = ((Student)_stuList[rd]).Sex;
            ((Student)_stuList[ld]).Stuid = ((Student)_stuList[rd]).Stuid;
            ((Student)_stuList[ld]).Major = ((Student)_stuList[rd]).Major;
            ((Student)_stuList[ld]).Clazz = ((Student)_stuList[rd]).Clazz;

            ((Student)_stuList[rd]).Name = temp.Name;
            ((Student)_stuList[rd]).Sex = temp.Sex;
            ((Student)_stuList[rd]).Stuid = temp.Stuid;
            ((Student)_stuList[rd]).Major = temp.Major;
            ((Student)_stuList[rd]).Clazz = temp.Clazz;

        }
        static void InsertSort(int left, int right)
        {
            for (int i = left + 1; i <= right; i++)
            {
                if (int.Parse(((Student)_stuList[i]).Stuid) < int.Parse(((Student)_stuList[i - 1]).Stuid))
                {
                    int j = i;
                    Student tmp = new Student(((Student)_stuList[j]).Name, ((Student)_stuList[j]).Sex, ((Student)_stuList[j]).Stuid, ((Student)_stuList[j]).Clazz, ((Student)_stuList[j]).Major);
                    while (j - 1 >= 0 && int.Parse(tmp.Stuid) < int.Parse(((Student)_stuList[j - 1]).Stuid))
                    {
                        Replace(j, j - 1);
                        j--;
                    }
                    ((Student)_stuList[j]).Name = tmp.Name;
                    ((Student)_stuList[j]).Sex = tmp.Sex;
                    ((Student)_stuList[j]).Stuid = tmp.Stuid;
                    ((Student)_stuList[j]).Major = tmp.Major;
                    ((Student)_stuList[j]).Clazz = tmp.Clazz;
                }
            }
        }
        static int Partition(int left, int right)
        {
            int mid = (left + right) / 2;
            if (mid != left)
            {
                if (int.Parse(((Student)_stuList[left]).Stuid) > int.Parse(((Student)_stuList[mid]).Stuid))
                {
                    Swap(left, mid);
                }
                if (int.Parse(((Student)_stuList[mid]).Stuid) > int.Parse(((Student)_stuList[right]).Stuid))
                {
                    Swap(mid, right);
                }
                if (int.Parse(((Student)_stuList[mid]).Stuid) > int.Parse(((Student)_stuList[left]).Stuid))
                {
                    Swap(mid, left);
                }
            }
            Student pivot = new Student(((Student)_stuList[left]).Name, ((Student)_stuList[left]).Sex, ((Student)_stuList[left]).Stuid, ((Student)_stuList[left]).Clazz, ((Student)_stuList[left]).Major);
            int i = left; int j = right;
            while (i < j)
            {
                while (i < j && int.Parse(((Student)_stuList[j]).Stuid) >= int.Parse(pivot.Stuid)) j--;
                if (i < j)
                {
                    Replace(i, j); i++;
                }
                while (i < j && int.Parse(((Student)_stuList[i]).Stuid) <= int.Parse(pivot.Stuid)) i++;
                if (i < j)
                {
                    Replace(j, i); j--;
                }
            }
            ((Student)_stuList[i]).Name = pivot.Name;
            ((Student)_stuList[i]).Sex = pivot.Sex;
            ((Student)_stuList[i]).Stuid = pivot.Stuid;
            ((Student)_stuList[i]).Major = pivot.Major;
            ((Student)_stuList[i]).Clazz = pivot.Clazz;
            return i;
        }

    }
}
