using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.UI.Core;
using Windows.UI.Input.Inking;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace MultiView
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class SecondaryPage : Page
    {
        public SecondaryPage()
        {
            this.InitializeComponent();

            Loaded += SecondaryPage_Loaded;

        }

        private void SecondaryPage_Loaded(object sender, RoutedEventArgs e)
        {
            // 描画属性を作成する
            InkDrawingAttributes attributes = new InkDrawingAttributes();
            attributes.Color = Windows.UI.Colors.Red;   // ペンの色
            attributes.Size = new Size(10, 2);          // ペンのサイズ
            attributes.IgnorePressure = false;          // ペンの圧力を使用するかどうか
            attributes.FitToCurve = false;

            // インクキャンバスに属性を設定する
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(attributes);
            // マウスとペンによる描画を許可する
            inkCanvas.InkPresenter.InputDeviceTypes =
                CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;
        }

        private void CompleteLine(InkUnprocessedInput sender, PointerEventArgs args)
        {
            // https://stackoverflow.com/questions/44332669/draw-line-onto-inkcanvas
            //
            List<InkPoint> points = new List<InkPoint>();
            InkStrokeBuilder builder = new InkStrokeBuilder();


            InkPoint pointOne = new InkPoint(new Point(line.X1, line.Y1), 0.5f);
            points.Add(pointOne);
            InkPoint pointTwo = new InkPoint(new Point(line.X2, line.Y2), 0.5f);
            points.Add(pointTwo);

            InkStroke stroke = builder.CreateStrokeFromInkPoints(points, System.Numerics.Matrix3x2.Identity);
            InkDrawingAttributes ida = inker.InkPresenter.CopyDefaultDrawingAttributes();
            stroke.DrawingAttributes = ida;
            inker.InkPresenter.StrokeContainer.AddStroke(stroke);
            selectionCanvas.Children.Remove(line);
        }
    }
}
