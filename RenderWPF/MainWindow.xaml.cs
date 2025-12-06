// ============================================================
// 3. MainWindow.xaml.cs
// ============================================================
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfShaderDemo
{
    public partial class MainWindow : Window
    {
        private WriteableBitmap bitmap;
        private int width = 512;
        private int height = 512;

        private DispatcherTimer animationTimer;
        private double time = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTexture();
            InitializeAnimation();
            GenerateStaticData();
        }

        private void InitializeTexture()
        {
            bitmap = new WriteableBitmap(
                width, height, 96, 96,
                PixelFormats.Bgr32, null);

            DataImage.Source = bitmap;
        }

        private void InitializeAnimation()
        {
            //    // Timer à 60 FPS
            //    animationTimer = new DispatcherTimer();
            //    animationTimer.Interval = TimeSpan.FromMilliseconds(16.67);
            //    animationTimer.Tick += AnimationTimer_Tick;
            //    animationTimer.Start(); // Démarre automatiquement
            //}

            //private void AnimationTimer_Tick(object sender, EventArgs e)
            //{
            //    // Incrémenter le temps et le passer au shader
            //    time += 0.016;
            //    AnimatedShader.Time = time;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            // Calculer le temps écoulé depuis la dernière frame
            // (e.RenderingTime contient le temps actuel)
            RenderingEventArgs renderingArgs = (RenderingEventArgs)e;
            double frameTime = renderingArgs.RenderingTime.TotalSeconds;

            // Mettre à jour le temps (ajuster l'incrément selon tes besoins)
            time = frameTime; // ou time += 0.016; si tu veux un incrément fixe
            AnimatedShader.Time = time;
        }

        // Générer des données statiques (une seule fois)
        // Le shader s'occupera de l'animation
        private void GenerateStaticData()
        {
            bitmap.Lock();
            try
            {
                unsafe
                {
                    int stride = bitmap.BackBufferStride;
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Données de base (par exemple un gradient)
                            double dx = x - width / 2.0;
                            double dy = y - height / 2.0;
                            double dist = Math.Sqrt(dx * dx + dy * dy);
                            double value = Math.Max(0, 1.0 - dist / (width / 2.0));

                            byte byteValue = (byte)(value * 255);

                            int offset = y * stride + x * 4;
                            ptr[offset] = byteValue;
                            ptr[offset + 1] = byteValue;
                            ptr[offset + 2] = byteValue;
                            ptr[offset + 3] = 255;
                        }
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        private void GeneratePatternData(string pattern)
        {
            bitmap.Lock();
            try
            {
                unsafe
                {
                    int stride = bitmap.BackBufferStride;
                    byte* ptr = (byte*)bitmap.BackBuffer;

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            double value = 0;

                            switch (pattern)
                            {
                                case "gradient":
                                    double dx = x - width / 2.0;
                                    double dy = y - height / 2.0;
                                    double dist = Math.Sqrt(dx * dx + dy * dy);
                                    value = Math.Max(0, 1.0 - dist / (width / 2.0));
                                    break;

                                case "grid":
                                    value = (Math.Sin(x * 0.1) * Math.Cos(y * 0.1) * 0.5 + 0.5);
                                    break;

                                case "checkers":
                                    value = ((x / 32) % 2 == (y / 32) % 2) ? 1.0 : 0.0;
                                    break;

                                case "diagonal":
                                    value = ((x + y) % 100) / 100.0;
                                    break;
                            }

                            byte byteValue = (byte)(value * 255);

                            int offset = y * stride + x * 4;
                            ptr[offset] = byteValue;
                            ptr[offset + 1] = byteValue;
                            ptr[offset + 2] = byteValue;
                            ptr[offset + 3] = 255;
                        }
                    }
                }

                bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
            }
            finally
            {
                bitmap.Unlock();
            }
        }

        // Event handlers
        private void ColorMapCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AnimatedShader != null)
            {
                AnimatedShader.ColorMapType = ColorMapCombo.SelectedIndex;
            }
        }

        private void AnimationCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (AnimatedShader != null)
            {
                AnimatedShader.AnimationType = AnimationCombo.SelectedIndex;
            }
        }

        private void SpeedSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (AnimatedShader != null)
            {
                AnimatedShader.AnimationSpeed = SpeedSlider.Value;
            }
        }

        private void PatternButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            string pattern = button?.Tag as string ?? "gradient";
            GeneratePatternData(pattern);
        }
    }
}
