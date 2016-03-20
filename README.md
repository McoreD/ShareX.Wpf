# ShareX.Wpf
WPF implementation of reimagined ShareX

ShareX.Wpf is an attempt to reimagine ShareX using WPF. 

## Current issues
* System.Timer or System.Threading.Timer cannot be used to track mouse position due to cross-thread operation errors; 
affects: Annotation.cs and RectangleLight.cs

## Features currently being implemented
* Uploaders plugin support
* Image Editor

## Featured to be implemented
* Hotkey support
