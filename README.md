fishBytes
=========

fishBytes is a stack-based bytecode programming language that I'm creating as a intermediate language for [fishScript](/Kettle3D/fishScript#fishScript) to compile to. I'm currently adding object-oriented features to it.

How to get it
-------------

fishBytes is in beta development. To use it how it is, get the .NET Core SDK (3.1) at <https://dot.net/download>, open Command Prompt (or Terminal) in this folder and run these commands on macOS, Linux and other UNIXes:
```
dotnet build
cd bin/Debug/netcoreapp3.1
sudo ln -sv "$PWD/fbi" /usr/bin/fbi # To add it to your path
```
and on Windows, just run `dotnet build`.  
I will provide binaries when they are ready (I think they're stable enough for people to use them).

How to use it
-------------

fishBytes can read its programs either from a file or the console. Open a Terminal or Command Prompt, and if you're on Windows, go `set PATH=%PATH%;<where-you-downloaded-fishBytes>\bin\Debug\netcoreapp3.1`

Then type in `fbi` and press <kbd>ENTER</kbd>. You'll see something like this:
```
Interactive console started. Enter some code and press ENTER to run it. CONTROL+C terminates the program. Enter 'h' to get help.
Enter code here: 
```
Now you can write some code. Start with something like this:
```
I5E
```
That should print out the number 5 and exit.

### What that did
fishBytes is made up of two main things: the *stack* and *commands*. It stores a list of objects (things like numbers, letters, etc.) that the program is using. Adding something to the top of the stack is known as *pushing*, while taking something off the top of the stack is known as *popping*. Commands are one-letter instructions telling fishBytes what to do.

What this program does is push an integer (a number that isn't decimal) onto the stack, which was empty. This is what the `I` command does. Then it multiplies that number by ten (0 * 0 = 0) and adds 5 (that's what the `5` command does). And after that it takes the item at the top of the stack and prints it to the terminal (that's the `E` command doing it's thing).

### There are more than three commands
A programming language that only lets you print 5 and 0 isn't very useful. There is a list of more commands here:
```
+------+--------------+----------------------+
| Char | Command Name |     Description      |
+------+--------------+----------------------+
|  a   |      ADD     | If the stack has no  |
|      |              | items, push 0. If it |
|      |              | has 1, add 1 to that |
|      |              | number. Else, pop two|
|      |              | numbers and add their|
|      |              | sum.                 |
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
|  i   |     UINT     | Push a 32-bit        |
|      |              | unsigned integer (0) |
+------+--------------+----------------------+
|  l   |     LINE     | Push a line of text  |
|      |              | read from stdout.    |
+------+--------------+----------------------+
|  m   |     MAKE     | Create an object of a|
|      |              | type popped from the |
|      |              | stack.               |
+------+--------------+----------------------+
|  s   |   SUBTRACT   | Pop two values, push |
|      |              | their difference.    |
+------+--------------+----------------------+
|  w   |     WHILE    | Will be added soon.  |
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
|  "   |    STRING    | Push an empty string |
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
```
And I'm adding even more...
