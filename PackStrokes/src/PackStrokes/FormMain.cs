using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Wacom.Ink;

namespace PackStrokes
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Text = @"PackStroke";
            Pbtn_start.Text = @"Start";
        }

        private void Pbtn_start_Click(object sender, EventArgs e)
        {
            StrokeCollection sc = new StrokeCollection();

            // Define Regions
            sc.CreateRegion(10, 10, 110, 60);
            sc.CreateRegion(200, 10, 300, 60);


            // Simurate Input Strokes
            List<float> data;
            uint stride;
            Path p;

            // 1st stroke
            data = new List<float> { 20,21,1, 20,22,1};
            stride = 3;
            p = new Path(data, stride, PathFormat.XYA);
            sc.CreateStroke(p);

            // 2nd stroke
            data = new List<float> { 21,30,1, 25,30,1, 24,40,1 };
            stride = 3;
            p = new Path(data, stride, PathFormat.XYA);
            sc.CreateStroke(p);

            // 3rd stroke
            data = new List<float> { 210,13,1, 212,30,1, 220,23,1 };
            stride = 3;
            p = new Path(data, stride, PathFormat.XYA);
            sc.CreateStroke(p);


            // ストロークをリージョンに当てはめる
            // 後から当てはめるか、
            // リアルタイムに当てはめるか sc.Regionがあるかどうか
            sc.StrokesToRegion();
        }

    }
}
