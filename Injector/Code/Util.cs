using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using System.Reflection.Emit;

public static class Util
{
    public static List<AssemblyNameReference> GetDifferenceBetweenLists(List<AssemblyNameReference> a, List<AssemblyNameReference> b)
    {
        List<string> ANames = new List<string>();
        for (int i = 0; i < a.Count; i++)
        {
            ANames.Add(a[i].FullName);
        }
        List<string> BNames = new List<string>();
        for (int i = 0; i < b.Count; i++)
        {
            BNames.Add(b[i].FullName);
        }



        List<AssemblyNameReference> output = new List<AssemblyNameReference>();
        for (int i = 0; i < ANames.Count; i++)
        {
            if (!BNames.Contains(ANames[i]))
            {
                output.Add(a[i]);
            }
        }
        return output;
    }

    public static void Test()
    {
        
    }
}