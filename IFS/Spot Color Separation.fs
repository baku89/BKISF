/*
{
	"DESCRIPTION": "Displaces an image in cartesian coordinate",
	"ISFVSN": "2",
	"CREDIT": "Baku Hashimoto <baku89.com>",
	"INPUTS": [
		{
			"NAME": "inputImage",
			"TYPE": "image"
		},
		{
			"NAME": "redColor",
			"LABEL": "Channel for Red",
			"TYPE": "color",
			"DEFAULT": [1, 0, 0, 1]
		},
		{
			"NAME": "greenColor",
			"LABEL": "Channel for Green",
			"TYPE": "color",
			"DEFAULT": [0, 1, 0, 1]
		},
		{
			"NAME": "blueColor",
			"LABEL": "Channel for Blue",
			"TYPE": "color",
			"DEFAULT": [0, 0, 1, 1]
		},
		{
			"NAME": "doClamp",
			"LABEL": "Clamp",
			"TYPE": "bool",
			"DEFAULT": true
		},
		{
			"NAME": "useTwo",
			"LABEL": "Only Use Red/Green",
			"TYPE": "bool",
			"DEFAULT": false
		},
		{
			"NAME": "useCMY",
			"LABEL": "Use CMY",
			"TYPE": "bool",
			"DEFAULT": true
		}
	]
}
*/

mat3 inverse(mat3 m) {
  float a00 = m[0][0], a01 = m[0][1], a02 = m[0][2];
  float a10 = m[1][0], a11 = m[1][1], a12 = m[1][2];
  float a20 = m[2][0], a21 = m[2][1], a22 = m[2][2];

  float b01 = a22 * a11 - a12 * a21;
  float b11 = -a22 * a10 + a12 * a20;
  float b21 = a21 * a10 - a11 * a20;

  float det = a00 * b01 + a01 * b11 + a02 * b21;

  return mat3(b01, (-a22 * a01 + a02 * a21), (a12 * a01 - a02 * a11), b11, (a22 * a00 - a02 * a20), (-a12 * a00 + a02 * a10), b21,
	      (-a21 * a00 + a01 * a20), (a11 * a00 - a01 * a10)) /
	 det;
}

mat3 transpose(mat3 m) {
  return mat3(m[0][0], m[1][0], m[2][0], m[0][1], m[1][1], m[2][1], m[0][2], m[1][2], m[2][2]);
}

void main() {
  vec4 inputColor = IMG_THIS_PIXEL(inputImage);

  vec3 red = redColor.rgb;
  vec3 green = greenColor.rgb;
  vec3 blue = blueColor.rgb;

  if (useCMY) {
    red = 1.0 - red;
    green = 1.0 - green;
    blue = 1.0 - blue;
  }

  if (useTwo) {
    blue = cross(red, green);
  }

  mat3 m = inverse(mat3(red, green, blue));

  vec3 outColor = m * inputColor.rgb;

  if (doClamp) {
    outColor = clamp(outColor, 0.0, 1.0);
  }

  gl_FragColor = vec4(outColor, inputColor.a);
}