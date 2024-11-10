using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    bool IsActive { get; }
    public void UseItem();
}
