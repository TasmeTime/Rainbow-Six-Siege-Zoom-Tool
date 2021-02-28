using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Karna.Magnification;
using Gma.System.MouseKeyHook;
using System.Threading;

namespace Mag
{

    public partial class Form1 : Form
    {


        #region ExtraStuff
        enum MagType
        {
            AGOC,
            RedDot
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
     (
         int nLeftRect, // x-coordinate of upper-left corner
         int nTopRect, // y-coordinate of upper-left corner
         int nRightRect, // x-coordinate of lower-right corner
         int nBottomRect, // y-coordinate of lower-right corner
         int nWidthEllipse, // height of ellipse
         int nHeightEllipse // width of ellipse
     );
        private IKeyboardMouseEvents m_GlobalHook;
        Magnifier m;
        [DllImport("Magnification.dll")]
        static extern bool MagInitialize();
        [DllImport("Magnification.dll")]
        static extern bool MagUninitialize();
        public Form1()
        {
            InitializeComponent();
        }

        public enum GWL
        {
            ExStyle = -20
        }

        public enum WS_EX
        {
            Transparent = 0x20,
            Layered = 0x80000
        }

        public enum LWA
        {
            ColorKey = 0x1,
            Alpha = 0x2
        }


        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);

        #endregion

        bool isEnable = true;
        MagType currentType = MagType.RedDot;
        protected override void OnShown(EventArgs e)
        {
            //Setting Form On Transparent Mode So You Can Click Through It
            base.OnShown(e);
            int wl = GetWindowLong(this.Handle, GWL.ExStyle);
            wl = wl | 0x80000 | 0x20;
            SetWindowLong(this.Handle, GWL.ExStyle, wl);
            SetLayeredWindowAttributes(this.Handle, 0, 255, LWA.Alpha);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            MessageBox.Show("R6 Siege Zoom Tool v1.1 By TasmeTime ;)");
            //Setting The Hotkeys
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += M_GlobalHook_MouseDownExt;
            m_GlobalHook.MouseUpExt += M_GlobalHook_MouseUpExt;
            m_GlobalHook.KeyDown += M_GlobalHook_KeyDown;
            m_GlobalHook.KeyPress += M_GlobalHook_KeyPress;

            //Rounding Form
            this.FormBorderStyle = FormBorderStyle.None;
            this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,(Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 1000, 1000));

            //Starting The Magnifier
            if (MagInitialize())
            {
                m = new Magnifier(this);
                m.Magnification = 1f;
            }
        }

        private void M_GlobalHook_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Left)
            {
                this.Location = new Point(this.Location.X-1, this.Location.Y);
            }
            else if (e.KeyCode == Keys.Right)
            {
                this.Location = new Point(this.Location.X + 1, this.Location.Y);
            }
            else if (e.KeyCode == Keys.Up)
            {
                this.Location = new Point(this.Location.X, this.Location.Y-1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                this.Location = new Point(this.Location.X , this.Location.Y+1);
            }
            else if (e.KeyCode == Keys.Home)
            {
                this.Location = new Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2, (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                this.Size = new Size(this.Size.Width + 1, this.Size.Height + 1);
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 1000, 1000));
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                this.Size = new Size(this.Size.Width - 1, this.Size.Height - 1);
                Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 1000, 1000));
            }else if (e.KeyCode == Keys.End)
            {
                Application.Exit();
            }
        }

        private void M_GlobalHook_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar("+")|| e.KeyChar == Convert.ToChar("="))
            {
                m.Magnification += 0.1f;
            }
            else if (e.KeyChar == Convert.ToChar("-"))
            {
                m.Magnification -= 0.1f;
            }
            else if (e.KeyChar == Convert.ToChar("*"))
            {
                isEnable = !isEnable;
            }
        }

        private void M_GlobalHook_MouseUpExt(object sender, MouseEventExtArgs e)
        {
            if (e.Button == MouseButtons.Right && isEnable)
            {
                this.Opacity = 0;
            }
        }

        private void M_GlobalHook_MouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (e.Button == MouseButtons.Right && isEnable)
            {
                this.Opacity = 1;
            }
        }
    }
}
