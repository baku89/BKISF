#!/usr/bin/node

const { exec } = require('child_process')

exec(
	'clang-format --style=file:.clang-format -i ./IFS/*.fs',
	(err, stdout, stderr) => {
		if (err) {
			console.log(`Failed: ${stderr}`)
			return
		}
		console.log(`stdout: ${stdout}`)
	}
)
