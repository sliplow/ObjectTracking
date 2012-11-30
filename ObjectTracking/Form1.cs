using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ObjectTracking
{
	public partial class Form1 : Form
	{
		private List<Rectangle> BlobRects { get; set; }
		private Bitmap PrevImage { get; set; }
		private System.Timers.Timer Timer { get; set; }
		 
		public Form1()
		{
			InitializeComponent();

			Timer = new System.Timers.Timer();
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

			PrevImage = videoSourcePlayer.GetCurrentVideoFrame();

			Timer.Interval = .5;
			Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
		}
		
		private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
		{
			if (image == null) return;

			//OnTimedEvent((Bitmap)image.Clone());

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
			Bitmap image = videoSourcePlayer.GetCurrentVideoFrame();
				
			if (image == null) return;

			OnTimedEvent(image);
		}

		private void OnTimedEvent(Bitmap image)
		{
			lock ( this )
            {
				if (PrevImage != null)
				{
					BlobRects = GetLocationRectangles(ThresholdImage(PrevImage, image));
				}

				PrevImage = image;
			}
		}

		private Bitmap ThresholdImage(Bitmap prevImage, Bitmap image)
		{
			//const int pixelatedFactor = 2;

			//new Pixellate(pixelatedFactor).ApplyInPlace(prevImage);
			//new Pixellate(pixelatedFactor).ApplyInPlace(image);

			// Create filter
			Subtract filter = new Subtract(new Grayscale(0.2125, 0.7154, 0.0721).Apply(prevImage));

			// Apply the filter
			Bitmap resultImage = filter.Apply(new Grayscale(0.2125, 0.7154, 0.0721).Apply(image));

			new Threshold(40).ApplyInPlace(resultImage);
			return resultImage;
		}

		private List<Rectangle> GetLocationRectangles(Bitmap thresholdImage)
		{
			BlobCounter blobCounter = new BlobCounter();

			// Get object rectangles
			blobCounter.ProcessImage(thresholdImage);
			Rectangle[] rects = blobCounter.GetObjectsRectangles();

			List<Rectangle> largeBlobRects = new List<Rectangle>();

			foreach (Rectangle rc in rects)
			{
				if ((rc.Width < 4) && (rc.Height < 4)) continue;

				largeBlobRects.Add(rc);
			}

			return largeBlobRects;
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
	}
}