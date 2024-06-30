using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class GestureSequence
{
    public List<GestureSequenceElement> Elements;
    private int _currentElement;

    private List<GestureSequenceElement> GetCurrentElements()
    {
        return Elements.FindAll(e => e.Position == _currentElement);
    }
    
    private List<GestureSequenceElement> GetElementsAtPos(int position)
    {
        return Elements.FindAll(e => e.Position == position);
    }

    public void NextElement()
    {
        _currentElement++;
    }

    private int GetHighestPosition()
    {
        return Elements.Select(element => element.Position).Prepend(0).Max();
    }

    public bool IsSequenceFinished()
    {
        return _currentElement >= GetHighestPosition();
    }
}