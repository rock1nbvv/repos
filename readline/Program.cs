using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using static System.Math;

namespace readline
{
    class Program
    {
        static int Operations(string x)
        {
            switch(x)
            {
                case "(":
                case ")":
                    return -1;
                case "+":
                case "-":
                case "\\/":
                    return 1;
                case "*":
                case "/":
                case "/\\":
                    return 2;
                case "^":
                case "!":
                    return 3;
                case "pi":
                    return 4;
                case "e":
                    return 5;
                case "sin":
                case "cos":
                case "tan":
                    return 6;
                default:
                    return 0;
            }
        }

        static int BinaryFunctions(string x)
        {
            switch (x)
            {
                case "/\\":
                case "\\/":
                    return 1;
                case "!":
                    return 2;
                default:
                    return 0;
            }
        }
        static double Counting(double a, double b, string operation)
        {
            switch(operation)
            {
                case "+":
                    return a + b;
                case "-":
                    return a - b;
                case "*":
                    return a * b;
                case "/":
                    return a / b;
                case "^":
                    return Math.Pow(a, b);
                case "/\\":
                    if (a == 1 && b == 1) return 1; else return 0;
                case "\\/":
                    if (a == 0 && b == 0) return 0; else return 1;
                default:
                    return 0;
            }
        }

        static double Convert(double a, string func)
        {
            switch(func)
            {
                case "sin":
                    return Math.Sin(a);
                case "cos":
                    return Math.Cos(a);
                case "tan":
                    return Math.Tan(a);
                case "!":
                    if (a == 1) return 0; else return 1;
                default: return 0;
            }
        }

        static int Functions(string x)
        {
            switch (x)
            {
                case "sin":
                case "cos":
                case "tan":
                    return 1;
                default:
                    return 0;
            }
        }
        static string Variables(string exp, string v)
        {
            v = v.Replace(" ", "");
            string[] v_parsed = v.Split(';'), v_parsed2 = null;
            for(int i =0; i<v_parsed.Length;i++)
            {
                v_parsed2 = v_parsed[i].Split('=');
                if (exp.Contains(v_parsed2[0])) exp = exp.Replace(v_parsed2[0], v_parsed2[1]);
            }
            return exp;
        }

        static List<string> ParseEx(string exp, string v)
        {
            if(v !="") exp=Variables(exp, v);
            int i = 0;
            double s = 0;
            string str = null, func = null;         
            char[] arr = exp.ToCharArray();
            List<string> ops = new List<string>();
           
            while (i < arr.Length)
            {
                if (i<(arr.Length-1) && (Operations(arr[i].ToString()+arr[i+1].ToString()) == 4))
                {
                        str += Math.PI;
                        if (str.Contains(",")) str = str.Replace(",", ".");
                        ops.Add(str);
                        str = null;
                        i++;                   
                }
                else if (Operations(arr[i].ToString()) == 5)
                {                   
                    str += Math.E;
                    if (str.Contains(",")) str = str.Replace(",", ".");
                    ops.Add(str);
                    str = null;
                    i++;
                    continue;
                }            
                else if (arr[i].ToString() == " ")
                {
                    i++;
                    continue;
                }
                else if (BinaryFunctions(arr[i].ToString()) != 0 || ((arr[i].ToString() == "/") || (arr[i].ToString().ToString() == "\\")))
                {
                    if (BinaryFunctions(arr[i].ToString()) == 2) ops.Add(arr[i].ToString());
                    else if (BinaryFunctions(arr[i].ToString() + arr[i + 1].ToString()) == 1)
                    {
                        string bf = null;
                        for (int j = i; j < i + 2; j++)
                        {
                            bf += arr[j].ToString();
                            if (BinaryFunctions(bf) != 0)
                            {
                                ops.Add(bf);
                                i++;
                                break;
                            }

                        }
                    }
                    else if (Operations(arr[i].ToString()) != 0) ops.Add(arr[i].ToString());
                }
                else if ((i == 0 && arr[i].ToString() == "-" && double.TryParse(arr[i + 1].ToString(), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out s) || ((i != 0 && i != arr.Length) && arr[i - 1].ToString() == "(" && arr[i].ToString() == "-") && double.TryParse(arr[i + 1].ToString(), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out s)))
                {
                    str += arr[i].ToString();
                    i++;
                    continue;
                }                
                else if (Operations(arr[i].ToString()) != 0) ops.Add(arr[i].ToString());
                else if (double.TryParse(arr[i].ToString(), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out s))
                {

                    for (int k = i; k < arr.Length; k++)
                    {
                        if (double.TryParse(arr[k].ToString(), NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out s) || arr[k].ToString() == ".")
                        {
                            str += arr[k].ToString();
                            i++;
                        }
                        else
                        {
                            i = k - 1;
                            break;
                        }
                    }
                    ops.Add(str);
                    str = null;
                }
                else
                {
                    func += arr[i];
                    if (Functions(func) != 0) ops.Add(func);
                }
                i++;
            }
            return ops;
        }


        static List<string> RPN(string exp, string v)
        {
            List<string> res = new List<string>();
            double s = 0;
            Stack<string> stack = new Stack<string>(), operations = new Stack<string>();
            
            List<string> parsed = ParseEx(exp, v);

            for(int j=0; j<parsed.Count; j++)
            {
                if (double.TryParse(parsed[j], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out s)) stack.Push(parsed[j]);
                else
                {
                    if (operations.Count == 0) operations.Push(parsed[j]);
                    else
                    {
                        if (parsed[j] == "(") operations.Push(parsed[j]);
                        else if (parsed[j] == ")")
                        {
                            for (int h = operations.Count; h > 0; h--)
                            {
                                if (operations.Peek() == "(")
                                {
                                    operations.Pop();
                                    break;
                                }
                                else stack.Push(operations.Pop());
                            }
                        }
                        else if (Operations(operations.Peek()) >= Operations(parsed[j]))
                        {
                            stack.Push(operations.Pop());
                            operations.Push(parsed[j]);
                        }
                        else operations.Push(parsed[j]);
                    }
                }               
            }
            while (operations.Count != 0) stack.Push(operations.Pop());
            while (stack.Count != 0) res.Add(stack.Pop());
            res.Reverse();
            return res;
        }

        static string Count(string exp, string v)
        {
            double s = 0;
            List<string> cnt= RPN(exp, v);
            Stack<string> stack = new Stack<string>();
            for(int i=0; i<cnt.Count; i++)
            {
                if (double.TryParse(cnt[i], NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out s)) stack.Push(cnt[i]);
                else if(Operations(cnt[i]) != 0 && Operations(cnt[i]) != 6 && BinaryFunctions(cnt[i]) != 2)
                {
                    string x1 = stack.Pop(), x2 = stack.Pop();
                    stack.Push(Counting(double.Parse(x2, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")), double.Parse(x1, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")), cnt[i]).ToString());
                    if (stack.Peek().Contains(",")) stack.Push(stack.Pop().Replace(",", "."));
                }
                else
                {
                    string x = stack.Pop();
                    stack.Push(Convert(double.Parse(x, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US")), cnt[i]).ToString());
                    if (stack.Peek().Contains(",")) stack.Push(stack.Pop().Replace(",", "."));
                }
            }
            return stack.Pop();
        }


        static void Main(string[] args)
        {
            Console.WriteLine("введите переменные, если их нет - нажмите enter");
            string v = Console.ReadLine();
            Console.WriteLine("введите выражение");
            string exp = Console.ReadLine();
            Console.WriteLine(Count(exp,v));
            Console.ReadKey();
        }

    }
}
