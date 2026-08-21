using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace libSplash
{
    class GifPictureBox : PictureBox
    {
        public enum TextColorOption { REF_PIXEL = 0, FIX_COLOR };

        private bool m_onlyLastImage = false;
        private bool m_useSingleLoop = false;
        private int m_nFrameCount = 0;

        private int m_nDrawingCount = 0;
        private System.Drawing.Imaging.FrameDimension m_dimension = null;

        private GifPictureBoxOwner m_owner = null;

        private TextColorOption m_textColorOption = TextColorOption.REF_PIXEL;
        private Point m_ptTextColorRef = new Point();
        private Color m_fixTextColor = Color.White;

        public bool UseSingleLoop
        {
            get { return m_useSingleLoop; }
            set
            {
                m_useSingleLoop = value;

                if (m_useSingleLoop)
                {
                    this.Enabled = true;
                    m_nDrawingCount = 0;
                }
            }
        }

        public bool OnlyLastImage
        {
            get { return m_onlyLastImage; }
            set { m_onlyLastImage = value; }
        }

        public new System.Drawing.Image Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;
                m_dimension = null;
                m_nDrawingCount = 0;

                if (this.Image != null)
                {
                    this.Enabled = Image.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Gif);
                    CalcFrame();

                    if (m_animator != null && m_animator.IsRunning)
                    {
                        m_animator.Stop();
                        m_animator = null;
                        m_optGIF = true;
                    }
                }
            }
        }

        public GifPictureBoxOwner Owner
        {
            get { return m_owner; }
            set { m_owner = value; }
        }

        public bool UseGIF
        {
            get { return m_optGIF; }
            set { m_optGIF = value; }
        }

        private Animator m_animator = null;
        private bool m_optGIF = false;

        public GifPictureBox()
        {
            this.DoubleBuffered = true;
            m_animator = new Animator(MakeImageList(), 5, this);
        }

        public void Run()
        {
            m_animator.Run();
            m_optGIF = false;
        }

        private List<Image> MakeImageList()
        {
            List<Image> images = new List<Image>();

            images.Add(global::libSplash.Properties.Resources._1);
            images.Add(global::libSplash.Properties.Resources._2);
            images.Add(global::libSplash.Properties.Resources._3);
            images.Add(global::libSplash.Properties.Resources._4);
            images.Add(global::libSplash.Properties.Resources._5);
            images.Add(global::libSplash.Properties.Resources._6);
            images.Add(global::libSplash.Properties.Resources._7);
            images.Add(global::libSplash.Properties.Resources._8);
            images.Add(global::libSplash.Properties.Resources._9);
            images.Add(global::libSplash.Properties.Resources._10);
            images.Add(global::libSplash.Properties.Resources._11);
            images.Add(global::libSplash.Properties.Resources._12);
            images.Add(global::libSplash.Properties.Resources._13);
            images.Add(global::libSplash.Properties.Resources._14);
            images.Add(global::libSplash.Properties.Resources._15);
            images.Add(global::libSplash.Properties.Resources._16);
            images.Add(global::libSplash.Properties.Resources._17);
            images.Add(global::libSplash.Properties.Resources._18);
            images.Add(global::libSplash.Properties.Resources._19);
            images.Add(global::libSplash.Properties.Resources._20);
            images.Add(global::libSplash.Properties.Resources._21);
            images.Add(global::libSplash.Properties.Resources._22);
            images.Add(global::libSplash.Properties.Resources._23);
            images.Add(global::libSplash.Properties.Resources._24);
            images.Add(global::libSplash.Properties.Resources._25);
            images.Add(global::libSplash.Properties.Resources._26);
            images.Add(global::libSplash.Properties.Resources._27);
            images.Add(global::libSplash.Properties.Resources._28);
            images.Add(global::libSplash.Properties.Resources._29);
            images.Add(global::libSplash.Properties.Resources._30);
            images.Add(global::libSplash.Properties.Resources._31);
            images.Add(global::libSplash.Properties.Resources._32);
            images.Add(global::libSplash.Properties.Resources._33);
            images.Add(global::libSplash.Properties.Resources._34);
            images.Add(global::libSplash.Properties.Resources._35);
            images.Add(global::libSplash.Properties.Resources._36);
            images.Add(global::libSplash.Properties.Resources._37);
            images.Add(global::libSplash.Properties.Resources._38);
            images.Add(global::libSplash.Properties.Resources._39);
            images.Add(global::libSplash.Properties.Resources._40);
            images.Add(global::libSplash.Properties.Resources._41);
            images.Add(global::libSplash.Properties.Resources._42);
            images.Add(global::libSplash.Properties.Resources._43);
            images.Add(global::libSplash.Properties.Resources._44);
            images.Add(global::libSplash.Properties.Resources._45);
            images.Add(global::libSplash.Properties.Resources._46);
            images.Add(global::libSplash.Properties.Resources._47);
            images.Add(global::libSplash.Properties.Resources._48);
            images.Add(global::libSplash.Properties.Resources._49);
            images.Add(global::libSplash.Properties.Resources._50);
            images.Add(global::libSplash.Properties.Resources._51);
            images.Add(global::libSplash.Properties.Resources._52);
            images.Add(global::libSplash.Properties.Resources._53);
            images.Add(global::libSplash.Properties.Resources._54);
            images.Add(global::libSplash.Properties.Resources._55);
            images.Add(global::libSplash.Properties.Resources._56);
            images.Add(global::libSplash.Properties.Resources._57);
            images.Add(global::libSplash.Properties.Resources._58);
            images.Add(global::libSplash.Properties.Resources._59);
            images.Add(global::libSplash.Properties.Resources._60);
            images.Add(global::libSplash.Properties.Resources._61);
            images.Add(global::libSplash.Properties.Resources._62);
            images.Add(global::libSplash.Properties.Resources._63);
            images.Add(global::libSplash.Properties.Resources._64);
            images.Add(global::libSplash.Properties.Resources._65);
            images.Add(global::libSplash.Properties.Resources._66);
            images.Add(global::libSplash.Properties.Resources._67);
            images.Add(global::libSplash.Properties.Resources._68);
            images.Add(global::libSplash.Properties.Resources._69);
            images.Add(global::libSplash.Properties.Resources._70);
            images.Add(global::libSplash.Properties.Resources._71);
            images.Add(global::libSplash.Properties.Resources._72);
            images.Add(global::libSplash.Properties.Resources._73);
            images.Add(global::libSplash.Properties.Resources._74);
            images.Add(global::libSplash.Properties.Resources._75);
            images.Add(global::libSplash.Properties.Resources._76);
            images.Add(global::libSplash.Properties.Resources._77);
            images.Add(global::libSplash.Properties.Resources._78);
            images.Add(global::libSplash.Properties.Resources._79);
            images.Add(global::libSplash.Properties.Resources._80);
            images.Add(global::libSplash.Properties.Resources._81);
            images.Add(global::libSplash.Properties.Resources._82);
            images.Add(global::libSplash.Properties.Resources._83);
            images.Add(global::libSplash.Properties.Resources._84);
            images.Add(global::libSplash.Properties.Resources._85);
            images.Add(global::libSplash.Properties.Resources._86);
            images.Add(global::libSplash.Properties.Resources._87);
            images.Add(global::libSplash.Properties.Resources._88);
            images.Add(global::libSplash.Properties.Resources._89);
            images.Add(global::libSplash.Properties.Resources._90);
            images.Add(global::libSplash.Properties.Resources._91);
            images.Add(global::libSplash.Properties.Resources._92);
            images.Add(global::libSplash.Properties.Resources._93);
            images.Add(global::libSplash.Properties.Resources._94);
            images.Add(global::libSplash.Properties.Resources._95);
            images.Add(global::libSplash.Properties.Resources._96);
            images.Add(global::libSplash.Properties.Resources._97);
            images.Add(global::libSplash.Properties.Resources._98);
            images.Add(global::libSplash.Properties.Resources._99);
            images.Add(global::libSplash.Properties.Resources._100);
            images.Add(global::libSplash.Properties.Resources._101);
            images.Add(global::libSplash.Properties.Resources._102);
            images.Add(global::libSplash.Properties.Resources._103);
            images.Add(global::libSplash.Properties.Resources._104);
            images.Add(global::libSplash.Properties.Resources._105);
            images.Add(global::libSplash.Properties.Resources._106);
            images.Add(global::libSplash.Properties.Resources._107);
            images.Add(global::libSplash.Properties.Resources._108);
            images.Add(global::libSplash.Properties.Resources._109);
            images.Add(global::libSplash.Properties.Resources._110);
            images.Add(global::libSplash.Properties.Resources._111);
            images.Add(global::libSplash.Properties.Resources._112);
            images.Add(global::libSplash.Properties.Resources._113);
            images.Add(global::libSplash.Properties.Resources._114);
            images.Add(global::libSplash.Properties.Resources._115);
            images.Add(global::libSplash.Properties.Resources._116);
            images.Add(global::libSplash.Properties.Resources._117);
            images.Add(global::libSplash.Properties.Resources._118);
            images.Add(global::libSplash.Properties.Resources._119);
            images.Add(global::libSplash.Properties.Resources._120);
            images.Add(global::libSplash.Properties.Resources._121);
            images.Add(global::libSplash.Properties.Resources._122);
            images.Add(global::libSplash.Properties.Resources._123);
            images.Add(global::libSplash.Properties.Resources._124);
            images.Add(global::libSplash.Properties.Resources._125);
            images.Add(global::libSplash.Properties.Resources._126);
            images.Add(global::libSplash.Properties.Resources._127);
            images.Add(global::libSplash.Properties.Resources._128);
            images.Add(global::libSplash.Properties.Resources._129);
            images.Add(global::libSplash.Properties.Resources._130);
            images.Add(global::libSplash.Properties.Resources._131);
            images.Add(global::libSplash.Properties.Resources._132);
            images.Add(global::libSplash.Properties.Resources._133);
            images.Add(global::libSplash.Properties.Resources._134);
            images.Add(global::libSplash.Properties.Resources._135);
            images.Add(global::libSplash.Properties.Resources._136);
            images.Add(global::libSplash.Properties.Resources._137);
            images.Add(global::libSplash.Properties.Resources._138);
            images.Add(global::libSplash.Properties.Resources._139);
            images.Add(global::libSplash.Properties.Resources._140);
            images.Add(global::libSplash.Properties.Resources._141);
            images.Add(global::libSplash.Properties.Resources._142);
            images.Add(global::libSplash.Properties.Resources._143);
            images.Add(global::libSplash.Properties.Resources._144);
            images.Add(global::libSplash.Properties.Resources._145);
            images.Add(global::libSplash.Properties.Resources._146);
            images.Add(global::libSplash.Properties.Resources._147);
            images.Add(global::libSplash.Properties.Resources._148);
            images.Add(global::libSplash.Properties.Resources._149);
            images.Add(global::libSplash.Properties.Resources._150);
            images.Add(global::libSplash.Properties.Resources._151);
            images.Add(global::libSplash.Properties.Resources._152);
            images.Add(global::libSplash.Properties.Resources._153);
            images.Add(global::libSplash.Properties.Resources._154);
            images.Add(global::libSplash.Properties.Resources._155);
            images.Add(global::libSplash.Properties.Resources._156);
            images.Add(global::libSplash.Properties.Resources._157);
            images.Add(global::libSplash.Properties.Resources._158);
            images.Add(global::libSplash.Properties.Resources._159);
            images.Add(global::libSplash.Properties.Resources._160);
            images.Add(global::libSplash.Properties.Resources._161);
            images.Add(global::libSplash.Properties.Resources._162);
            images.Add(global::libSplash.Properties.Resources._163);
            images.Add(global::libSplash.Properties.Resources._164);
            images.Add(global::libSplash.Properties.Resources._165);
            images.Add(global::libSplash.Properties.Resources._166);
            images.Add(global::libSplash.Properties.Resources._167);
            images.Add(global::libSplash.Properties.Resources._168);
            images.Add(global::libSplash.Properties.Resources._169);
            images.Add(global::libSplash.Properties.Resources._170);
            images.Add(global::libSplash.Properties.Resources._171);
            images.Add(global::libSplash.Properties.Resources._172);
            images.Add(global::libSplash.Properties.Resources._173);
            images.Add(global::libSplash.Properties.Resources._174);
            images.Add(global::libSplash.Properties.Resources._175);
            images.Add(global::libSplash.Properties.Resources._176);
            images.Add(global::libSplash.Properties.Resources._177);
            images.Add(global::libSplash.Properties.Resources._178);
            images.Add(global::libSplash.Properties.Resources._179);
            images.Add(global::libSplash.Properties.Resources._180);
            images.Add(global::libSplash.Properties.Resources._181);
            images.Add(global::libSplash.Properties.Resources._182);
            images.Add(global::libSplash.Properties.Resources._183);
            images.Add(global::libSplash.Properties.Resources._184);
            images.Add(global::libSplash.Properties.Resources._185);
            images.Add(global::libSplash.Properties.Resources._186);
            images.Add(global::libSplash.Properties.Resources._187);
            images.Add(global::libSplash.Properties.Resources._188);
            images.Add(global::libSplash.Properties.Resources._189);
            images.Add(global::libSplash.Properties.Resources._190);
            images.Add(global::libSplash.Properties.Resources._191);
            images.Add(global::libSplash.Properties.Resources._192);
            images.Add(global::libSplash.Properties.Resources._193);
            images.Add(global::libSplash.Properties.Resources._194);
            images.Add(global::libSplash.Properties.Resources._195);
            images.Add(global::libSplash.Properties.Resources._196);
            images.Add(global::libSplash.Properties.Resources._197);
            images.Add(global::libSplash.Properties.Resources._198);
            images.Add(global::libSplash.Properties.Resources._199);
            images.Add(global::libSplash.Properties.Resources._200);
            images.Add(global::libSplash.Properties.Resources._201);
            images.Add(global::libSplash.Properties.Resources._202);
            images.Add(global::libSplash.Properties.Resources._203);
            images.Add(global::libSplash.Properties.Resources._204);
            images.Add(global::libSplash.Properties.Resources._205);
            images.Add(global::libSplash.Properties.Resources._206);
            images.Add(global::libSplash.Properties.Resources._207);
            images.Add(global::libSplash.Properties.Resources._208);
            images.Add(global::libSplash.Properties.Resources._209);
            images.Add(global::libSplash.Properties.Resources._210);
            images.Add(global::libSplash.Properties.Resources._211);
            images.Add(global::libSplash.Properties.Resources._212);
            images.Add(global::libSplash.Properties.Resources._213);
            images.Add(global::libSplash.Properties.Resources._214);
            images.Add(global::libSplash.Properties.Resources._215);
            images.Add(global::libSplash.Properties.Resources._216);
            images.Add(global::libSplash.Properties.Resources._217);
            images.Add(global::libSplash.Properties.Resources._218);
            images.Add(global::libSplash.Properties.Resources._219);
            images.Add(global::libSplash.Properties.Resources._220);
            images.Add(global::libSplash.Properties.Resources._221);
            images.Add(global::libSplash.Properties.Resources._222);
            images.Add(global::libSplash.Properties.Resources._223);
            images.Add(global::libSplash.Properties.Resources._224);
            images.Add(global::libSplash.Properties.Resources._225);
            images.Add(global::libSplash.Properties.Resources._226);
            images.Add(global::libSplash.Properties.Resources._227);
            images.Add(global::libSplash.Properties.Resources._228);
            images.Add(global::libSplash.Properties.Resources._229);
            images.Add(global::libSplash.Properties.Resources._230);
            images.Add(global::libSplash.Properties.Resources._231);
            images.Add(global::libSplash.Properties.Resources._232);
            images.Add(global::libSplash.Properties.Resources._233);
            images.Add(global::libSplash.Properties.Resources._234);
            images.Add(global::libSplash.Properties.Resources._235);
            images.Add(global::libSplash.Properties.Resources._236);
            images.Add(global::libSplash.Properties.Resources._237);
            images.Add(global::libSplash.Properties.Resources._238);
            images.Add(global::libSplash.Properties.Resources._239);
            images.Add(global::libSplash.Properties.Resources._240);
            images.Add(global::libSplash.Properties.Resources._241);
            images.Add(global::libSplash.Properties.Resources._242);
            images.Add(global::libSplash.Properties.Resources._243);
            images.Add(global::libSplash.Properties.Resources._244);
            images.Add(global::libSplash.Properties.Resources._245);
            images.Add(global::libSplash.Properties.Resources._246);
            images.Add(global::libSplash.Properties.Resources._247);
            images.Add(global::libSplash.Properties.Resources._248);
            images.Add(global::libSplash.Properties.Resources._249);
            images.Add(global::libSplash.Properties.Resources._250);
            images.Add(global::libSplash.Properties.Resources._251);
            images.Add(global::libSplash.Properties.Resources._252);
            images.Add(global::libSplash.Properties.Resources._253);
            images.Add(global::libSplash.Properties.Resources._254);
            images.Add(global::libSplash.Properties.Resources._255);
            images.Add(global::libSplash.Properties.Resources._256);
            images.Add(global::libSplash.Properties.Resources._257);
            images.Add(global::libSplash.Properties.Resources._258);
            images.Add(global::libSplash.Properties.Resources._259);
            images.Add(global::libSplash.Properties.Resources._260);
            images.Add(global::libSplash.Properties.Resources._261);
            images.Add(global::libSplash.Properties.Resources._262);
            images.Add(global::libSplash.Properties.Resources._263);
            images.Add(global::libSplash.Properties.Resources._264);
            images.Add(global::libSplash.Properties.Resources._265);
            images.Add(global::libSplash.Properties.Resources._266);
            images.Add(global::libSplash.Properties.Resources._267);
            images.Add(global::libSplash.Properties.Resources._268);
            images.Add(global::libSplash.Properties.Resources._269);
            images.Add(global::libSplash.Properties.Resources._270);
            images.Add(global::libSplash.Properties.Resources._271);
            images.Add(global::libSplash.Properties.Resources._272);
            images.Add(global::libSplash.Properties.Resources._273);
            images.Add(global::libSplash.Properties.Resources._274);
            images.Add(global::libSplash.Properties.Resources._275);
            images.Add(global::libSplash.Properties.Resources._276);
            images.Add(global::libSplash.Properties.Resources._277);
            images.Add(global::libSplash.Properties.Resources._278);
            images.Add(global::libSplash.Properties.Resources._279);
            images.Add(global::libSplash.Properties.Resources._280);
            images.Add(global::libSplash.Properties.Resources._281);
            images.Add(global::libSplash.Properties.Resources._282);
            images.Add(global::libSplash.Properties.Resources._283);
            images.Add(global::libSplash.Properties.Resources._284);
            images.Add(global::libSplash.Properties.Resources._285);
            images.Add(global::libSplash.Properties.Resources._286);
            images.Add(global::libSplash.Properties.Resources._287);
            images.Add(global::libSplash.Properties.Resources._288);
            images.Add(global::libSplash.Properties.Resources._289);
            images.Add(global::libSplash.Properties.Resources._290);
            images.Add(global::libSplash.Properties.Resources._291);
            images.Add(global::libSplash.Properties.Resources._292);
            images.Add(global::libSplash.Properties.Resources._293);
            images.Add(global::libSplash.Properties.Resources._294);
            images.Add(global::libSplash.Properties.Resources._295);
            images.Add(global::libSplash.Properties.Resources._296);
            images.Add(global::libSplash.Properties.Resources._297);
            images.Add(global::libSplash.Properties.Resources._298);
            images.Add(global::libSplash.Properties.Resources._299);
            images.Add(global::libSplash.Properties.Resources._300);
            images.Add(global::libSplash.Properties.Resources._301);
            images.Add(global::libSplash.Properties.Resources._302);
            images.Add(global::libSplash.Properties.Resources._303);
            images.Add(global::libSplash.Properties.Resources._304);
            images.Add(global::libSplash.Properties.Resources._305);
            images.Add(global::libSplash.Properties.Resources._306);
            images.Add(global::libSplash.Properties.Resources._307);
            images.Add(global::libSplash.Properties.Resources._308);
            images.Add(global::libSplash.Properties.Resources._309);
            images.Add(global::libSplash.Properties.Resources._310);
            images.Add(global::libSplash.Properties.Resources._311);
            images.Add(global::libSplash.Properties.Resources._312);
            images.Add(global::libSplash.Properties.Resources._313);
            images.Add(global::libSplash.Properties.Resources._314);
            images.Add(global::libSplash.Properties.Resources._315);
            images.Add(global::libSplash.Properties.Resources._316);
            images.Add(global::libSplash.Properties.Resources._317);

            return images;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Color pixelColor = Color.White;

            if (m_animator == null || m_optGIF)
            {
                int nDrawingCount = -1;

                if (m_useSingleLoop)
                {
                    if (m_dimension != null && m_nFrameCount > 0)
                    {
                        nDrawingCount = m_nDrawingCount;

                        if (++m_nDrawingCount == m_nFrameCount)
                        {
                            this.Image.SelectActiveFrame(m_dimension, m_nFrameCount - 1);
                            this.Enabled = false;
                            nDrawingCount = m_nFrameCount - 1;
                        }
                    }
                }
                else
                {
                    if (++m_nDrawingCount == m_nFrameCount)
                    {
                        this.Image.SelectActiveFrame(m_dimension, m_nFrameCount - 1);
                        m_nDrawingCount = 0;

                        if (m_owner != null)
                            nDrawingCount = m_nFrameCount - 1;
                    }
                    else
                    {
                        this.Image.SelectActiveFrame(m_dimension, m_nDrawingCount - 1);

                        if (m_owner != null)
                            nDrawingCount = m_nDrawingCount - 1;
                    }
                }

                if (m_onlyLastImage && m_nFrameCount > 1 && m_dimension != null)
                {
                    this.Image.SelectActiveFrame(m_dimension, m_nFrameCount - 1);
                    this.Enabled = false;
                }

                base.OnPaint(pe);

                pixelColor = m_textColorOption == TextColorOption.REF_PIXEL ? GetPixelColor(m_ptTextColorRef.X, m_ptTextColorRef.Y) : m_fixTextColor;
            }
            else
            {
                if (m_textColorOption == TextColorOption.REF_PIXEL)
                    m_animator.Draw(pe.Graphics, m_ptTextColorRef.X, m_ptTextColorRef.Y, ref pixelColor);
                else
                {
                    m_animator.Draw(pe.Graphics);
                    pixelColor = m_fixTextColor;
                }
            }

            if (m_owner != null)
            {
                //Color color = m_textColorOption == TextColorOption.REF_PIXEL ? GetPixelColor(m_ptTextColorRef.X, m_ptTextColorRef.Y) : m_fixTextColor;
                //m_owner.OnPostPaint(pe.Graphics, nDrawingCount, m_nFrameCount);
                m_owner.OnPostPaint(pe.Graphics, pixelColor);
            }
        }

        private Color GetPixelColor(int x, int y)
        {
            return ((Bitmap)this.Image).GetPixel(x, y);
        }

        protected void CalcFrame()
        {
            if (m_dimension != null)
                return;

            if (this.Image != null)
            {
                System.Guid[] guids = this.Image.FrameDimensionsList;
                m_dimension = new System.Drawing.Imaging.FrameDimension(guids[0]);

                m_nFrameCount = this.Image.GetFrameCount(m_dimension);
            }
        }

        public void SetRefTextColor(int x, int y)
        {
            m_textColorOption = TextColorOption.REF_PIXEL;
            m_ptTextColorRef.X = x;
            m_ptTextColorRef.Y = y;
        }

        public void SetFixTextColor(Color color)
        {
            m_textColorOption = TextColorOption.FIX_COLOR;
            m_fixTextColor = color;
        }
    }

    interface GifPictureBoxOwner
    {
        void OnPostPaint(Graphics g, Color color);
        //void OnPostPaint(Graphics g, int nDrawingCount, int nFrameCount);
    }
}
