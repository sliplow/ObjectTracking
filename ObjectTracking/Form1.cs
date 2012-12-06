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
		
		/// <summary>
		/// Based off www.codeproject.com/Articles/10248/Motion-Detection-Algorithms
		/// </summary>
		/// <param name="source"></param>
		private void OpenVideoSource(IVideoSource source)
		{
			Timer = new System.Timers.Timer();
			PrevImage = null;
			
			// close previous video source
			CloseVideoSource();

			// start new video source
			videoSourcePlayer.VideoSource = new AsyncVideoSource(source);
			videoSourcePlayer.Start();

			// Wait until video has loaded
			while(!videoSourcePlayer.IsRunning){}
						
			Timer.Start();
			Pause.Visible = true;

			PrevImage = videoSourcePlayer.GetCurrentVideoFrame();

			// Duration between blob updates
			Timer.Interval = 1820;
			Timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
		}
		
		/// <summary>
		/// This runs everytime a new frame is shown in the current video
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="image"></param>
		private void videoSourcePlayer_NewFrame(object sender, ref Bitmap image)
		{
			if (image == null) return;

			OnTimedEvent((Bitmap)image.Clone());

			if (BlobRects == null || BlobRects.Count == 0) return;

			DrawBlobs(BlobRects, image);
		}
		
		/// <summary>
		/// Based off www.codeproject.com/Articles/10248/Motion-Detection-Algorithms
		/// </summary>
		/// <param name="blobRects"></param>
		/// <param name="image"></param>
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
			// Tell the video to update the position data

			UpdateBg = true;
		}

		private void OnTimedEvent(Bitmap image)
		{
			if(!UpdateBg) return;

			// If it is time to update blob position data
			if (PrevImage != null)
			{
				// Get new blob positions

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

			movingBlobs.OrderByDescending(x => x.Height * x.Width);

			// Clear Bindings

			left.DataBindings.Clear();
			top.DataBindings.Clear();
			width.DataBindings.Clear();
			height.DataBindings.Clear();

			motionX.DataBindings.Clear();
			motionY.DataBindings.Clear();
			direction.DataBindings.Clear();

			// Add new postion data to the repeater collection

			left.DataBindings.Add(new Binding("Text", movingBlobs, "Left"));
			top.DataBindings.Add(new Binding("Text", movingBlobs, "Top"));
			height.DataBindings.Add(new Binding("Text", movingBlobs, "Height"));
			width.DataBindings.Add(new Binding("Text", movingBlobs, "Width"));

			motionX.DataBindings.Add(new Binding("Text", movingBlobs, "MotionLeft"));
			motionY.DataBindings.Add(new Binding("Text", movingBlobs, "MotionTop"));
			direction.DataBindings.Add(new Binding("Text", movingBlobs, "Direction"));
			
			if (dataRepeater1.Enabled)
			{
				// databind the list as a datasource.

				dataRepeater1.Invoke((MethodInvoker)delegate { dataRepeater1.DataSource = movingBlobs; });
			}
			return list;
		}

		/// <summary>
		/// Based off www.codeproject.com/Articles/10248/Motion-Detection-Algorithms
		/// </summary>
		/// <param name="prevImage"></param>
		/// <param name="image"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Based off www.codeproject.com/Articles/10248/Motion-Detection-Algorithms
		/// </summary>
		/// <param name="thresholdImage"></param>
		/// <returns></returns>
		private List<Rectangle> GetLocationRectangles(Bitmap thresholdImage)
		{
			int size = thresholdImage.Size.Width / 20;

			BlobCounter blobCounter = new BlobCounter();

			blobCounter.ProcessImage(thresholdImage);
			Rectangle[] rects = blobCounter.GetObjectsRectangles();


			List<Rectangle> largeBlobRects = new List<Rectangle>();

			foreach (Rectangle rc in rects)
			{
				if ((rc.Width < size) && (rc.Height < size*2.5)) continue;

				largeBlobRects.Add(rc);
			}
			
			return largeBlobRects.OrderBy(x => x.Height * x.Width).ToList();
		}

		/// <summary>
		/// Based off www.codeproject.com/Articles/10248/Motion-Detection-Algorithms
		/// </summary>
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
				// Stop video
				videoSourcePlayer.Stop();

				Pause.Text = "Start";
				return;
			}

			Pause.Text = "Stop";

			// Restart Video
			videoSourcePlayer.Start();
		}
	}
}