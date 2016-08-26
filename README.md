# UE4 Property Visualizer

UPDATE: Extension updated to reflect recent changes to the names of core UObject member variables. Should work with UE 4.12+.

Visual Studio extension for displaying the values of blueprint variables in the VS watch windows.
When adding a watch on a UObject, an extra entry "BP Properties" will be displayed if the object is of a blueprint class. Expanding this will list the blueprint variables.

NOTES:
- Requires VS 2015
- You should be using the UE4.natvis file included in the engine for visualizing UE4 types. It's in Engine/Extras/VisualStudioDebugging and needs to be copied into [User]/Documents/Visual Studio 2015/Visualizers/.
- It mostly works, but has been known to crash Visual Studio or sometimes fail to generate output.
- For some reason Add Watch and Quickwatch seem to be more reliable than the popup watch expansion resulting from hovering the cursor over a variable.
- Most types are supported. Interfaces yet to be added, possibly a few others.
- Due to how it harnesses the UE4 reflection system, sometimes watch information is generated for a variable of a type for which symbols are not available in the scope of the watch window's call stack. In these cases, the extension just does the best it can, which generally means a more raw formatting of the value and adding a ? in the variable type column.
- Slightly sluggish performance. Presume this comes from recursively calling back into the default visualization framework, when really we just want to evaluate expressions, not visualize them. 

HELP ME!
I think this would be a pretty cool tool to have working properly, and maybe incorporated into UnrealVS eventually. Unfortunately though just getting this to its current semi-working state was one of the toughest programming tasks I've ever faced, and I don't have much time to keep it updated let alone improve it. If anyone has any VS extension experience and wants to contribute, that would be awesome. Just bear in mind that due to what this is doing under the hood, it (mostly) necessarily involves some really evil code.

Of course, just getting any feedback on issues anyone has would also be useful and help keep me motivated.
