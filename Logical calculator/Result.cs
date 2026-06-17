using System;
using System.Collections.Generic;
using NCalc;

namespace Logical_calculator;

public class Result
{
    public string equation;
    public Dictionary<string, bool> values;
    public bool result;
    public bool writeResult;
    public string[] steps;
    
    public Result(Expression exp)
    {
        values = new Dictionary<string, bool>();
        equation = exp.ToString();
        foreach (var row in exp.Parameters)
        {
            values[row.Key] = (bool)row.Value;
        }
        result = (bool)exp.Evaluate();
    }
}