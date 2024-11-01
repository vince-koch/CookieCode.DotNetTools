# dnt

### Command List

* [bump-version](#bump-version)
* [cleanup](#cleanup)
* [doc](#doc)
* [fix-namespaces](#fix-namespaces)
* [fix-project-refs](#fix-project-refs)
* [generate-docs](#generate-docs)
* [handle](#handle)
* [pack](#pack)
* [prune](#prune)
* [send-http](#send-http)
* [unzip](#unzip)
* [zip](#zip)
* [zip-publish](#zip-publish)
* [zip-source](#zip-source)

Generated at 10/31/2024 10:57:58 PM

### bump-version

Bumps the specified portion of the project version

|Arguments |Description |
|----------|------------|
|Position 0|Project file path<br/>Required|
|Position 1|Project file path<br/>Default: Build|

### cleanup

Zips a directory of source files, ignoring files specified by .gitignore

|Arguments |Description |
|----------|------------|
|-s<br/>--source|Set the starting folder|
|-r<br/>--rules|Add one or more exclude pattern rules|
|--is-dry-run|List paths that will be removed|
|--is-confirmed|Assumes confirmation and does not confirm with user|

### doc

Document an assembly

### fix-namespaces

Update class namespaces to match folder and file names

|Arguments |Description |
|----------|------------|
|-f<br/>--folder|Root folder to begin processing at<br/>Required|
|-n<br/>--namespace|Root namespace to apply to the folder<br/>Required|

### fix-project-refs

Attempts to fix project references

|Arguments |Description |
|----------|------------|
|Position 0|Source folder|

### generate-docs

Document an assembly

|Arguments |Description |
|----------|------------|
|Position 0<br/>-s<br/>--source|Source assembly path<br/>Required|
|Position 1<br/>-t<br/>--target|Target document path<br/>Required|

### handle

A tool to identify which process has a file or directory handle locked, and optionally kill the process.  Note that this tool works by downloading Microsoft SysInternals handle.exe and parsing it's output.

|Arguments |Description |
|----------|------------|
|Position 0|File or directory to look for|

### pack



|Arguments |Description |
|----------|------------|
|Position 0|Source folder, project, or solution|

### prune

Remove empty folders

|Arguments |Description |
|----------|------------|
|Position 0|Starting directory|

### send-http

Sends a raw http request

|Arguments |Description |
|----------|------------|
|Position 0<br/>-i<br/>--input|The path to the text file containing the HTTP request to send|
|-t<br/>--timeout|The timeout (in seconds) to wait, default is 60|

### unzip

Unzips a zip archive

|Arguments |Description |
|----------|------------|
|-z<br/>--zip|Source zip archive<br/>Required|
|-t<br/>--target|Target folder<br/>Required|
|-o<br/>--overwrite|Overwrite files<br/>Default: True|

### zip

Zips a set of source files and folders

|Arguments |Description |
|----------|------------|
|-s<br/>--source|Source paths<br/>Required|
|-z<br/>--zip|Zip archive path<br/>Required|

### zip-publish

Zips publish output

### zip-source

Zips a directory of source files, ignoring files specified by .gitignore

|Arguments |Description |
|----------|------------|
|-s<br/>--source|Set the starting folder|
|-z<br/>--zip|Path of the zip file|
|-r<br/>--rules|Add one or more exclude pattern rules|

