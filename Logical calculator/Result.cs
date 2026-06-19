using System;
using System.Collections.Generic;
using System.Linq;
using NCalc;

namespace Logical_calculator;

public class Result
{
    public string equation;
    public Dictionary<string, bool> values;
    public bool result;
    public static int countTrue;
    public static int countFalse;
    
    public Result(Expression exp)
    {
        values = new Dictionary<string, bool>();
        equation = exp.ToString();
        foreach (var row in exp.Parameters)
        {
            values[row.Key] = (bool)row.Value;
        }
        result = (bool)exp.Evaluate();
        if (result){countTrue++;}
        else{countFalse++;}
    }

    public string[] getData()
    {
        string[] answ = values.Values.Select(v => v ? "1":"0").Append(result ? "1" : "0").ToArray();
        return answ;
    }
}