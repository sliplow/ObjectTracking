using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using ObjectTracking.Models;

namespace ObjectTracking
{
	public partial class Form1 : Form
	{
		private List<Rectangle> BlobRects { get; set; }
		private Bitmap PrevImage { get; set; }
		private System.Timers.Timer Timer { get; set; }
		private bool UpdateBg { get; set; }
		 
		public Form1()
		{
			InitializeComponent();

			Timer = new System.Timers.Timer();
			Pause.Visible = false;
		}

		/// <summary>
		/// Based my upload function on this article.
		/// 
		/// http://voices.yahoo.com/how-create-browse-file-dialog-box-c-6141416.html
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Upload(object sender, EventArgs e)
		{
			// Call a browse files dialog box

			// Intialize variables

			// Open the dialog box

			OpenFileDialog openFileDialog1 = new OpenFileDialog();

			openFileDialog1.InitialDirectory = "C:/Users/Dominic Northey/Videos/Sky Diving";

			openFileDialog1.Filter = "Video Files (*.wmv)|*.wmv|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 1;
			openFileDialog1.Multiselect = false;
			openFileDialog1.RestoreDirectory = true;

			DialogResult Result = openFileDialog1.ShowDialog();

			// Load the image to the load image picture box

			if (Result == DialogResult.OK)
			{
				OpenVideoSource(new FileVideoSource(openFileDialog1.FileName));
			}
		}
		
		// Open video source
		private void OpenVideoSource(IVideoSource source)
		{
			Timer = new System.Timers.Timer();
			PrevImage = null;
			
			// close previous video source
			CloseVideoSource();

			// start new video source
			videoSourcePlayer.VideoSource = new AsyncVideoSource(source);
			videoSourcePlayer.Start();

			while(!videoSourcePlayer.IsRunning)
			{

			}
						
			Timer.Start();
			Pause.Visible = true;

			PrevImage = videoSourcePlayer.GetCurrentVideoFrame();

			Timer.Interval = 1820;
			Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
		}
		
		private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
		{
			if (image == null) return;

			OnTimedEvent((Bitmap)image.Clone());

			if (BlobRects == null || BlobRects.Count == 0) return;

			DrawBlobs(BlobRects, image);
		}
		
		private void DrawBlobs(List<Rectangle> blobRects, Bitmap image)
		{
			// Create graphics object from initial image
			Graphics g = Graphics.FromImage(image);

			// Draw each rectangle
			using (Pen pen = new Pen(Color.Red, 2))
			{
				foreach (Rectangle rc in blobRects)
				{
					g.DrawRectangle(pen, rc);
				}
			}

			g.Dispose();
		}

		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			UpdateBg = true;
		}

		private void OnTimedEvent(Bitmap image)
		{
			if(!UpdateBg) return;

				if (PrevImage != null)
				{
					BlobRects = UpdateBlobPostion(GetLocationRectangles(
						(ThresholdImage(PrevImage, image))));
				}
				
				PrevImage = image;

			UpdateBg = false;
		}

		private List<Rectangle> UpdateBlobPostion(List<Rectangle> list)
		{
			if(BlobRects == null) return new List<Rectangle>();

			MovingBlobCollection movingBlobs = new MovingBlobCollection(list, BlobRects.ToList());

			left.DataBindings.Clear();
			top.DataBindings.Clear();
			width.DataBindings.Clear();
			height.DataBindings.Clear();

			motionX.DataBindings.Clear();
			motionY.DataBindings.Clear();
			direction.DataBindings.Clear();

			
			left.DataBindings.Add(new Binding("Text", movingBlobs, "Left"));
			top.DataBindings.Add(new Binding("Text", movingBlobs, "Top"));
			height.DataBindings.Add(new Binding("Text", movingBlobs, "Height"));
			width.DataBindings.Add(new Binding("Text", movingBlobs, "Width"));

			motionX.DataBindings.Add(new Binding("Text", movingBlobs, "MotionLeft"));
			motionY.DataBindings.Add(new Binding("Text", movingBlobs, "MotionTop"));
			direction.DataBindings.Add(new Binding("Text", movingBlobs, "Direction"));
			
			if (dataRepeater1.Enabled)
			{
				dataRepeater1.Invoke((MethodInvoker)delegate { dataRepeater1.DataSource = movingBlobs; });
			}
			return list;
		}

		private Bitmap ThresholdImage(Bitmap prevImage, Bitmap image)
		{
			// create filter
			new MoveTowards(image).ApplyInPlace(prevImage);

			FiltersSequence processingFilter = new FiltersSequence();
			processingFilter.Add(new Difference(prevImage));
			processingFilter.Add(new Pixellate());
			processingFilter.Add(new Grayscale(0.2125, 0.7154, 0.0721));
			processingFilter.Add(new Threshold(45));
			
			// apply the filter
			
			return processingFilter.Apply(image);
		}

		private List<Rectangle> GetLocationRectangles(Bitmap thresholdImage)
		{
			int size = thresholdImage.Size.Width / 20;

			BlobCounter blobCounter = new BlobCounter();

			// Get object rectangles
			blobCounter.ProcessImage(thresholdImage);
			Rectangle[] rects = blobCounter.GetObjectsRectangles();


			List<Rectangle> largeBlobRects = new List<Rectangle>();

			//if(rects.Count() == 0) return largeBlobRects;

			foreach (Rectangle rc in rects)
			{
				if ((rc.Width < size) && (rc.Height < size*2.5)) continue;

				largeBlobRects.Add(rc);
			}
			
			// TODO check this works.
			return largeBlobRects.OrderBy(x => x.Height * x.Width).ToList();
		}

		// Close current video source
		private void CloseVideoSource()
		{
			// set busy cursor
			this.Cursor = Cursors.WaitCursor;

			// stop current video source
			videoSourcePlayer.SignalToStop();

			// wait 2 seconds until camera stops
			for (int i = 0; (i < 50) && (videoSourcePlayer.IsRunning); i++)
			{
				Thread.Sleep(100);
			}
			if (videoSourcePlayer.IsRunning)
			{
				videoSourcePlayer.Stop();
			}

			// stop timers
			Timer.Stop();
		}

		private void Pause_Click(object sender, EventArgs e)
		{
			if (videoSourcePlayer.IsRunning)
			{
				// TODO: Change this to pause.
				videoSourcePlayer.Stop();

				Pause.Text = "Start";
				return;
			}

			Pause.Text = "Stop";
			videoSourcePlayer.Start();
		}
	}
}