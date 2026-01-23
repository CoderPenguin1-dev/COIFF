# Cinder Engine Node File

## Basic Info
* Each section of the story is put into a node file.
* Nodes contain the text and options of the section.
* Assets and variables can be inserted into the text using {id}.
    * Variables have a higher priority than assets.
    * This also makes both `{` and `}` reserved characters.
    * This also happens prior to any scripts being ran.
* All text (including option text) are trimmed.
    * If you require untrimmed text (ex. ASCII art), please see the docs on [Assets](AssetFile.md).

## Setting Up
First create a new file with the `.node` extension.

In the file, it should structed as seen below.
```
NodeId
Node Text
_
nodeId : Option Text
```

The ID *has* to be on Line 1. The node's text can be on as many lines as needed, as long as it's ended by an underscore (_).

Each option needs a node that it will go it, as well as the text that will be displayed. There can also be as many options as needed.

## Example
```
MyNode1
Hello, World!
Lorem Ipsum!
Many other placeholders!
_
MyNode2 : Go To Node 2
MyNode3: Go To Node 3
```