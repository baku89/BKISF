/*
{
	"NAME": "Voro Noise",
	"DESCRIPTION": "Draws a noise texture based on blurred voronoi",
	"CREDIT": "inigo quilez <http: //iquilezles.org/www/articles/voronoise/voronoise.htm>",
	"ISFVSN": "2",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "size",
			"LABEL": "Size",
			"TYPE": "float",
			"UNIT": "length",
			"DEFAULT": 0.1
		},
		{
			"NAME": "offset",
			"LABEL": "Offset",
			"TYPE": "point2D",
			"DEFAULT": [
				0.5,
				0.5
			]
		},
		{
			"NAME": "rotation",
			"LABEL": "Rotation",
			"TYPE": "float",
			"UNIT": "angle"
		},
		{
			"NAME": "disperse",
			"LABEL": "Disperse",
			"TYPE": "point2D",
			"DEFAULT": [
				0,
				1
			]
		},
		{
			"NAME": "blur",
			"LABEL": "Blur",
			"TYPE": "float",
			"UNIT": "percent",
			"DEFAULT": 0.5,
			"CLAMP_MIN": true,
			"CLAMP_MAX": true
		},
		{
			"NAME": "grayscale",
			"LABEL": "Use Grayscale",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "seed",
			"LABEL": "Seed",
			"TYPE": "float"
		}
	]
}
*/

// Sources:
// <https://gist.github.com/patriciogonzalezvivo/670c22f3966e662d2f83#voronoise>
//<https://www.shadertoy.com/view/Xd23Dh>
//<http://iquilezles.org/www/articles/voronoise/voronoise.htm>

#define SQRT2 1.4142

vec3 hash3(vec2 p) {
  vec3 q = vec3(dot(p, vec2(127.1, 311.7)), dot(p, vec2(269.5, 183.3)), dot(p, vec2(419.2, 371.9)));
  return fract(sin(q) * 43758.5453);
}

float iqnoise(in vec2 x, vec2 off) {
  vec2 p = floor(x);
  vec2 f = fract(x);

  float k = 1.0 + 63.0 * pow(1.0 - blur, 4.0);

  float va = 0.0;
  float wt = 0.0;
  for (int j = -2; j <= 2; j++)
    for (int i = -2; i <= 2; i++) {
      vec2 g = vec2(float(i), float(j));
      vec3 o = hash3(p + g + seed) * vec3(off, 1.);
      vec2 r = g - f + o.xy;
      float d = dot(r, r);
      float ww = pow(1.0 - smoothstep(0.0, SQRT2, sqrt(d)), k);
      va += o.z * ww;
      wt += ww;
    }

  return va / wt;
}

// https://github.com/mattdesl/glsl-random/blob/master/index.glsl
float random(vec2 co) {
  float a = 12.9898;
  float b = 78.233;
  float c = 43758.5453;
  float dt = dot(co.xy, vec2(a, b));
  float sn = mod(dt, 3.14);
  return fract(sin(sn) * c);
}

vec2 random1To2(float sd) {
  return vec2(sd + 431.231244, sd * 2.345 + -31242.344);
}

vec2 toNDC(vec2 uv) {
  return uv * vec2(1.0, RENDERSIZE.y / RENDERSIZE.x);
}

vec2 rotate(vec2 v, float a) {
  float s = sin(a);
  float c = cos(a);
  mat2 m = mat2(c, -s, s, c);
  return m * v;
}

void main() {
  vec2 pos = toNDC(isf_FragNormCoord.xy - offset);
  pos = rotate(pos, rotation);
  pos /= size;

  vec2 dsp = disperse.xy;
  dsp = dsp * 2.0 - 1.0;

  float r = iqnoise(pos, dsp);

  vec3 color = vec3(r);

  if (!grayscale) {
    color.g = iqnoise(pos + random1To2(seed + 123.123), dsp);
    color.b = iqnoise(pos + random1To2(seed + 831.23), dsp);
  }

  float mask = IMG_THIS_PIXEL(inputImage).a;

  gl_FragColor = vec4(color, mask);
}
