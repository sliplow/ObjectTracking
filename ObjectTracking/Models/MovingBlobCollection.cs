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
								
				foreach(Rectangle prevRect in prevRects)
				{
					if (blob.IsSameObject(prevRect))
					{
						prevRects.Remove(prevRect);
						this.Add(blob);

						return;
					}
				}
			}
		}
	}
}