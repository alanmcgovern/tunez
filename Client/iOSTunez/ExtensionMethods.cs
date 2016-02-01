using System;
using CoreGraphics;

namespace iOSTunez
{
	public static class ExtensionMethods
	{
		public static CGRect Expand (this CGRect rect, nfloat x, nfloat y, nfloat width, nfloat height)
		{
			return new CGRect {
				X = rect.X + x,
				Y = rect.Y + y,
				Width = rect.Width + width,
				Height = rect.Height + height,
			};
		}
	}
}

