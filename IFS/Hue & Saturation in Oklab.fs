
/*{
	"DESCRIPTION": "",
	"CREDIT": "",
	"ISFVSN": "2",
	"CATEGORIES": [
		"XXX"
	],
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "hueOffset",
			"LABEL": "Hue",
			"TYPE": "float",
			"UNIT": "angle"
		},
		{
			"NAME": "chromaScale",
			"LABEL": "Chroma Scale",
			"TYPE": "float",
			"CLAMP_MIN": true,
			"UNIT": "percent",
			"DEFAULT": 1
		},
		{
			"NAME": "chromaOffset",
			"LABEL": "Chroma Offset",
			"TYPE": "float",
			"UNIT": "percent",
			"DEFAULT": 0
		},
		{
			"NAME": "lightnessScale",
			"LABEL": "Lightness Scale",
			"TYPE": "float",
			"CLAMP_MIN": true,
			"UNIT": "percent",
			"DEFAULT": 1
		},
		{
			"NAME": "lightnessOffset",
			"LABEL": "Lightness Offset",
			"TYPE": "float",
			"UNIT": "percent",
			"DEFAULT": 0
		},
		{
			"NAME": "useSRGB",
			"LABEl": "Use sRGB",
			"TYPE": "bool",
			"DEFAULT": true
		}
	]

}*/

// The idea of using Oklab for more natural hue shifting comes from what Tettou-san <https://www.tettou771.com/> was talking in this podcast:
// https://twitter.com/img_club/status/1555706971604787200

// https://www.shadertoy.com/view/3lGyDG
float linear1(float c) {
  return (c <= 0.04045) ? c / 12.92 : pow((c + 0.055) / 1.055, 2.4);
}
vec3 srgb2linear(vec3 c) {
  return vec3(linear1(c.r), linear1(c.g), linear1(c.b));
}
float srgb1(float c) {
  return (c < 0.0031308 ? c * 12.92 : 1.055 * pow(c, 0.41666) - 0.055);
}
vec3 linear2srgb(vec3 c) {
  return vec3(srgb1(c.r), srgb1(c.g), srgb1(c.b));
}

vec3 rgb2oklab(vec3 rgb) {
  vec3 lms = mat3(					    //
		 0.4121656120, 0.2118591070, 0.0883097947,  //
		 0.5362752080, 0.6807189584, 0.2818474174,  //
		 0.0514575653, 0.1074065790, 0.6302613616   //
		 ) *
	     rgb;

  lms = pow(lms, vec3(1. / 3.));

  vec3 oklab = mat3(						 //
		   +0.2104542553, +1.9779984951, +0.0259040371,	 //
		   +0.7936177850, -2.4285922050, +0.7827717662,	 //
		   -0.0040720468, +0.4505937099, -0.8086757660	 //
		   ) *
	       lms;

  return oklab;
}

vec3 oklab2rgb(vec3 lab) {
  vec3 lms = mat3(					       //
		 +1.0000000000, +1.0000000000, +1.0000000000,  //
		 +0.3963377774, -0.1055613458, -0.0894841775,  //
		 +0.2158037573, -0.0638541728, -1.2914855480   //
		 ) *
	     lab;

  lms = pow(lms, vec3(3.0));

  vec3 rgb = mat3(					       //
		 +4.0767245293, -1.2681437731, -0.0041119885,  //
		 -3.3072168827, +2.6093323231, -0.7034763098,  //
		 +0.2307590544, -0.3411344290, +1.7068625689   //
		 ) *
	     lms;

  return rgb;
}

float hypot(float x, float y) {
  return sqrt(x * x + y * y);
}

void main() {
  vec4 color = IMG_THIS_PIXEL(inputImage);

  vec3 rgb = color.rgb;

  if (useSRGB) {
    rgb = srgb2linear(rgb);
  }

  vec3 lab = rgb2oklab(rgb);

  float lightness = lab.x;
  float chroma = hypot(lab.y, lab.z);
  float hue = atan(lab.z, lab.y);

  lightness = max(0.0, lightness * lightnessScale + lightnessOffset);
  chroma = max(0.0, chroma * chromaScale + chromaOffset);
  hue += hueOffset;

  lab = vec3(lightness, chroma * cos(hue), chroma * sin(hue));

  rgb = oklab2rgb(lab);

  if (useSRGB) {
    rgb = linear2srgb(rgb);
  }

  gl_FragColor = vec4(rgb, color.a);
}
