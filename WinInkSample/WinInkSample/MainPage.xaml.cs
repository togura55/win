using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace WinInkSample
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Symbol UndoOps = (Symbol)0xE10E;    // Undo

        public MainPage()
        {
            this.InitializeComponent();

            // InkCanvasの初期化
            // サポートするインク入力デバイスのタイプをセット
            inkCanvas.InkPresenter.InputDeviceTypes =
                Windows.UI.Core.CoreInputDeviceTypes.Mouse |
                Windows.UI.Core.CoreInputDeviceTypes.Pen;

            //           var resource = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            //           this.TextBlock_DescriptionTitle.Text = resource.GetString("IDS_DESC_TITLE");
            //           this.TextBlock_DescriptionContents.Text = resource.GetString("IDS_DESC_CONTENTS_1");

            // 初期値としてのストロークの属性をセット
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Windows.UI.Colors.Black;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);

            // InkPresenterの既定は、2番手的なアフォーダンス（ペンの胴体ボタン、
            // マウスの右ボタンなど）によって、変更された入力データをインクデータとして処理します。
            // バックグラウンドのインクスレッドではなく、アプリのUIスレッドで
            // 変更された入力データをカスタム処理のためにアプリに渡すには、
            // InputProcessingConfiguration.RightDragActionをLeaveUnprocessedに設定します。
            inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction =
                InkInputRightDragAction.LeaveUnprocessed;

            // 変更された入力からの未処理のポインターイベントをリッスンします。 
            // 入力は、選択機能を提供するために使用されます。
            inkCanvas.InkPresenter.UnprocessedInput.PointerPressed +=
                UnprocessedInput_PointerPressed;
            inkCanvas.InkPresenter.UnprocessedInput.PointerMoved +=
                UnprocessedInput_PointerMoved;
            inkCanvas.InkPresenter.UnprocessedInput.PointerReleased +=
                UnprocessedInput_PointerReleased;

            // 新しいインクデータをリッスンするかストロークを消去して、
            // 選択UIをクリーンアップします。
            inkCanvas.InkPresenter.StrokeInput.StrokeStarted +=
                StrokeInput_StrokeStarted;
            inkCanvas.InkPresenter.StrokesErased +=
                InkPresenter_StrokesErased;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCanvasSize(RootGrid, outputGrid, inkCanvas);
        }

        private static void UpdateCanvasSize(FrameworkElement root, FrameworkElement output, FrameworkElement inkCanvas)
        {
            output.Width = root.ActualWidth;
            output.Height = root.ActualHeight / 2;
            inkCanvas.Width = root.ActualWidth;
            inkCanvas.Height = root.ActualHeight / 2;
        }

        private void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            try
            {
                int i = 0;
            }
            catch(Exception ex)
            {
                throw new NotImplementedException(ex.ToString());
            }
        }

        private void StrokeInput_StrokeStarted(InkStrokeInput sender, PointerEventArgs args)
        {
            try
            {
                int i = 0;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.ToString());
            }
        }

        private void UnprocessedInput_PointerReleased(InkUnprocessedInput sender, PointerEventArgs args)
        {
            try
            {
                int i = 0;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.ToString());
            }
        }

        private void UnprocessedInput_PointerMoved(InkUnprocessedInput sender, PointerEventArgs args)
        {
            try
            {
                int i = 0;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.ToString());
            }
        }

        private void UnprocessedInput_PointerPressed(InkUnprocessedInput sender, PointerEventArgs args)
        {
            try
            {
                int i = 0;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException(ex.ToString());
            }
        }

        /// <summary>
        /// Undo toolbar button procedure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToolButton_Undo(object sender, RoutedEventArgs e)
        {
            UndoLastStorke();
        }

        /// <summary>
        /// Delete a last stroke
        /// </summary>
        private void UndoLastStorke()
        {
            IReadOnlyList<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            if (strokes.Count > 0)
            {
                strokes[strokes.Count - 1].Selected = true;
                inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
            }
        }
    }
}
