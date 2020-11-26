using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class DBG //: MonoBehaviour
{

    //public static DBG Instance { get; private set; }

    static DBGNode current_node;

    static float creation_time = 0f;
    static void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        creation_time = Time.time;

        //Instance = this;

        //Initialize vars
        method_call_stack = new Stack<string>();
        current_node = new DBGNode();
    }

    static void Start()
    {
        //DontDestroyOnLoad(transform.gameObject);
    }

    //void Update()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        //KEEP 
    //        if (creation_time < Instance.creation_time)
    //        {
    //             Debug.Log("Destroying creation time:" + Instance.creation_time);
    //            Destroy(Instance.gameObject);
    //        }
    //        else
    //        {
    //             Debug.Log("Creation time:" + creation_time);
    //            Destroy(gameObject);
    //        }
    //        // Destroy(gameObject);
    //    }
    //}

    static private string indentation;
    static public bool log_enabled = true;
    static public void BypassLog(string s)
    {
        if (current_node.fsm)
            Debug.Log(indentation + current_node.name + ":" + s);
        else
            Debug.Log(indentation + s);
    }

    static public void Log(string s, bool condition = true, string color = "black" )
    {
        //Debug.LogWarning("indentation [" + indentation + "]");
        switch (type)
        {
            case DBG_Types.ALL:
                if (condition == true)
                {
                    if (current_node != null && current_node.fsm)
                        Debug.Log(indentation + current_node.name + ":" + s);
                    else
                    {
                        Debug.Log("<color=" + color + ">" + indentation + s + "</color>");
                        //"<color=red>" + (Time.realtimeSinceStartup - current_node.t0).ToString("0.000") + "s </color> ";
                    }
                }
                break;
            case DBG_Types.METHOD_LIST_ONLY:
                //if the last element in the stackis not a method we are loking for, do not display
                //Debug.LogWarning("METHOD_LIST_ONLY" + log_enabled + ":" + condition);
                if (methods_to_track.Contains(current_node.name))
                    if (condition == true)
                    {
                        if (current_node.fsm)
                            Debug.Log(indentation + current_node.name + ":" + s);
                        else
                            Debug.Log(indentation + s);
                    }
                break;
            case DBG_Types.DEEP_METHOD_LIST:
                //if stack contains one of the methods we are looking for display because this is a sub of it
                bool log_enabled = false;
                foreach (string method in method_call_stack)
                    if (methods_to_track.Contains(method))
                        log_enabled = true;

                if (log_enabled)
                    if (condition == true)
                    {
                        if (current_node.fsm)
                            Debug.Log(indentation + current_node.name + ":" + s);
                        else
                            Debug.Log(indentation + s);
                    }
                break;
            //case DBG_Types.BRANCHES:
            //    foreach (DBG_branch b in root.branches)
            //        if (b.active)
            //            if (b.method_list.Count > 0 && b.method_list.Contains(current_node.name))
            //            {
            //                if (condition == true)
            //                {
            //                    if (current_node.fsm)
            //                        Debug.Log(indentation + current_node.name + ":" + s);
            //                    else
            //                        Debug.Log(indentation + s);
            //                }
            //            }
            //    break;
            default:
                break;
        }
    }

    static public void Log(string s, string color)
    {
        Log(s, true, color);
    }
    //public void Transition(string s, bool condition = true)
    //{
    //    //Debug.LogWarning("indentation [" + indentation + "]");
    //    switch (type)
    //    {
    //        case DBG_Types.ALL:
    //            if (condition == true)
    //            {
    //                if (current_node.fsm)
    //                    Debug.Log(indentation + current_node.name + ":" + s);
    //                else
    //                    Debug.Log(indentation + s);
    //            }
    //            break;
    //        case DBG_Types.METHOD_LIST_ONLY:
    //            //if the last element in the stackis not a method we are loking for, do not display
    //            //Debug.LogWarning("METHOD_LIST_ONLY" + log_enabled + ":" + condition);
    //            if (methods_to_track.Contains(current_node.name))
    //                if (condition == true)
    //                {
    //                    if (current_node.fsm)
    //                        Debug.Log(indentation + current_node.name + ":" + s);
    //                    else
    //                        Debug.Log(indentation + s);
    //                }
    //            break;
    //        case DBG_Types.DEEP_METHOD_LIST:
    //            //if stack contains one of the methods we are looking for display because this is a sub of it
    //            bool log_enabled = false;
    //            foreach (string method in method_call_stack)
    //                if (methods_to_track.Contains(method))
    //                    log_enabled = true;

    //            if (log_enabled)
    //                if (condition == true)
    //                {
    //                    if (current_node.fsm)
    //                        Debug.Log(indentation + current_node.name + ":" + s);
    //                    else
    //                        Debug.Log(indentation + s);
    //                }
    //            break;
    //        case DBG_Types.BRANCHES:
    //            foreach (DBG_branch b in root.branches)
    //                if (b.active)
    //                    if (b.method_list.Count > 0 && b.method_list.Contains(current_node.name))
    //                    {
    //                        if (condition == true)
    //                        {
    //                            if (current_node.fsm)
    //                                Debug.Log(indentation + current_node.name + ":" + s);
    //                            else
    //                                Debug.Log(indentation + s);
    //                        }
    //                    }
    //            break;
    //        default:
    //            break;
    //    }
    //}

    static public void indent()
    {
        //Debug.LogWarning("indent");
        switch (type)
        {
            case DBG_Types.ALL:
                indentation += "\t";
                break;
            case DBG_Types.METHOD_LIST_ONLY:
                if (methods_to_track.Contains(current_node.name))
                    indentation += "\t";
                    break;
            case DBG_Types.DEEP_METHOD_LIST:
                bool enabled = false;
                foreach (string method in method_call_stack)
                    if (methods_to_track.Contains(method))
                        enabled = true;

                if (enabled)
                    indentation += "\t";
                break;
            //case DBG_Types.BRANCHES:
            //    foreach (DBG_branch b in root.branches)
            //        if (b.active)
            //            if (b.method_list.Contains(current_node.name))
            //            {
            //                indentation += "\t";
            //            }
            //    break;
            default:
                break;
        }
    }

    static public void unindent()
    {
       // Debug.LogWarning("unindent");

        switch (type)
        {
            case DBG_Types.ALL:
                remove_tab();
                break;
            case DBG_Types.METHOD_LIST_ONLY:
                if (methods_to_track.Contains(current_node.name))
                    remove_tab();
                break;
            case DBG_Types.DEEP_METHOD_LIST:
                bool enabled = false;
                foreach (string method in method_call_stack)
                    if (methods_to_track.Contains(method))
                        enabled = true;

                if (enabled)
                    remove_tab();
                break;
            //case DBG_Types.BRANCHES:
            //    foreach (DBG_branch b in root.branches)
            //        if (b.active)
            //            if (b.method_list.Contains(current_node.name))
            //            {
            //               remove_tab();
            //            }
            //    break;
            default:
                break;
        }
    }

    static private void remove_tab()
    {
        if (indentation.Length >= 1)
            indentation = indentation.Substring(0, indentation.Length - 1);
        else
            indentation = "";
    }


    //----------METHOD NAMES-----------
    static private Stack<string> method_call_stack;
    static private bool display_sub_calls;
    public enum DBG_Types { ALL, METHOD_LIST_ONLY, DEEP_METHOD_LIST, BRANCHES }
    static public DBG_Types type = DBG_Types.ALL;
    static public List<string> methods_to_track;
    static public List<DBGNode> fsm_stack;
    //Focus Debugging.
    static public void BeginMethod(string method_name,bool condition = true,string color = "blue")
    {
        //Make leaf, goto leaf
        DBGNode new_node = new DBGNode();
        new_node.t0 = Time.realtimeSinceStartup;
        new_node.parent = current_node;
        new_node.name = method_name;
        if(current_node==null)
            current_node = new DBGNode();

        current_node.children.Add( new_node );
        
        current_node = new_node;
        //--
        switch(type){
            case DBG_Types.ALL:
                if (method_call_stack == null)
                    method_call_stack = new Stack<string>();
                method_call_stack.Push(method_name);
                if (current_node.fsm)
                    Debug.Log(indentation + current_node.name + ":" + "BEGIN: " + method_name);
                else {
                    Debug.Log(indentation + "BEGIN: " + method_name);
                    //Debug.Log(   indentation + "<color=" + color + ">" +
                    //             current_node.name + ":" + "BEGIN: " + method_name +
                    //             "</color>");
                }
                //Log( "BEGIN: " +method_name,condition);
                indent();
                break;
            case DBG_Types.METHOD_LIST_ONLY:
                method_call_stack.Push(method_name);
                if (methods_to_track.Contains(method_name))
                {
                    Log("BEGIN: " + method_name, condition);
                    indent();
                }   
                break;
            case DBG_Types.DEEP_METHOD_LIST:
                method_call_stack.Push(method_name);

                if (method_call_stack.Contains(method_name) || method_call_stack.Count > 0)
                {
                    Log("BEGIN: " + method_name, condition);
                    indent();
                }
                break;
            //case DBG_Types.BRANCHES:
            //    method_call_stack.Push(method_name);
            //    foreach (DBG_branch b in root.branches)
            //        if (b.active)
            //            if (b.method_list.Contains(method_name))
            //            {
            //                Log("BEGIN: " + method_name, condition);
            //                indent();
            //            }
            //    break;
            default:
                break;
        }

    }

    static public bool show_method_execution_time = true;
    static public float execution_time_threshhold = 1f;
    //DBG.EndMethod(testing_method, true)

    static public void EndMethod(string method_name,bool condition = true)
    {
        switch (type)
        {
            case DBG_Types.ALL:
                method_call_stack.Pop();
                unindent();
                string s = "";
                if (show_method_execution_time)
                {
                    float execution_time = (Time.realtimeSinceStartup - current_node.t0);
                    if (execution_time >= execution_time_threshhold)
                    {
                        s = "END: " + method_name + "-" + "<color=red>" + (Time.realtimeSinceStartup - current_node.t0).ToString("0.000") + "s </color> ";
                        if (current_node.fsm)
                            Debug.Log(indentation + current_node.name + ":" + s);
                        else
                            Debug.Log(indentation + s);
                        //Log("END: " + method_name + "-" + "<color=red>" + (Time.realtimeSinceStartup - current_node.t0).ToString("0.000") + "s </color> ", condition);
                    }
                    else
                    {
                        s = "END: " + method_name + "-" + (Time.realtimeSinceStartup - current_node.t0).ToString("0.000") + "s";
                        if (current_node.fsm)
                            Debug.Log(indentation + current_node.name + ":" + s);
                        else
                            Debug.Log(indentation + s);
                        //Log("END: " + method_name + "-" + (Time.realtimeSinceStartup - current_node.t0).ToString("0.000") + "s", condition);
                    }
                }
                else
                {
                    s = "END: " + method_name;
                    if (current_node.fsm)
                        Debug.Log(indentation + current_node.name + ":" + s);
                    else
                        Debug.Log(indentation + s);
                    //Log("END: " + method_name, condition);
                }
                    break;
            case DBG_Types.METHOD_LIST_ONLY:
                if (methods_to_track.Count > 0 && methods_to_track.Contains(method_name))
                {
                    method_call_stack.Pop();
                    unindent();
                    Log("END: " + method_name, condition);
                }
                break;
            case DBG_Types.DEEP_METHOD_LIST:
                if ( method_call_stack.Count > 0)
                {
                    unindent();
                    Log("END: " + method_name, condition);
                    method_call_stack.Pop();
                }
                break;
            //case DBG_Types.BRANCHES:
            //    foreach(DBG_branch b in root.branches)
            //        if(b.active)
            //            if (b.method_list.Count > 0 && b.method_list.Contains(method_name))
            //            {
            //                method_call_stack.Pop();
            //                unindent();
            //                Log("END: " + method_name , condition);
            //            }
            //    break;
            default:
                break;
        }

        if (current_node.parent != null)
            current_node = current_node.parent;

    }

    static public void BeginFSM(string method_name)
    {

        ////method_call_stack.Add(method_name);

        //DBGNode new_node = new DBGNode();
        //new_node.t0 = Time.realtimeSinceStartup;
        //new_node.parent = current_node;
        //new_node.name = method_name;
        //current_node.children.Add(new_node);

        //current_node = new_node;
        //current_node.fsm = true;

    }

    static public void EndFSM()
    {
        //method_call_stack.RemoveAt(method_call_stack.Count - 1);

        //current_node.fsm = false;

        //if (current_node.parent != null)
        //    current_node = current_node.parent;



        //// unindent();
    }

    static public void LogTransition(string s)
    {
        Debug.Log(indentation +  "<color=olive>" + s + "</color>");
    }
    static public void LogTransition(string s,bool condition)
    {
        if(condition)
            Debug.Log(indentation + "<color=olive>" + s + "</color>");
    }

    static public void Transition(string from, string to)
    {
        Debug.Log(indentation + "<color=olive>" + from + " -> " + to + "</color>");
    }

    static public void LogTransition(string s, string color)
    {
        Debug.Log(indentation + "<color="+color+">" + s + "</color>");
    }

    static public void BeginCoroutine(string method_name, bool condition = true)
    {

    }

    static public void EndCoroutine(string method_name, bool condition = true)
    {

    }

    static private float t0;
    static public void BeginTimer(string method_name )
    {
        t0 = Time.realtimeSinceStartup;
    }

    static public void EndTimer(string method_name )
    {
        Log(method_name + ":" + (Time.realtimeSinceStartup - t0).ToString("0.000")  );
    }

    static public void Assert(bool condition)
    {
        Debug.Assert(condition);
    }

    /*public struct DBG_branch
    {
        public List<string> method_list;
        DBG_branch a;
    }*/
//    [System.Serializable]
//    static public class DBG_branch
//    {
//        static public string name;
//        static public bool active;
//        static public List<string> method_list;
////        public List<DBG_branch> branch;
//    }

    //[System.Serializable]
    //static public class DBG_root
    //{
    //    static public List<DBG_branch> branches;
    //}

    //[System.Serializable]
    //static public class method
    //{
    //    static public string name;
    //    static public bool active;
    //}

    //static public DBG_root root;
}
