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

		public int MotionLeft { get { return Motion.X; } }
		public int MotionTop { get { return Motion.Y; } }

		public MovingBlob(Rectangle rect)
		{
			Location = rect.Location;
			Size = rect.Size;
		}

		public bool IsSameObject(Rectangle rectangle)
		{
			// If total motion is more than 150 pixels then we assume its different object.

			if((Math.Abs(Left - rectangle.Left) + 
				Math.Abs(Top - rectangle.Top)) > 150)
			{			
				return false;
			}
					
			return true;
		}

		public void SetMotion(Rectangle rectangle)
		{
			// Work out object motion
			Motion = new Point(Left - rectangle.Left, Top - rectangle.Top);

			// Detection the direction of the motion.
			DetectMotionDirection();
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