using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Physics_Sim
{
    public partial class MainForm : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public MainForm()
        {
            InitializeComponent();
            AllocConsole();

            Thread inputThread = new Thread(() => { 
                while (true)
                {
                    Console.Write("Enter an option: ");
                    switch (Console.ReadLine())
                    {
                        case "Draw":
                            Console.WriteLine("Redrawing graph...");
                            redrawGraph();
                            break;
                        case "XScale":
                            Console.Write("Enter new X-Scale: ");
                            xScale = Convert.ToInt32(Console.ReadLine());
                            redrawGraph();
                            break;
                        case "YScale":
                            Console.Write("Enter new Y-Scale: ");
                            yScale = Convert.ToInt32(Console.ReadLine());
                            redrawGraph();
                            break;
                        default:
                            Console.WriteLine("Unknown command.");
                            break;
                    }
                }                
            });
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        public void redrawGraph()
        {
            simDrawn = false;
            Action act = () => this.Refresh();
            this.Invoke(act);
        }

        /** 
         * <summary>
         * Sets Y-Scale of graph.
         * </summary>
         */
        int yScale = 1;
        /** 
         * <summary>
         * Sets X-Scale of graph.
         * </summary>
         */
        int xScale = 40;
        /** 
         * <summary>
         * Holds position of mouse on the x-axis.
         * </summary>
         */
        int mouseXPos = 0;
        /** 
         * <summary>
         * Holds position of mouse on the y-axis.
         * </summary>
         */
        int mouseYPos = 0;
        /** 
         * <summary>
         * Defines if simulation has been drawn or not. Prevents excess for loop execution.
         * </summary>
         */
        bool simDrawn = false;

        /**
         * <summary>
         * Draws graph on main form (Form1).
         * </summary>
         */
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            setupChart(e);

            double initialVelocity = 100; //100 m/s
            double initialHeight = 0; //0 m
            double time = 50; //50 Seconds
            Point[] pointArray = new Point[(int) time];

            if (!simDrawn)
            {
                Console.WriteLine("h = -4.9 * t^2 + " + initialVelocity + " * t + " + initialHeight);
                for (int i = 0; i < time; i++)
                {
                    double height = -4.9 * Math.Pow(i, 2) + initialVelocity * i + initialHeight;
                    pointArray[i] = new Point(i * xScale + 20, -(int)height * yScale + this.Size.Height - 50);
                    Console.WriteLine("{X=" + i + ",Y=" + height + "}");
                    height = -height * yScale;
                    e.Graphics.DrawRectangle(new Pen(Color.LimeGreen, 2), i * xScale + 20, (float) height + this.Size.Height - 50, 2, 2);
                }

                simDrawn = true;
            }

            Array.Reverse(pointArray);
            e.Graphics.DrawCurve(new Pen(Color.White, 1), pointArray);
        }

        /** 
         * <summary>
         * Draws x and y axis as well as grid lines every 10 pixels.
         * </summary>
         */
        public void setupChart(PaintEventArgs e)
        {
            for (int i = 0; i < this.Size.Height; i = i + 10)
            {
                e.Graphics.DrawLine(new Pen(Color.Gray, 1), 0, i, this.Size.Width, i);
            }
            for (int i = 0; i < this.Size.Width; i = i + 10)
            {
                e.Graphics.DrawLine(new Pen(Color.Gray, 1), i, 0, i, this.Size.Height);
            }
            e.Graphics.DrawLine(new Pen(Color.White, 3), 0, this.Size.Height - 50, this.Size.Width, this.Size.Height - 50);
            e.Graphics.DrawLine(new Pen(Color.White, 3), 20, 0, 20, this.Size.Height);
        }

        /** 
         * <summary>
         * Fires when mouse is moved over form 1. Sets mouseXPos and mouseYPos as well as updates text of label.
         * </summary>
         */
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mouseXPos = e.X;
            mouseYPos = e.Y;
            label1.Text = "Mouse Coordinates: " + ((double)(e.X - 20)/xScale) + ", " + (((this.Size.Height - 50 - e.Y)/yScale));
        }
    }
}
