#pragma once
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Collections;
using namespace System::ComponentModel;
using namespace System::Data;
using namespace System::Drawing;
using namespace System::Text;
using namespace System::Threading::Tasks;
using namespace System::Windows::Forms;
using namespace System::Drawing::Drawing2D;

namespace DXFViewer
{
	ref struct RGB
	{
	public:
		double R;
		double G;
		double B;
	};
	ref struct HSB
	{
	public:
		double H;
		double S;
		double B;
	};

	public ref class ColorExtension
	{
	public:	
		static Color GetContrast(Color source, bool preserveOpacity)
		{
			Color inputColor = source;
			//if RGB values are close to each other by a diff less than 10%, then if RGB values are lighter side, decrease the blue by 50% (eventually it will increase in conversion below), if RBB values are on darker side, decrease yellow by about 50% (it will increase in conversion)
			System::Byte avgColorValue = (System::Byte)((source.R + source.G + source.B) / 3);
			int diff_r = Math::Abs(source.R - avgColorValue);
			int diff_g = Math::Abs(source.G - avgColorValue);
			int diff_b = Math::Abs(source.B - avgColorValue);
			if (diff_r < 20 && diff_g < 20 && diff_b < 20) //The color is a shade of gray
			{
				if (avgColorValue < 123) //color is dark
				{
					inputColor = Color::FromArgb(source.A, 220, 230, 50);
				}
				else
				{
					inputColor = Color::FromArgb(source.A, 255, 255, 50);
				}
			}

			System::Byte sourceAlphaValue = source.A;
			if (!preserveOpacity)
			{
				sourceAlphaValue = Math::Max(source.A, (System::Byte)127); //We don't want contrast color to be more than 50% transparent ever.
			}
			RGB^ rgb = gcnew RGB();
			rgb->R = inputColor.R;
			rgb->G = inputColor.G;
			rgb->B = inputColor.B;

			HSB^ hsb = ConvertToHSB(rgb);
			hsb->H = (hsb->H < 180.0f) ? hsb->H + 180 : hsb->H - 180;
			//hsb.B = isColorDark ? 240 : 50; //Added to create dark on light, and light on dark
			rgb = ConvertToRGB(hsb);
			return Color::FromArgb((int)sourceAlphaValue, (int)rgb->R, (int)rgb->G, (int)rgb->B);
		}

	private:
		static RGB^ ConvertToRGB(HSB^ hsb)
		{

			double chroma = hsb->S * hsb->B;
			int hue2 = hsb->H / 60;
			double bb = hue2 % 2;
			double x = chroma * (1 - Math::Abs(bb  - 1.0));
			double r1 = 0.0;
			double g1 = 0.0;
			double b1 = 0.0;
			if (hue2 >= 0 && hue2 < 1)
			{
				r1 = chroma;
				g1 = x;
			}
			else if (hue2 >= 1 && hue2 < 2)
			{
				r1 = x;
				g1 = chroma;
			}
			else if (hue2 >= 2 && hue2 < 3)
			{
				g1 = chroma;
				b1 = x;
			}
			else if (hue2 >= 3 && hue2 < 4)
			{
				g1 = x;
				b1 = chroma;
			}
			else if (hue2 >= 4 && hue2 < 5)
			{
				r1 = x;
				b1 = chroma;
			}
			else if (hue2 >= 5 && hue2 <= 6)
			{
				r1 = chroma;
				b1 = x;
			}
			double m = hsb->B - chroma;
			RGB^ rgb = gcnew RGB();

			rgb->R = r1 + m;
			rgb->G = g1 + m;
			rgb->B = b1 + m;
			return rgb;
		}

		static HSB^ ConvertToHSB(RGB^ rgb)
		{
			double r = rgb->R;
			double g = rgb->G;
			double b = rgb->B;

			double max = Max(r, g, b);
			double min = Min(r, g, b);
			double chroma = max - min;
			double hue2 = 0.0;
			if (chroma != 0)
			{
				if (max == r)
				{
					hue2 = (g - b) / chroma;
				}
				else if (max == g)
				{
					hue2 = (b - r) / chroma + 2;
				}
				else
				{
					hue2 = (r - g) / chroma + 4;
				}
			}
			double hue = hue2 * 60;
			if (hue < 0)
			{
				hue += 360;
			}
			double brightness = max;
			double saturation = 0;
			if (chroma != 0)
			{
				saturation = chroma / brightness;
			}
			HSB^ hsb = gcnew HSB();
			hsb->H = hue;
			hsb->S = saturation;
			hsb->B = brightness;
			return hsb;

		}

			
		static double Max(double d1, double d2, double d3)
		{
			if (d1 > d2)
			{
				return Math::Max(d1, d3);
			}
			return Math::Max(d2, d3);
		}
		static double Min(double d1, double d2, double d3)
		{
			if (d1 < d2)
			{
				return Math::Min(d1, d3);
			}
			return Math::Min(d2, d3);
		}			
	};
}

