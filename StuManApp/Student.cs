using System.ComponentModel;

namespace StuManApp
{
    public class Student : INotifyPropertyChanged
    {
        string name;
        string sex;
        string stuid;
        string clazz;
        string major;

        public Student()
        {
        }

        public Student(string name, string sex, string stuid, string clazz, string major)
        {
            this.name = name;
            this.sex = sex;
            this.stuid = stuid;
            this.clazz = clazz;
            this.major = major;

        }

        public string Name { get => name; set => name = value; }

        public string Sex { get => sex; set => sex = value; }

        public string Stuid { get => stuid; set => stuid = value; }

        public string Clazz { get => clazz; set => clazz = value; }

        public string Major { get => major; set => major = value; }


        public event PropertyChangedEventHandler PropertyChanged;
    }


}
