# UE4 Property Visualizer

Visual Studio extension for displaying the values of blueprint variables in the VS watch windows.
When adding a watch on a UObject, an extra entry "BP Properties" will be displayed if the object is of a blueprint class. Expanding this will list the blueprint variables.

NOTES:
- Requires VS 2015
- Works with Add Watch and Quickwatch. Currently does not work with the popup watch expansion resulting from hovering the cursor over a variable (reason unknown).
- Most types are supported. Interfaces yet to be added, possibly a few others.
- Slightly sluggish performance. Presume this comes from recursively calling back into the default visualization framework, when really we just want to evaluate expressions, not visualize them. 
- As of UE 4.10.1, the UE4.natvis file included in the engine for visualizing UE4 types in VS has an error. As a result, TMap/TSet do not display correctly. I have a corrected version available here: https://www.dropbox.com/s/2kcv3l5ei88ahj9/UE4.natvis?dl=0
It needs to be copied into <User>/Documents/Visual Studio 2015/Visualizers/.
