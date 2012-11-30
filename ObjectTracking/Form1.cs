using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.VFW;

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

			openFileDialog1.Filter = "Video Files (*.mp4, *.wma)|*.mp4; *.wma|All files (*.*)|*.*";
			openFileDialog1.FilterIndex = 1;
			openFileDialog1.Multiselect = false;
			openFileDialog1.RestoreDirectory = true;

			DialogResult Result = openFileDialog1.ShowDialog();

			// Load the image to the load image picture box

			if (Result == DialogResult.OK)
			{
				 OpenVideoSource(new AVIFileVideoSource(openFileDialog1.FileName));
			}
		}

		private void View(object sender, EventArgs e)
		{
			//if (Input == null) 
			//{
			//    MessageBox.Show("Please Upload a video.");
			//    return;
			//}

			//ShowAllMotion();

			//videoSourcePlayer.VideoSource = Input.;

			//Input.Close();
		}



		private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
		{
			//if (Timer. % motionRefeshCounter == 0)
			{
				videoSourcePlayer.GetCurrentVideoFrame();
				BlobRects = GetLocationRectangles(ThresholdImage(PrevImage, image));
				PrevImage = image;
			}

			if (BlobRects.Count == 0) continue;

			DrawBlobs(BlobRects, image);
		}

		private Bitmap ThresholdImage(Bitmap prevImage, Bitmap image)
		{
			// Create filter
			Subtract filter = new Subtract(prevImage);

			// Apply the filter
			Bitmap resultImage = filter.Apply(image);

			new Threshold().ApplyInPlace(resultImage);
			return resultImage;
		}

		private List<Rectangle> GetLocationRectangles(Bitmap thresholdImage)
		{
			BlobCounter blobCounter = new BlobCounter();

			// Get object rectangles
			blobCounter.ProcessImage(thresholdImage);
			Rectangle[] rects = blobCounter.GetObjectsRectangles();
						
			List<Rectangle> largeBlobRects = new List<Rectangle>();

			foreach ( Rectangle rc in rects )
			{
				if ( ( rc.Width < 35 ) && ( rc.Height < 35 ) ) continue;
					
				largeBlobRects.Add(rc);					
			}

			return largeBlobRects;
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
		
		// Open video source
		private void OpenVideoSource(IVideoSource source)
		{
			// set busy cursor
			this.Cursor = Cursors.WaitCursor;

			// close previous video source
			CloseVideoSource();

			// start new video source
			videoSourcePlayer.VideoSource = new AsyncVideoSource(source);
			videoSourcePlayer.Start();
			
			Timer.Start();

			Timer.Interval = 0.30;
			//Timer.Elapsed();

			this.Cursor = Cursors.Default;
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