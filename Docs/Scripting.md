# Scripting in Cinder Engine

## Basic Info
* The main purpose of scripts are to control the flow of the story, store relevant data (ex. inventory), or whatever is needed.
    They can also change the data of nodes.
* Scripts are written in Lua and can be attached to nodes.
* A single node can have a single script. They can be added by adding the Script's ID next to the Node's ID (`nodeID : scriptID`).
* You can insert variables into a node's text by using `{variable}`. See the docs for [Nodes](NodeFile.md) for more info.

## Important Notes
* As soon as a non-local variable or function is defined, it will stay within the scripting environment until the engine is reset.
Keep this in mind when switching nodes.
* Due to the above, consider having a single-run init script that defines all needed variables and functions.

## Setting Up
First, create a new `.lua` file. Now on the first line, add in a comment with the script's ID. (ex. `--MyScript`).

To connect a node to a script, go to your node's file and append the Node ID line with a colon (:) and your script's ID.
(ex. `MyNode : MyScript`)

Now your script will run whenever the node is swapped to!

## Built-in Engine Functions

All parameters below are expected to be strings.

`SetCurrentNode(nodeId)` 

    Change to the given node by it's ID.

`SetNodeText(text)`

    Change the currently active node's text.

`GetNodeText`

    Returns the currently active node's text.

`GetNodeId()`

    Returns the currently active node's ID.

`GetAsset(assetId)`

    Returns the asset by its ID.

`ResetEngine()`

    Fully resets the engine, including the Lua environment and rereading the game file.

`ClearEnvironment()`

    Resets the scripting environment.

`FormatNodeText()`

    Inserts variables and assets into the currently active node.

## Example
In `MyNode.node`
```
MyNode : MyScript
Hello! This is a test for scripting!
This node's ID is {myVar}!
_
```

In `MyScript.lua`
```lua
--MyScript
myVar = GetNodeId()
```