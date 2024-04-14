void Palette0_float(in float t, out half3 Color)
{
	half3 a = half3(0.5, 0.5, 0.5);
	half3 b = half3(0.5, 0.5, 0.5);
	half3 c = half3(1, 1, 1);
	half3 d = half3(0, 0.333, 0.667);
	
	Color = a+ b*cos(6.28318 * (c*t+d));
}