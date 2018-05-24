namespace StuManApp
{
    public class StuViewModel
    {
        private System.Collections.ObjectModel.ObservableCollection<Student> _students;
        public System.Collections.ObjectModel.ObservableCollection<Student> Students
        {
            get { return _students; }
            set { _students = value; }
        }
        public StuViewModel()
        {
            _students = new System.Collections.ObjectModel.ObservableCollection<Student>();
            Init();
        }
        public void Init()
        {
            _students.Clear();
            foreach (Student s in StudentList._stuList)
            {
                _students.Add(s);
            }
        }

    }
}
