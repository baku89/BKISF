
/*{
  "DESCRIPTION": "Droste effect",
  "CREDIT": "SCORPION <https://www.shadertoy.com/view/4tlGRn>",
  "ISFVSN": "2",
  "CATEGORIES": [
    "Distort"
  ],
  "INPUTS": [
    {
      "NAME": "inputImage",
      "TYPE": "image"
    },
    {
      "NAME": "zoom",
      "LABEL": "Zoom",
      "TYPE": "float",
      "MIN": 0,
      "MAX": 10,
      "UNIT": "percent"
    },
    {
      "NAME": "drosteScale",
      "LABEL": "Droste Scale",
      "TYPE": "float",
      "DEFAULT": 0.3,
      "MIN": 0,
      "MAX": 1,
      "CLAMP_MIN": true,
      "CLAMP_MAX": true,
      "UNIT": "percent"
    },
    {
      "NAME": "scale",
      "LABEL": "Scale",
      "TYPE": "float",
      "UNIT": "percent",
      "DEFAULT": 1
    },
    {
      "NAME": "rotation",
      "LABEL": "Rotation",
      "TYPE": "float",
      "DEFAULT": 0,
      "UNIT": "angle"
    },
    {
      "NAME": "branches",
      "LABEL": "Branches",
      "TYPE": "float",
      "DEFAULT": 1,
      "MIN": 0,
      "MAX": 10
    }
  ]
}*/

// This implementation uses GLSL code by ArKano22:
// http://www.gamedev.net/topic/590070-glsl-droste/

const float TAU = 6.283185307179586;

// Complex Math:
vec2 complexExp(vec2 z) {
  return vec2(exp(z.x) * cos(z.y), exp(z.x) * sin(z.y));
}

vec2 complexLog(vec2 z) {
  return vec2(log(length(z)), atan(z.y, z.x));
}

vec2 complexMult(vec2 a, vec2 b) {
  return vec2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
}

float complexMag(vec2 z) {
  return float(pow(length(z), 2.0));
}

vec2 complexReciprocal(vec2 z) {
  return vec2(z.x / complexMag(z), -z.y / complexMag(z));
}

vec2 complexDiv(vec2 a, vec2 b) {
  return complexMult(a,complexReciprocal(b));
}

vec2 complexPower(vec2 a, vec2 b) {
  return complexExp(complexMult(b, complexLog(a)));
}

// Misc Functions:
float nearestPower(float a, float base) {
  return pow(base, ceil(log(abs(a)) / log(base)) - 1.0);
}

float map(float value, float istart, float istop, float ostart, float ostop) {
  return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
}

mat2 rotate2d(in float radians) {
    float c = cos(radians);
    float s = sin(radians);
    return mat2(c, -s, s, c);
}

vec4 getDrosteColor(float bg) {
  // Shift and scale coordinates to [-0.5,0.5]
  vec2 uv = isf_FragNormCoord.xy - 0.5;
  uv.y *= RENDERSIZE.y / RENDERSIZE.x;

  uv *= rotate2d(-rotation);
  uv /= scale;

  // Escher grid transform
  float factor = pow(1.0 / drosteScale, branches);
  uv = complexPower(
    uv,
    complexDiv(vec2(log(factor), TAU), vec2(0.0, TAU)));

  // recutangular droste effect
  float ft = fract(zoom);
  // TODO: figure out how to smooth zoom speed
  ft = log(3. * ft + 1.) / log(4.);
  uv *= 1.0 + ft * (drosteScale - 1.0);

  float npower = max(nearestPower(uv.x * (1.0 + bg), drosteScale),
                     nearestPower(uv.y * (1.0 + bg), drosteScale));

  uv.x = map(uv.x, -npower, npower, -1.0, 1.0);
  uv.y = map(uv.y, -npower, npower, -1.0, 1.0);

  return IMG_NORM_PIXEL(inputImage, uv / 2. + vec2(.5));
}

float blendOverAlpha(vec4 A, vec4 B) { return B.a + A.a * (1. - B.a); }

vec4 blendOver(vec4 A, vec4 B) {
  float alpha = B.a + A.a * (1. - B.a);
  vec3 rgb = (B.rgb * B.a + A.rgb * A.a * (1. - B.a)) / alpha;
  return vec4(rgb, alpha);
}

void main() {
  vec4 color1 = getDrosteColor(0.0);
  vec4 color2 = getDrosteColor(1.0);

  // UNDO SHIFT AND SCALE:
  gl_FragColor = blendOver(color2, color1);
}