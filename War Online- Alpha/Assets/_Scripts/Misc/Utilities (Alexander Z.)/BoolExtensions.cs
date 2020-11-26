using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoolExtensions 
{
    /// <summary>
    /// Simulates a token using the boolean variable.
    /// Only returns true if the boolean value is true. Then switches it off. In essence consuming the value true. Replacing it for False.
    ///  If the value is false, it returns false.
    /// </summary>
    /// <param name="token"> itself, "this bool" </param>
    /// <returns>returns true if true but switches it to false, returns false if false</returns>
    public static bool ConsumeToken(this ref bool token)
    {
        if (token == false)
            return false;

        token = false;
        return true;
    }
}
