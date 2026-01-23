# Cinder Engine Story File

## Basic Info
* The file where all of the story's data (scripts, nodes, assets) are stored.
* The file is a zip-like format.
* Folders are not checked when reading.
* The preferred file extension is `.cstory`

## Setting Up
First, create a new ZIP archive (ex. `game.cstory`). Inside you'll want to make a new file titled `metadata`. Please read the [Metadata](##Metadata) section for more information.

Then put all of your node, asset, and script files into the archive as well.

## Metadata
In the file, it should structed as seen below.
```
Story Name
Story Author
File Version
Starting Node ID
```
The *Story Name* indicates what the story will be called. *Story Author* indicating who created the story.
*File Version* indicates what version the story is. This **does not** mean what Cinder Engine version.
Finally, *Starting Node ID* indicates which node the story starts at.

## Example Structure
```
game.cstory
    metadata
    MyAsset.asset
    MyNode.node
    MyScript.lua
```