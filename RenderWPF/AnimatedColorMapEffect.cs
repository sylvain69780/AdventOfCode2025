// ============================================================
// 2. AnimatedColorMapEffect.cs
// ============================================================
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace WpfShaderDemo
{
    public class AnimatedColorMapEffect : ShaderEffect
    {
        private static readonly PixelShader pixelShader1 = new()
        {
            UriSource = new Uri("pack://application:,,,/AnimatedColorMapEffect.ps")
        };
        private static PixelShader pixelShader = pixelShader1;

        public AnimatedColorMapEffect()
        {
            PixelShader = pixelShader;
            UpdateShaderValue(InputProperty);
            UpdateShaderValue(ColorMapTypeProperty);
            UpdateShaderValue(TimeProperty);
            UpdateShaderValue(AnimationTypeProperty);
            UpdateShaderValue(AnimationSpeedProperty);
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty(
                "Input",
                typeof(AnimatedColorMapEffect),
                0);

        //public Brush Input
        //{
        //    get => (Brush)GetValue(InputProperty);
        //    set => SetValue(InputProperty, value);
        //}

        public static readonly DependencyProperty ColorMapTypeProperty =
            DependencyProperty.Register(
                "ColorMapType",
                typeof(double),
                typeof(AnimatedColorMapEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double ColorMapType
        {
            get => (double)GetValue(ColorMapTypeProperty);
            set => SetValue(ColorMapTypeProperty, value);
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(
                "Time",
                typeof(double),
                typeof(AnimatedColorMapEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double Time
        {
            get => (double)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        public static readonly DependencyProperty AnimationTypeProperty =
            DependencyProperty.Register(
                "AnimationType",
                typeof(double),
                typeof(AnimatedColorMapEffect),
                new UIPropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public double AnimationType
        {
            get => (double)GetValue(AnimationTypeProperty);
            set => SetValue(AnimationTypeProperty, value);
        }

        public static readonly DependencyProperty AnimationSpeedProperty =
            DependencyProperty.Register(
                "AnimationSpeed",
                typeof(double),
                typeof(AnimatedColorMapEffect),
                new UIPropertyMetadata(1.0, PixelShaderConstantCallback(3)));

        public double AnimationSpeed
        {
            get => (double)GetValue(AnimationSpeedProperty);
            set => SetValue(AnimationSpeedProperty, value);
        }
    }
}
