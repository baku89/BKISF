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

  mat3 m = mat3(red, green, blue);

  vec3 outColor = m * inputColor.rgb;

  gl_FragColor = vec4(outColor, inputColor.a);
}