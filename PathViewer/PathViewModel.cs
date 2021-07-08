using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace PathViewer
{
    public class PathViewModel : INotifyPropertyChanged
    {
        private double zoom = 1.0;
        private string strokeColor = nameof(Colors.Black);
        private int strokeThickness = 2;
        private string fillColor = nameof(Colors.Transparent);
        private string data = "M 0,90 A 90,90 0 0 0 180,90 M 30,30 A 15,15 0 0 0 60,30 M 30,30 A 15,15 0 0 1 60,30 M 120,30 A 15,15 0 0 0 150,30 M 120,30 A 15,15 0 0 1 150,30";

        public event PropertyChangedEventHandler PropertyChanged;

        public string Data { get => data; set { data = value; OnPropertyChanged(); } }

        public double Zoom { get => zoom; set { zoom = value; OnPropertyChanged(); } }

        public List<string> ColorsList => typeof(Colors).GetProperties().Select(p => p.Name).ToList();

        public List<int> Thicknesses => Enumerable.Range(1, 20).ToList();

        public string StrokeColor { get => strokeColor; set { strokeColor = value; OnPropertyChanged(); } }

        public int StrokeThickness { get => strokeThickness; set { strokeThickness = value; OnPropertyChanged(); } }

        public string FillColor { get => fillColor; set { fillColor = value; OnPropertyChanged(); } }

        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
