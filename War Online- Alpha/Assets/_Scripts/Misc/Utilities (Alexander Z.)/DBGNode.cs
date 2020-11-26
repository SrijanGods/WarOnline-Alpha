using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DBGNode  {

    public List<DBGNode> children;
    public DBGNode parent;

    public string name;
    public bool fsm;
    public float t0;

    public DBGNode()
    {
        t0 = 0;
        parent = null;
        children = new List<DBGNode>();
        fsm = false;
    }
}
