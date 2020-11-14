#region Collapse for adding Commands
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;
using System;
#endregion
/*                 ASCII to hex conversion chart
   | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | A | B | C | D | E | F |
   |___|___|___|___|___|___|___|___|___|___|___|___|___|___|___|___|
 0 |NUL|SOH|STX|ETX|EOT|ENQ|ACK|BEL|BS | HT|LF | VT|FF | CR|SO | SI|
 1 |DLE|DC1|DC2|DC3|DC4|NAK|SYN|ETB|CAN| EM|SUB|ESC|FS | GS|RS | US|
 2 |   | ! | " | # | $ | % | & | ' | ( | ) | * | + | , | - | . | / |
 3 | 0 | 1 | 2 | 3 | 4 | 5 | 6 | 7 | 8 | 9 | : | ; | < | = | > | ? |
 4 | @ | A | B | C | D | E | F | G | H | I | J | K | L | M | N | O |
 5 | P | Q | R | S | T | U | V | W | X | Y | Z | [ | \ | ] | ^ | _ |
 6 | ` | a | b | c | d | e | f | g | h | i | j | k | l | m | n | o |
 7 | p | q | r | s | t | u | v | w | x | y | z | { | | | } | ~ |DEL|
*/
#region Collapse for adding commands
#region fishBytes Native Objects

/// <summary>
/// A native fishBytes object.
/// </summary>
public struct fishBytesNativeObject
{
    Dictionary<object, object> properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:fishBytesNativeObject"/> struct.
    /// </summary>
    /// <param name="properties">The fields to use. (Optional)</param>
    public fishBytesNativeObject(Dictionary<object, object> properties = null)
    {
        if (properties != null) this.properties = properties;
        else this.properties = new Dictionary<object, object>();

        if (!properties.ContainsKey("__typename__"))
        {
            properties.Add("__typename__", "AnonymousObject");
        }
    }

    /// <summary>
    /// Construct an object from the specified template object.
    /// </summary>
    /// <returns>The object.</returns>
    /// <param name="template">A template opject to use.</param>
    public static fishBytesNativeObject Construct(fishBytesNativeObject template)
    {
        return new fishBytesNativeObject(template.properties);
    }

    /// <summary>
    /// Get a field with the specified name.
    /// </summary>
    /// <returns>The contents of the field.</returns>
    /// <param name="name">The name of the field.</param>
    public object Get(object name)
    {
        if (properties.ContainsKey(name)) return properties[name];
        throw new Exception("The field " + name + " did not exist. Use debug to find a list of fields.");
    }

    /// <summary>
    /// Set or modify a property of the object.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="contents">The value to set the property to.</param>
    public void Set(object name, object contents)
    {
        try { properties[name] = contents; }
        catch { properties.Add(name, contents); }
    }

    /// <summary>
    /// Set a property of the object to null.
    /// </summary>
    /// <param name="name">The name of the property to set.</param>
    public void Set(object name) { Set(name, null); }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    /// <returns>A <see cref="T:System.String"/> that represents the current <see cref="T:fishBytesNativeObject"/>.</returns>
    public override string ToString()
    {
        string output = "";
        foreach (var key in properties.Keys)
        {
            if (properties[key].GetType() == "".GetType() || properties[key].GetType() == '\0'.GetType())
                output += $"\"{key.ToString()}\": \"{properties[key].ToString()}\",\n";
            else output += $"\"{key.ToString()}\": {properties[key].ToString()},\n";
        }
        return "{\n"+output+"}";
    }

    /// <summary>
    /// Converts a string representation of an object to the actual object.
    /// </summary>
    /// <param name="serialised">The string representation of the object.</param>
    /// <returns>A native object converted from the string representation.</returns>
    public static fishBytesNativeObject FromString(string serialised)
    {
        fishBytesNativeObject obj = new fishBytesNativeObject();
        var serialisedArr = serialised.ToCharArray().ToList();
        serialisedArr.RemoveAt(0);
        serialisedArr.RemoveAt(serialisedArr.Count - 1);
        serialised = new string(serialisedArr.ToArray());
        uint objlvl = 0;
        string eobj = "";
        string name = "";
        string Obj = "";
        bool isName = true;
        foreach (char c in serialised)
        {
            if (objlvl == 1)
            {
                eobj += c;
            }
            else if (c == '{')
            {
                objlvl = 1;
                eobj = "";
            }
            else if (c == '}')
            {
                objlvl = 0;
                obj.Set(GetStructFromString(name), FromString(eobj));
            }
            else if (objlvl == 0)
            {
                if (isName)
                {
                    if (c == ':')
                    {
                        Obj = "";
                    }
                    else name += c;
                }
                else
                {
                    if (c == ',')
                    {
                        name = "";
                        obj.Set(GetStructFromString(name), GetStructFromString(Obj));
                    }
                    else Obj += c;
                }
            }
            else
            {
                throw new Exception("Something is wrong with our code. We will try and" +
                    $" solve this as soon as possible. Error code: OBJLVL_EQ_{objlvl}");
            }
        }
        return obj;
    }

    static string ToSafeString(string str)
    {
        return str.Replace("\\", @"\\")
        .Replace("\n", @"\n")
        .Replace("\t", @"\t")
        .Replace("\r", @"\r")
        .Replace("\"", "\\\"")
        .Replace("\a", @"\a")
        .Replace("'", @"\'")
        .Replace("\b", @"\b")
        .Replace("\f", @"\f")
        .Replace("\v", @"\v");
    }

    static object GetStructFromString (string str)
    {
        if (float.TryParse(str, out float Float))
            return Float;
        if (long.TryParse(str, out long Long))
            return Long;
        if (double.TryParse(str, out double Double))
            return Double;
        if (ulong.TryParse(str, out ulong ULong))
            return ULong;
        if (str == "true")
            return true;
        if (str == "false")
            return false;
        if (str == "null")
            return null;
        if (str.StartsWith("\"") && str.EndsWith("\""))
        {
            var ltmp = str.ToCharArray().ToList();
            ltmp.RemoveAt(0);
            ltmp.RemoveAt(ltmp.Count - 1);
            return new string(ltmp.ToArray());
        }
        return str;
    }
}
#endregion

///<summary>
/// This class manages the interpretation and execution of programs
///</summary>
public class fishBytesInterpreter
{
    /// <summary>
    /// The location of the interpreter.
    /// </summary>
    public uint location;
    List<object> stack = new List<object>();
    List<uint> protectedStack = new List<uint>();
    Dictionary<int, object> variables;
    fishBytesNativeObject thisFunc = new fishBytesNativeObject();
    Func<fishBytesNativeObject> getLocalObject;
    int commentLevel;

    public fishBytesInterpreter()
    {
#if FISHBYTES_FRAMEWORK_DEFINED // Check for a fishBytes framework
        variables = fishBytesFramework.GetNamespaces();
#else
        variables = new Dictionary<int, object>();
#endif
    }


    /// <summary>
    /// Pop an object at a specified position on the stack.
    /// </summary>
    /// <returns>The item popped.</returns>
    /// <param name="place">The index to pop it from.</param>
    object Pop(int place)
    {
        try
        {
            var x = stack[place];
            stack.RemoveAt(place);
            return x;
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Logging debugging information:\n");
            var C = 0;
            if (stack.Count == 0)
            {
                Console.WriteLine("The stack is empty.");
            }
            foreach (var item in stack)
            {
                Console.WriteLine($"Item {C.ToString().PadLeft(2)}: {item.ToString()}");
                C++; // No, C#
            }
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            throw new ArgumentOutOfRangeException(e.ToString());
            //return 0;
        }
    }

    /// <summary>
    /// Pop the top item of the stack
    /// </summary>
    /// <returns>The item popped.</returns>
    object Pop()
    {
        return Pop(0);
    }

    /// <summary>
    /// Push the specified value onto the stack.
    /// </summary>
    /// <param name="value">The value to push.</param>
    void Push(object value)
    {
        stack.Insert(0, value);
    }

    /// <summary>
    /// Execute the specified program.
    /// </summary>
    /// <returns>The exit code.</returns>
    /// <param name="opcodes">The program to execute.</param>
    public int Execute(byte[] opcodes)
    {
        stack.Clear();
        getLocalObject = () => thisFunc;

        while (location < opcodes.Length)
        {
            if (opcodes[location] == 0x7B) // Open Comment
                commentLevel += 1;
            else if (opcodes[location] == 0x7D)
                commentLevel -= 1;
            else if (commentLevel == 0)
            {
                switch (opcodes[location])
                {
#endregion
#region Already added commands
                    case 0x61: // ADD
                        if (stack.Count == 0)
                        {
                            Push(null);
                        }
                        else if (stack.Count == 1)
                        {
                            Push((Convert.ToSingle(Pop())) + 1);
                        }
                        else
                        {
                            var b = (Pop());
                            var a = (Pop());

                            if (a.GetType() == ' '.GetType())
                            {
                                Push(Convert.ToString(a) + Convert.ToString(b));
                            }
                            else if (a.GetType() == "".GetType())
                            {
                                Push(Convert.ToString(a) + Convert.ToString(b));
                            }
                            else
                            {
                                Push(Convert.ToSingle(a) + Convert.ToSingle(b));
                            }
                        }
                        break;

                    case 0x63: // BYTE
                        Push((byte)0);
                        break;

                    case 0x64: // DEBUG
                        Console.WriteLine("Logging debugging information:\n");
                        var C = 0;
                        if (stack.Count == 0)
                        {
                            Console.WriteLine("The stack is empty.");
                        }
                        foreach (var item in stack)
                        {
                            if (item == null)
                                Console.Error.WriteLine($"Item {C.ToString().PadLeft(2)}: <NULL>");
                            else
                                Console.WriteLine($"Item {C.ToString().PadLeft(2)}: {item.ToString()}");
                            C++; // No, C#
                        }
                        break;

                    case 0x65: // INPUT
                        Push(Console.ReadKey().KeyChar);
                        break;

                    case 0x67: // LOAD
                        int place = (int)Pop();
                        Push(variables[place]);
                        break;

                    case 0x69: // UNSIGNED INTEGER
                        Push((uint)0);
                        break;

                    case 0x6C: // LINE
                        Push(Console.ReadLine());
                        break;

                    case 0x73: // SUBTRACT
                        var e = Convert.ToSingle(Pop());
                        var d = Convert.ToSingle(Pop());
                        Push(d - e);
                        break;

                    case 0x41: // ARGS
                        for (int i = Program.Args.Length - 1; i >= 0; i--)
                        {
                            Push(Program.Args[i]);
                        }
                        break;

                    case 0x42: // BOOLEAN
                        Push(false);
                        break;

                    case 0x43: // SIGNED BYTE
                        Push((sbyte)0);
                        break;

                    case 0x44: // DIVIDE
                        var g = Convert.ToSingle(Pop());
                        var f = Convert.ToSingle(Pop());
                        Push(f / g);
                        break;

                    case 0x45: // ECHO
                        Console.WriteLine(Pop()?.ToString());
                        break;

                    case 0x46: // FLOAT
                        Push(0f);
                        break;

                    case 0x47: // GOTO
                        location = (uint)Pop();
                        goto next;

                    case 0x49: // INTEGER
                        Push(0);
                        break;

                    case 0x4C: // DOUBLE (LONG FLOAT)
                        Push((double)0f);
                        break;

                    case 0x4D: // MULTIPLY
                        var j = Convert.ToSingle(Pop());
                        var h = Convert.ToSingle(Pop());
                        Push(h * j);
                        break;

                    case 0x74: // CAST
                        var castType = Pop();
                        var ThingToCast = Pop();

                        if (castType.GetType() == ' '.GetType() && ThingToCast.GetType() == 0f.GetType())
                        {
                            Push(Convert.ToChar(Convert.ToUInt16(ThingToCast)));
                        }

                        else if (castType.GetType() == 0f.GetType())
                            Push(Convert.ToSingle(ThingToCast));

                        else if (castType.GetType() == new double().GetType())
                            Push(Convert.ToDouble(ThingToCast));

                        else if (castType.GetType() == new decimal().GetType())
                            Push(Convert.ToDecimal(ThingToCast));

                        else if (castType.GetType() == 0.GetType())
                            Push(Convert.ToInt32(ThingToCast));

                        else if (castType.GetType() == new short().GetType())
                            Push(Convert.ToInt16(ThingToCast));

                        else if (castType.GetType() == new long().GetType())
                            Push(Convert.ToInt64(ThingToCast));

                        else if (castType.GetType() == "".GetType())
                            Push(ThingToCast.ToString());

                        else if (castType.GetType() == '\0'.GetType())
                            Push(Convert.ToChar(ThingToCast));

                        else if (castType.GetType() == new uint().GetType())
                            Push(Convert.ToUInt32(ThingToCast));
                        else if (castType.GetType() == new fishBytesNativeObject().GetType())
                        {
                            if (ThingToCast.GetType() == "".GetType()) Push(fishBytesNativeObject.FromString((string)ThingToCast));
                        }
                        else
                        {
                            Push(ThingToCast);
                            Console.WriteLine($"We couldn't cast type {ThingToCast.GetType().ToString()} to type {castType.GetType().ToString()}.");
                        }
                        break;

                    case 0x68: // HELP
                        Console.WriteLine(Program.help);
                        break;

                    case 0x3D: // EQUALS
#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless, but not in this scenario
                        if (Pop() == Pop())
                        {
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless, but not in this scenario
                            Push(true);
                        }
                        else
                        {
                            Push(false);
                        }
                        break;

                    case 0x3F: // IF
                        var l = Pop();
                        var c = Pop();
                        if (c.Equals(0) || c.Equals(false))
                            break;
                        location = (uint)l;
                        protectedStack.Insert(0, location);
                        goto next;

                    case 0x22: // STRING
                        Push("");
                        break;

                    case 0x2A: // DUPLICATE
                        object t = Pop();
                        Push(t);
                        Push(t);
                        break;

                    case 0x21: // NOT
                        var p = Pop()
                        ; if (p.Equals(false) || p.Equals(0))
                        {
                            Push(true)
                       ;
                        }
                        else
                        {
                            Push(false)
                       ;
                        }
                        ; break

                     ;
                    case 0x30: // 0
                        Push(Convert.ToSingle(Pop()) * 10);
                        break;

                    case 0x31: // 1
                        Push(Convert.ToSingle(Pop()) * 10 + 1);
                        break;

                    case 0x32: // 2
                        Push(Convert.ToSingle(Pop()) * 10 + 2);
                        break;

                    case 0x33: // 3
                        Push(Convert.ToSingle(Pop()) * 10 + 3);
                        break;

                    case 0x34: // 4
                        Push(Convert.ToSingle(Pop()) * 10 + 4);
                        break;

                    case 0x35: // 5
                        Push(Convert.ToSingle(Pop()) * 10 + 5);
                        break;

                    case 0x36: // 6
                        Push(Convert.ToSingle(Pop()) * 10 + 6);
                        break;

                    case 0x37: // 7
                        Push(Convert.ToSingle(Pop()) * 10 + 7);
                        break;

                    case 0x38: // 8
                        Push(Convert.ToSingle(Pop()) * 10 + 8);
                        break;

                    case 0x39: // 9
                        Push(Convert.ToSingle(Pop()) * 10 + 9);
                        break;

                    case 0x27: // CHARACTER
                        Push('\0');
                        break;

                    case 0x4E: // NULL
                        Push(null);
                        break;

                    case 0x50: // POP
                        Pop();
                        break;

                    case 0x51: // SAVE
                        Push(stack);
                        break;

                    case 0x52: // RESET
                        stack.Clear();
                        location = 0;
                        goto next;

                    case 0x53: // SQRT aka SQUARE ROOT
                        var num = Pop();
                        Push(Math.Sqrt(Convert.ToDouble(num)));
                        break;

                    case 0x56: // VARIABLE
                        var x = Pop();
                        var y = Pop();
                        try
                        {
                            variables[Convert.ToInt32(x)] = y;
                        }
                        catch
                        {
                            variables.Add(Convert.ToInt32(x), y);
                        }
                        break;

                    case 0x57: // WIPE
                        stack.Clear();
                        break;

                    case 0x58: // EXIT
                        if (stack.Count == 0)
                        {
                            location = (uint)opcodes.Length;
                        }
                        else
                        {
                            return (byte)Pop();
                        }
                        break;

                    case 0x26: // AND
                        var z = Pop();
                        var w = Pop();
                        if (z.Equals(0) || z.Equals(false))
                        {
                            Push(false);
                        }
                        else if (w.Equals(0) || w.Equals(false))
                        {
                            Push(false);
                        }
                        else
                        {
                            Push(true);
                        }
                        break;

                    case 0x7C:
                        var oof = Pop();
                        var foo = Pop();
                        if (!(oof.Equals(0) || oof.Equals(false)))
                        {
                            Push(true);
                        }
                        else if (!(foo.Equals(0) || foo.Equals(false)))
                        {
                            Push(true);
                        }
                        else
                        {
                            Push(false);
                        }
                        break;

                    case 0x3C: // LESS THAN (LT)
#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless, but not in this scenario.
                        if (Convert.ToSingle(Pop()) >= Convert.ToSingle(Pop()))
                        {
                            Push(true);
                        }
                        else
                        {
                            Push(false);
                        }
                        break;

                    case 0x3E: // GREATER THAN (GT)
                        if (Convert.ToSingle(Pop()) <= Convert.ToSingle(Pop()))
                        {
#pragma warning restore RECS0088 // Comparing equal expression for equality is usually useless, but not in this scenario.
                            Push(true);
                        }
                        else
                        {
                            Push(false);
                        }
                        break;

                    case 0x5F: // DELAY
                        System.Threading.Thread.Sleep(Convert.ToInt32(Convert.ToSingle(Pop()) * 1000));
                        break;

                    case 0x7E: // ENVIRONMENT VARIABLE
                        Push(Environment.GetEnvironmentVariable(Pop().ToString()));
                        break;

                    case 0x2B: // DO
                        protectedStack.Insert(0, location);
                        location = Convert.ToUInt32(Pop());
                        goto next;

                    case 0x2D: // END SUB
                        location = protectedStack[0] + 1; // We want to go to the
                        protectedStack.RemoveAt(0);       // character after so
                        goto next;                        // that we don't get
                                                          // stuck in an infinite
                                                          // loop.


                    case 0x24: // RUN // RUNCOM
                        var pr = new ProcessStartInfo(Pop().ToString())
                        {
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true
                        };
                        var pro = Process.Start(pr);
                        pro.ErrorDataReceived += (sender, err) => Console.Error.Write(err);
                        pro.OutputDataReceived += (sender, err) => Console.Write(err);
                        pro.BeginOutputReadLine();
                        pro.BeginErrorReadLine();
                        pro.WaitForExit();
                        Push(pro.ExitCode);
                        break;
#endregion

                    case 0x6D: // MAKE
                        Push(fishBytesNativeObject.Construct((fishBytesNativeObject)Pop()));
                        break;

                    case 0x66: // ARRAY_LOAD
                        foreach (var item in (List<object>)Pop())
                        {
                            stack.Add(item);
                        }
                        break;

                    case 0x3A: // SET_LOCAL
                        int loc = Convert.ToInt32(Pop());
                        getLocalObject = () => (fishBytesNativeObject)variables[loc];
                        break;

                    case 0x6E: // LOCAL_SET_LOCAL
                        int lc = Convert.ToInt32(Pop());
                        getLocalObject = () => (fishBytesNativeObject)variables[lc];
                        break;

                    case 0x76: // VAR_LOCAL
                        getLocalObject().Set(Pop(), Pop());
                        break;

                    case 0x62: // LOAD_LOCAL
                        Push(getLocalObject().Get(Pop()));
                        break;

                    case 0x6A: // GET_FROM_INDEX
                        Push(stack[Convert.ToInt32(Pop())]);
                        break;

                    case 0x4F: // OBJECT
                        Push(new fishBytesNativeObject());
                        break;

                    case 0x5D: // MOVE_TO_INDEX
                        stack.Insert((int)Pop(), Pop());
                        break;

                    case 0x5B: // POP_FROM_INDEX
                        Push(Pop(Convert.ToInt32(Pop())));
                        break;

                    case 0x6B: // SIZE
                        Push(stack.Count);
                        break;

                    case 0x2E: // PTRLOC
                        Push(location);
                        break;

                    // add more above this line
#region Collapse for adding commands
                    // Whitespace characters are ignored.
                    case 0x0A:
                    case 0x0D:
                    case 0X20: // NOPE
                        break;

                    default:
                        Console.WriteLine($"Invalid opcode: {Convert.ToChar(opcodes[location])} (0x{Convert.ToString(opcodes[location], 16)})");
                        break;
                }
            }
            location += 1;
        next:
            Console.Write("");
        }
        return 0;
    }
}


///<summary>
/// This class manages the command-line interface
///</summary>
public class Program
{
    /// <summary>
    /// The list of opcodes supported in this version.
    /// </summary>
#endregion
#region Help
    public static readonly string help = $@"
=====================================================================
=                         List of opcodes:                          =
=====================================================================

+------+--------------+----------------------+
| Byte | Command Name |     Description      |
+------+--------------+----------------------+
|  a   |      ADD     | If the stack has no  |
|      |              | items, push 0. If it |
|      |              | has 1, add 1 to that |
|      |              | number. Else, pop two|
|      |              | numbers and add their|
|      |              | sum.                 |
+------+--------------+----------------------+
|  b   |  LOAD_LOCAL  | Pop a value and push |
|      |              | the contents of a    |
|      |              | local variable with  |
|      |              | that name.           |
+------+--------------+----------------------+
|  c   |     BYTE     | Push an 8-bit        |
|      |              | unsigned integer (0) |
+------+--------------+----------------------+
|  d   |    DEBUG     | Print what's in the  |
|      |              | stack.               |
+------+--------------+----------------------+
|  e   |     INPUT    | Push a character from|
|      |              | stdin.               |
+------+--------------+----------------------+
|  f   |  ARRAY_LOAD  | Pop an array and push|
|      |              | all of it's items.   |
+------+--------------+----------------------+
|  g   |     LOAD     | Pop a value and get a|
|      |              | variable with that   |
|      |              | name.                |
+------+--------------+----------------------+
|  h   |     HELP     | Print out this info. |
+------+--------------+----------------------+
|  i   |     UINT     | Push a 32-bit        |
|      |              | unsigned integer (0) |
+------+--------------+----------------------+
|  j   |GET_FROM_INDEX| Pop an index and copy|
|      |              |the item at that index|
|      |              | on the stack to the  |
|      |              | top.                 |
+------+--------------+----------------------+
|  k   |     SIZE     | Push the size of the |
|      |              | stack.               |
+------+--------------+----------------------+
|  l   |     LINE     | Push a line of text  |
|      |              | read from stdout.    |
+------+--------------+----------------------+
|  m   |     MAKE     | Create an object of a|
|      |              | type popped from the |
|      |              | stack.               |
+------+--------------+----------------------+
|  n   |LOCAL_SETLOCAL| Point all VAR_LOCAL  |
|      |              | and LOAD_LOCAL calls |
|      |              | to another object    |
|      |              | stored in a local var|
|      |              | with a name popped   |
|      |              | from the stack.      |    
+------+--------------+----------------------+
|  s   |   SUBTRACT   | Pop two values, push |
|      |              | their difference.    |
+------+--------------+----------------------+
|  v   |   VAR_LOCAL  | Pop a, pop b, set a  |
|      |              | local variable named |
|      |              | a to the value of b. |
+------+--------------+----------------------+
|  A   |     ARGS     | Push each argument   |
|      |              | onto the stack.      |
+------+--------------+----------------------+
|  B   |     BOOL     | Push false.          |
+------+--------------+----------------------+
|  C   |     SBYTE    | Push a signed 8-bit  |
|      |              | integer (0)          |
+------+--------------+----------------------+
|  D   |     DIVIDE   | Pop b then a, push   |
|      |              | a / b.               |
+------+--------------+----------------------+
|  E   |     ECHO     | Pop a value then log |
|      |              | it to stdout.        |
+------+--------------+----------------------+
|  F   |     FLOAT    | Push 0.0             |
+------+--------------+----------------------+
|  G   |     GOTO     | Pop n, then point the|
|      |              | interpreter to that  |
|      |              | location.            |
+------+--------------+----------------------+
|  I   |    INTEGER   | Push 0.              |
+------+--------------+----------------------+
|  L   |     DOUBLE   | Push a double-length |
|      |              | float (0)            |
+------+--------------+----------------------+
|  M   |    MULTIPLY  | Pop two values and   |
|      |              | push their product.  |
+------+--------------+----------------------+
|  N   |     NULL     | Push null.           |
+------+--------------+----------------------+
|  O   |    OBJECT    | Push an empty object |
+------+--------------+----------------------+
|  P   |     POP      | Pop a value from the |
|      |              | stack.               |
+------+--------------+----------------------+
|  Q   |     SAVE     | Push an array of the |
|      |              | items on the stack.  |
+------+--------------+----------------------+
|  R   |     RESET    | Clear the stack and  |
|      |              | goto 0.              |
+------+--------------+----------------------+
|  S   |     SQRT     | Pop a value, push its|
|      |              | square root.         |
+------+--------------+----------------------+
|  V   |    VARIABLE  | Pop a, pop b, set a  |
|      |              | variable called a to |
|      |              | the value of b.      |
+------+--------------+----------------------+
|  W   |     WIPE     | Clear the stack.     |
+------+--------------+----------------------+
|  X   |     EXIT     | Pop an exit code and |
|      |              | quit.                |
+------+--------------+----------------------+
|  !   |     NOT      | Logical NOT.         |
+------+--------------+----------------------+
|  %   |    MODULO    | Pop b, pop a, push   |
|      |              | the remainder of a/b |
+------+--------------+----------------------+
|  ^   |   EXPONENT   | Pop b, pop a, push a |
|      |              | to the power of b.   |
+------+--------------+----------------------+
|  &   |     AND      | Logical AND operator |
+------+--------------+----------------------+
|  *   |   DUPLICATE  | Duplicate the top    |
|      |              | item of the stack.   |
+------+--------------+----------------------+
| 0..9 |       n      | Multiply the top item|
|      |              | of the stack by 10   |
|      |              | and add n.           |
+------+--------------+----------------------+
|  |   |      OR      | Logical OR operator  |
+------+--------------+----------------------+
|  ?   |      IF      | Pop l, pop c, if c is|
|      |              | true then goto l.    |
+------+--------------+----------------------+
|  <   |      LT      | Pop b, pop a, if a is|
|      |              | less than  b then    |
|      |              | push true, otherwise |
|      |              | push false.          |
+------+--------------+----------------------+
|  >   |      GT      | Same as COPY COPY GT |
|      |              | NOT EQUALS NOT AND   |
+------+--------------+----------------------+
|  '   |     CHAR     |Push the NUL character|
+------+--------------+----------------------+
|  {'"'}   |    STRING    | Push an empty string |
+------+--------------+----------------------+
|  _   |     DELAY    | Pop a number and wait|
|      |              | that many secands.   |
+------+--------------+----------------------+
|  =   |    EQUALS    | Pop two values, if   |
|      |              | they are equal, push |
|      |              | 1 else push 0.       |
+------+--------------+----------------------+
|  ~   |      ENV     | Pop a string, push an|
|      |              | environment variable |
|      |              | with that name.      |
+------+--------------+----------------------+
|  +   |      DO      | Pop a location from  |
|      |              | the stack and execute|
|      |              | a submodule there.   |
+------+--------------+----------------------+
|  -   |      END     | End a control block  |
|      |              | (ie. function, loop, |
|      |              | submodule)           |
+------+--------------+----------------------+
|  $   |      RUN     | Pop a command and run|
|      |              |it in the system shell|
+------+--------------+----------------------+
|  :   |   SET_LOCAL  | Point all VAR_LOCAL  |
|      |              | and LOAD_LOCAL calls |
|      |              | to another object    |
|      |              | stored in a variable |
|      |              | with a name popped   |
|      |              | from the stack.      |
+------+--------------+----------------------+
|  [   |POP_FROM_INDEX| Move the item at a   |
|      |              | popped index to the  |
|      |              | top of the stack.    |
+------+--------------+----------------------+
|  ]   |MOVE_TO_INDEX | Move the item at the |
|      |              | top of the stack to a|
|      |              | popped location.     |
+------+--------------+----------------------+
|  .   |    PTRLOC    | Push the location of |
|      |              | the interpreter.     |
+------+--------------+----------------------+
";
#endregion
/*
|      |              |                      |
+------+--------------+----------------------+
|      |              |                      |*/
#region Collapse for adding commands
    /// <summary>
    /// The entry point of the program, where the program control starts and ends.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Interactive console started. Enter some code and press ENTER to run it. CONTROL+C terminates the program. Enter 'h' to get help.");
            //Console.WriteLine(help);
            //new fishBytesInterpreter().Execute(new byte[] { 0x46, 0x46, 0x61, 0x61, 0x61, 0x61, 0x45 });
            while (true)
            {
                Console.Write("Enter code here: ");
                byte[] bytes = Encoding.ASCII.GetBytes(Console.ReadLine());
                var sword = new fishBytesInterpreter();
                try
                {
                    var ec = sword.Execute(bytes);
                    if (ec != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Process ended with error code " + ec.ToString());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($".NET Exception Encountered at byte {sword.location} ({Convert.ToChar(bytes[sword.location])})");
                    Console.WriteLine($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine(e);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($".NET Exception Encountered at byte {sword.location} ({Convert.ToChar(bytes[sword.location])})");
                    Console.WriteLine($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine(e);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                default:
                    var path = args[i];
                    if (!File.Exists(path))
                    {
                        if (File.Exists(Environment.CurrentDirectory + "/" + path))
                        {
                            path = Environment.CurrentDirectory + "/" + path;
                        }
                        else if (File.Exists(GetLibFolder() + "/" + path))
                        {
                            path = GetLibFolder() + "/" + path;
                        }
                        else if (File.Exists(GetLibFolder() + "/" + path + ".fbi"))
                        {
                            path = GetLibFolder() + "/" + path + ".fbi";
                        }
                        else
                        {
                            throw new FileNotFoundException(path + ": file not found");
                        }
                    }
                    var sword = new fishBytesInterpreter();
                    try
                    {
                        var ec = sword.Execute(File.ReadAllBytes(path));
                        if (!(ec == 0))
                        {
                            Environment.Exit(ec);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($".NET Exception Encountered at byte {sword.location} ({Convert.ToChar(File.ReadAllBytes(path)[sword.location])})");
                        Console.WriteLine($"~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine(e.Message);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Get the library folder path for the system.
    /// </summary>
    /// <returns>The library folder path.</returns>
    private static string GetLibFolder()
    {
        string windir = Environment.GetEnvironmentVariable("windir");
        if (!string.IsNullOrEmpty(windir) && windir.Contains(@"\") && Directory.Exists(windir))
        {
            // Doors and Widnows
            return Environment.GetEnvironmentVariable("localappdata") + @"\fbi\lib";
        }
        if (File.Exists(@"/proc/sys/kernel/ostype"))
        {
            string osType = File.ReadAllText(@"/proc/sys/kernel/ostype");
            if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
            {
                // Lliiinnnuuuuxxxxx bbbboooooiiiiiii
                return Environment.GetEnvironmentVariable("HOME") + "/.local/share/fbi/lib";
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Warning: Your OS is not fully supported. As a result, some programs may not function properly.");
            Console.ForegroundColor = ConsoleColor.White;
            return Environment.CurrentDirectory;
        }
        if (File.Exists(@"/System/Library/CoreServices/SystemVersion.plist"))
        {
            // Makakakakakakakakakakakakakakakintosh
            return Environment.GetEnvironmentVariable("HOME") + "/Library/Application Support/fbi/lib";
        }
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("Warning: Your OS is not fully supported. As a result, some programs may not function properly.");
        Console.ForegroundColor = ConsoleColor.White;
        return Environment.CurrentDirectory;
    }

    /// <summary>
    /// The program's arguments.
    /// </summary>
    public static string[] Args;
}

///<summary>
/// This class creates and opens fishBytes archives.
///</summary>
public class FarFileManager
{

}
#endregion
