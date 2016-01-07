A ShrinkingPanel is like a StackPanel that can optionally shrink a child element.

Available on NuGet with package ID "ShrinkingPanel"

Here's how it works:
- If all children fit in the available direction (specified by Orientation), then they all get that space.
- If not, the child at index ShrinkingElementIndex will be shrunk so everything fits.

The ShrinkingPanel is designed to be used if you have a child element that can do its own scrolling, like a ListBox> or a ScrollViewer.  If you put that element at the ShrinkingElementIndex, then you can put controls after it that will show up, but won't get pushed to the bottom/right unless the ShrinkingElement needs all of the available space.
 
