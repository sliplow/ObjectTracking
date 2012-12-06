using System.Collections.Generic;
using System.Drawing;

namespace ObjectTracking.Models
{
	public class MovingBlobCollection : List<MovingBlob>
	{
		public MovingBlobCollection(List<Rectangle> newRects, List<Rectangle> prevRects)
		{
			foreach(Rectangle rect in newRects)
			{
				MovingBlob blob = new MovingBlob(rect);

				// Work out the blob's previous position.

				SetPreviousPosition(prevRects, blob);
			}
		}

		private void SetPreviousPosition(List<Rectangle> prevRects, MovingBlob blob)
		{
			foreach (Rectangle prevRect in prevRects)
			{
				if (blob.IsSameObject(prevRect))
				{
					blob.SetMotion(prevRect);
					
					// Remove from the previous array so that other objects dont think they are this blob.
					prevRects.Remove(prevRect);
					this.Add(blob);
					
					return;
				}
			}
		}
	}
}