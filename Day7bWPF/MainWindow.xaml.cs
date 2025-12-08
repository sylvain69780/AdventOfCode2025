using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Day7bWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WriteableBitmap bitmap;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var (width, height) = (20,20);
            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray32Float, null);

            float[] pixels = new float[height * width]; // 4 bytes par pixel

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    int index = (y * width + x);
                    pixels[index] = (float)Math.Sqrt((x-10)* (x - 10)+(y-10)*(y-10))/width;    
                }
            }

            // Écrire tous les pixels en une seule fois
            int stride = width * sizeof(float);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            DataImage.Source = bitmap;
        }
    }
}