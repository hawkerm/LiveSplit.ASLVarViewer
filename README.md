# ASL Var Viewer

A [LiveSplit](http://livesplit.github.io/) Information Component for displaying State/Variables from ASL Script based [AutoSplitters](https://github.com/LiveSplit/LiveSplit/blob/master/Documentation/Auto-Splitters.md).

Can read any *current/vars* variable within the loaded/running ASL script.  Configurable in standard Layout UI settings via drop-down when an ASL script is running.

Contains Standard Component Background/Foreground Color Settings.

## Instructions

Built for [LiveSplit v1.6.8](https://github.com/LiveSplit/LiveSplit/releases/tag/1.6.8)

Copy the *LiveSplit.ASLVarViewer.UI.dll* file into Components directory within LiveSplit directory before running LiveSplit.

For the best experience, configure your **Scriptable Auto Splitter** component first pointed to a script and load your game.  This will ensure the script is running and retrieving values.  For some scripts, you may want to start/stop a run first.

Then add an ASLVarViewer component.  Within the Layout Editor, you can choice a label and variable to display.  This can be from one of two places within an ASL Script (the current state or the general vars).

If the script is not running, the variable selection drop-down may be disabled.
