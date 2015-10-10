using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FindingStar
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        public void button_Click(object sender, RoutedEventArgs e)
        {
            NewMethod();
            
        }

        public void NewMethod()
        {
            //#region create method 1

            //Path path = new Path();

            //PathGeometry pathGeometry = new PathGeometry();

            //PathFigure pathFigure = new PathFigure();

            //pathFigure.StartPoint = new Point(400, 300);

            //PathSegmentCollection segmentCollection = new PathSegmentCollection();

            //segmentCollection.Add(new LineSegment() { Point = new Point(600, 100) });

            //pathFigure.Segments = segmentCollection;

            //pathGeometry.Figures = new PathFigureCollection() { pathFigure };

            //path.Data = pathGeometry;

            //path.Stroke = new SolidColorBrush(Colors.BlueViolet);

            //path.StrokeThickness = 3;

            //main.Children.Add(path);

            //#endregion



            //#region create method2

            //Path pp = new Path();

            //pp.Stroke = new SolidColorBrush(Colors.Blue);

            //pp.StrokeThickness = 3;

            //StreamGeometry geometry = new StreamGeometry();

            //geometry.FillRule = FillRule.Nonzero; //声前F0还是F1,现在是F1

            //using (StreamGeometryContext ctx = geometry.Open())

            //{

            //    ctx.BeginFigure(new Point(30, 60), true, true);

            //    ctx.LineTo(new Point(150, 600), true, false);

            //}

            //geometry.Freeze();

            //pp.Data = geometry;

            //main.Children.Add(pp);

            //#endregion



            #region create method3

            Path pp3 = new Path();

            //pp3.Data

            pp3.Stroke = new SolidColorBrush(Colors.Red);

            pp3.StrokeThickness = 3;

            GeometryConverter gc = new GeometryConverter();

            pp3.Data = (Geometry)gc.ConvertFromString("M 20,30 500,100");

            main.Children.Add(pp3);

            #endregion
        }
    }
}
