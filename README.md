# ShareX.Wpf
WPF implementation of reimagined ShareX

ShareX.Wpf is an attempt to reimagine ShareX using WPF. 

## Current issues
* System.Timer or System.Threading.Timer cannot be used to track mouse position due to cross-thread operation errors; 
affects: Annotation.cs and RectangleLight.cs
* One can only adjust size of the adorner that belongs to the last added annotation; affects: all annotations.
* public override void Render(DrawingContext dc) should call the Render() method to draw an DrawImage; affects: all annotations

