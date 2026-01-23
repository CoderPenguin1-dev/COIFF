using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lua;
using Lua.Standard;

namespace CinderEngine;

public class Game
{
    #region Fields
    public string GameTitle { get; private set; }
    public string Author { get; private set; }
    public string GameVersion { get; private set; }
    
    private readonly string _filePath;
    private LuaState _luaState;
    private string StartingId { get; set; }
    private readonly List<StoryNode> _nodes = [];
    private readonly List<Asset> _assets = [];
    private readonly List<LuaScript> _scripts = [];
    private StoryNode _currentNode;
    #endregion
    
    public Game(string gameFilePath)
    {
        _filePath = gameFilePath;
        ResetEngine();
    }

    private void InitScriptApiFunctions()
    {
        // SetCurrentNode(nodeId) -- Sets the current active node.
        _luaState.Environment["SetCurrentNode"] = new LuaFunction((context, ct) =>
        {
            var arg1 = context.GetArgument<string>(0);
            SetCurrentNode(arg1);
            return new(0);
        });
        
        // SetNodeText(text) -- Sets the current node's text. Isn't permanent.
        _luaState.Environment["SetNodeText"] = new LuaFunction((context, ct) =>
        {
            var arg1 = context.GetArgument<string>(0);
            StoryNode node = new()
            {
                Text = arg1,
                NodeId = _currentNode.NodeId,
                ScriptId = _currentNode.ScriptId,
                OptionIDs = _currentNode.OptionIDs,
                Options = _currentNode.Options
            };
            _currentNode = node;
            return new(0);
        });
        
        // GetNodeText() -- Returns the current node's text.
        _luaState.Environment["GetNodeText"] = new LuaFunction((context, ct) =>
        {
            context.Return(GetNodeText());
            return new(0);
        });
        
        // GetNodeId() -- Returns the current node's ID.
        _luaState.Environment["GetNodeId"] = new LuaFunction((context, ct) =>
        {
            context.Return(_currentNode.NodeId);
            return new(0);
        });
        
        // GetAsset(assetId) -- Returns the string of the request asset file.
        _luaState.Environment["GetAsset"] = new LuaFunction((context, ct) =>
        {
            var arg1 =  context.GetArgument<string>(0);
            foreach (var asset in _assets)
            {
                if (asset.AssetId == arg1)
                {
                    context.Return(asset.Text);
                    break;
                }
            }
            return new(0);
        });
        
        // ResetEngine() -- Fully resets the engine, rereading the game file.
        _luaState.Environment["ResetEngine"] = new LuaFunction((context, ct) =>
        {
            ResetEngine();
            return new(0);
        });
        
        // ClearEnvironment() -- Clears the engine's Lua environment.
        _luaState.Environment["ClearEnvironment"] = new LuaFunction((context, ct) =>
        {
            _luaState = LuaState.Create();
            _luaState.OpenStandardLibraries();
            InitScriptApiFunctions();
            return new(0);
        });

        _luaState.Environment["FormatNodeText"] = new LuaFunction(async (context, ct) =>
        {
            _currentNode.Text = await FormatNodeText(_currentNode.Text);
            return 0;
        });
    }
    
    private void InitGame()
    {
        ZipArchive archive = ZipFile.OpenRead(_filePath);
        foreach (var entry in archive.Entries)
        {
            if (entry.Name.Equals("metadata", StringComparison.CurrentCultureIgnoreCase))
            {
                var stream = entry.Open();
                StreamReader reader = new(stream);
                GameTitle = reader.ReadLine();
                Author = reader.ReadLine();
                GameVersion = reader.ReadLine();
                StartingId = reader.ReadLine();
                reader.Close();
            }

            if (entry.FullName.EndsWith(".node", StringComparison.CurrentCultureIgnoreCase))
            {
                StoryNode node = new();
                var stream = entry.Open();
                StreamReader reader = new(stream);
                
                string line = reader.ReadLine();
                if (line.Contains(':')) // Check if there's a connected script.
                {
                    node.NodeId = line.Substring(0, line.IndexOf(':')).Trim();
                    node.ScriptId = line.Substring(line.IndexOf(':') + 1).Trim();
                }
                else
                {
                    node.NodeId = line;
                    node.ScriptId = string.Empty;
                }
                
                node.Text = "";
                line = reader.ReadLine();
                while (line != "_")
                {
                    node.Text += line + '\n';
                    line = reader.ReadLine();
                }

                // Parse the options.
                line = reader.ReadLine();
                List<string> optionIds = [];
                List<string> options = [];
                while (line != null)
                {
                    string id = "";
                    foreach (char c in line)
                    {
                        if (c == ':') break;
                        id += c;
                    }
                    optionIds.Add(id.Trim());
                    options.Add(line.Substring(line.IndexOf(':') + 1).Trim());
                    line = reader.ReadLine();
                }
                
                node.OptionIDs = optionIds.ToArray();
                node.Options = options.ToArray();
                _nodes.Add(node);
            }

            if (entry.FullName.EndsWith(".asset", StringComparison.CurrentCultureIgnoreCase))
            {
                var stream = entry.Open();
                StreamReader reader = new(stream);
                Asset asset = new() {AssetId = reader.ReadLine(), Text = ""};
                asset.Text = "";
                
                string line = reader.ReadLine();
                while (line != null)
                {
                    asset.Text += $"{line}\n";
                    line = reader.ReadLine();
                }
                _assets.Add(asset);
            }

            if (entry.FullName.EndsWith(".lua", StringComparison.CurrentCultureIgnoreCase))
            {
                LuaScript lua = new();
                lua.Script = "";
                var stream = entry.Open();
                StreamReader reader = new(stream);
                lua.ScriptId = reader.ReadLine()[2..].Trim(); // Removes the "--" from the line.
                
                string line = reader.ReadLine();
                while (line != null)
                {
                    lua.Script += $"{line}\n";
                    line = reader.ReadLine();
                }
                _scripts.Add(lua);
            }
        }
        
        SetCurrentNode(StartingId);
        archive.Dispose();
    }
    
    private async void SetCurrentNode(string nodeId)
    {
        foreach (var node in _nodes)
        {
            if (node.NodeId == nodeId)
            {
                _currentNode = new StoryNode()
                {
                    NodeId = nodeId,
                    Text = await FormatNodeText(node.Text),
                    Options = node.Options,
                    OptionIDs = node.OptionIDs,
                    ScriptId = node.ScriptId
                };
                
                if (_currentNode.ScriptId != string.Empty)
                {
                    foreach (var script in _scripts)
                    {
                        if (script.ScriptId == _currentNode.ScriptId)
                        {
                            await _luaState.DoStringAsync(script.Script);
                            break;
                        }
                    }
                }
                return;
            }
        }
    }

    private async Task<string> FormatNodeText(string text)
    {
        List<string> insertionVariables = [];
        bool inInsertion = false;
        string currentVarName = "";
        foreach (char c in text)
        {
            if (c == '{') // The start of an insertion.
            {
                inInsertion = true;
                continue;
            }

            if (c == '}') // The end of an insertion.
            {
                inInsertion = false;
                insertionVariables.Add(currentVarName);
                currentVarName = "";
                continue;
            }
                    
            if (inInsertion)
                currentVarName += c;
        }

        foreach (string variableName in insertionVariables)
        {
            var variable = await _luaState.DoStringAsync($"return {variableName}");
            if (variable[0].TypeToString() != "nil") // Checking if the variable doesn't exist. Must be an asset instead.
                text = text.Replace($"{{{variableName}}}", $"{variable[0]}");
        }
        
        // Insert assets.
        foreach (var asset in _assets)
            text = text.Replace($"{{{asset.AssetId}}}", asset.Text);
        
        return text;
    }
    
    public void ParseSelectedOption(int optionIndex)
    {
        SetCurrentNode(_currentNode.OptionIDs[optionIndex]);
    }

    public string GetNodeText()
    {
        return _currentNode.Text;
    }

    public string[] GetNodeOptions()
    {
        return _currentNode.Options;
    }

    public void ResetEngine()
    {
        _luaState = LuaState.Create();
        _luaState.OpenStandardLibraries();
        InitScriptApiFunctions();
        InitGame();
    }
}