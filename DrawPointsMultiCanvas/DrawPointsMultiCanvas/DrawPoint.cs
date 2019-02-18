using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPointsMultiCanvas
{
    class DrawPoint
    {
        public DrawPoint()
        {

        }

        public void Start()
        {
            // generate a timer thread   
        }

        public void Stop()
        {

        }

        private void TimerThread()
        {

        }

        //List<Circle> circle;
        //Paint paint;
        //Point size;
        //static final int BALL_NUMBER = 500;

        //public RandomView(Context context)
        //{
        //    super(context);

        //    circle = new ArrayList<>();                 //円のリストを作成します
        //    Random rnd = new Random();                  //乱数を発生させます
        //    paint = new Paint();

        //    for (int i = 0; i < BALL_NUMBER; i++)
        //    {
        //        Circle c = new Circle();
        //        c.x = rnd.nextInt(1440);        //ランダムなX座標の取得
        //        c.y = rnd.nextInt(2560);        //ランダムなY座標の取得
        //        c.r = rnd.nextInt(256);         //ランダムな赤要素の取得
        //        c.g = rnd.nextInt(256);         //ランダムな緑要素の取得
        //        c.b = rnd.nextInt(256);         //ランダムな青要素の取得
        //        c.s = rnd.nextInt(50);          //ランダムな円の大きさを取得
        //        circle.add(c);
        //    }
        //}
        //protected void onDraw(Canvas cs)
        //{
        //    super.onDraw(cs);

        //    for (int i = 0; i < BALL_NUMBER; i++)
        //    {
        //        Circle cl = circle.get(i);              //リストから円を取り出す

        //        paint.setColor(Color.rgb(cl.r, cl.g, cl.b));
        //        paint.setStyle(Paint.Style.FILL);
        //        cs.drawCircle(cl.x, cl.y, cl.s, paint);    //円の描画
        //    }
        //}
    }
}
