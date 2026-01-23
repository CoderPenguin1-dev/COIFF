# Cinder Engine Asset Files

## Basic Info
* Assets contain text that can be inserted into nodes.
* Unlike nodes, the text inside them has no reserved characters and is not trimmed.
    * Due to this, they **can't** have variables or assets inserted into them.
    You could, however, script something similar utilizing the `FormatNodeText()` function.

## Setting Up
First create a new file with the `.asset` extension.

In the file, it should structed as seen below.
```
AssetId
Asset Text
```
The text can take up as many lines as needed.