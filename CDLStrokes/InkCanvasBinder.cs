using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Input.Inking;

namespace WillDevicesSampleApp
{
    //class InkCanvasBinder
    //{
        public static class InkCanvasBinder
        {
            public static InkStrokeContainer GetInkStrokes(DependencyObject obj) =>
                obj.GetValue(InkStrokesProperty) as InkStrokeContainer;

            public static void SetInkStrokes(DependencyObject obj, InkStrokeContainer value) =>
                obj.SetValue(InkStrokesProperty, value);

            public static DependencyProperty InkStrokesProperty = DependencyProperty.RegisterAttached(
                "InkStrokes", typeof(InkStrokeContainer), typeof(InkCanvasBinder),
                new PropertyMetadata(null, InkStrokesProperty_PropertyChanged));

            private static void InkStrokesProperty_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                var inkCanvas = d as InkCanvas;
                if (inkCanvas != null) inkCanvas.InkPresenter.StrokeContainer = e.NewValue as InkStrokeContainer;
            }
        }
    //}
}
