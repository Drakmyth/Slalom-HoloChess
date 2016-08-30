using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts
{

    public interface IMonsterModel
    {
        GameObject UnityObject { get; set; }
        SpaceNode CurrentNode { get; set; }
    }

}