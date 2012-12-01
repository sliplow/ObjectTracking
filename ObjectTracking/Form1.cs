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
			

			// apply the filter
			//Bitmap tmp1 = new Edges().Apply(resultImage);

			//// extract red channel from the original image
			//IFilter extrachChannel = new ExtractChannel(RGB.R);
			//Bitmap redChannel = extrachChannel.Apply(image);

			////  merge red channel with moving object borders
			//Merge mergeFilter = new Merge();
			//mergeFilter.OverlayImage = tmp1;
			//Bitmap tmp2 = mergeFilter.Apply(redChannel);
			
			//// replace red channel in the original image
			//ReplaceChannel replaceChannel = new ReplaceChannel(RGB.R);
			//replaceChannel.ChannelImage = tmp2;
			//Bitmap tmp3 = replaceChannel.Apply(image);

			return resultImage;
		}

		private List<Rectangle> GetLocationRectangles(Bitmap thresholdImage)
		{
			const int size = 9;

			BlobCounter blobCounter = new BlobCounter();

			// Get object rectangles
			blobCounter.ProcessImage(thresholdImage);
			Rectangle[] rects = blobCounter.GetObjectsRectangles();

			List<Rectangle> largeBlobRects = new List<Rectangle>();

			foreach (Rectangle rc in rects)
			{
				if ((rc.Width < size) && (rc.Height < size*2.5)) continue;

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

		private void Pause_Click(object sender, EventArgs e)
		{
			if (videoSourcePlayer.IsRunning)
			{
				// TODO: Change this to pause.
				videoSourcePlayer.Stop();

				Pause.Text = "Continue";
				return;
			}

			Pause.Text = "Start";
			videoSourcePlayer.Start();
		}
	}
}