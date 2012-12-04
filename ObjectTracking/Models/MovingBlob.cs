using System;
using System.Drawing;
using Enumerators;

namespace ObjectTracking.Models
{
	public class MovingBlob
	{
		public readonly static int detectionVariation = 15;

		public Point Location { get; set; }
		public Size Size { get; set; }

		public int Left { get {	return Location.X; } }
		public int Top { get { return Location.Y; } }		
		public int Height {	get { return Size.Height; }	}
		public int Width { get { return Size.Width; } }

		public Point Motion { get; set; }
		public Direction Direction { get; set; }

		public MovingBlob(Rectangle rect)
		{
			Location = rect.Location;
			Size = rect.Size;
		}

		public bool IsSameObject(Rectangle rectangle)
		{
			if((Math.Abs(rectangle.Left - Left) + 
				Math.Abs(rectangle.Top - Top)) > 200)
			{			
				return false;
			}

			Motion = new Point(rectangle.Left - Left, rectangle.Top - Top);
		
			DetectMotionDirection();
			return true;
		}

		private void DetectMotionDirection()
		{
			if (Math.Abs(Motion.X) > detectionVariation)
			{
				if (Motion.X > 0)
				{
					if (Math.Abs(Motion.Y) > detectionVariation)
					{
						if (Motion.Y > detectionVariation)
						{
							Direction = Direction.UpRight;

							return;
						}
						Direction = Direction.DownRight;

						return;
					}

					Direction = Direction.Right;
				}
				else
				{
					if (Math.Abs(Motion.Y) > detectionVariation)
					{
						if (Motion.Y > detectionVariation)
						{
							Direction = Direction.UpLeft;

							return;
						}
						Direction = Direction.DownLeft;

						return;
					}

					Direction = Direction.Left;
				}
			}
			else
			{
				if (Math.Abs(Motion.Y) > detectionVariation)
				{
					if (Motion.Y > detectionVariation)
					{
						Direction = Direction.Up;

						return;
					}
					Direction = Direction.Down;

					return;
				}

				Direction = Direction.Stationary;
			}

			return;
		}
	}
}