
**Introduction**

Simple Symbolic Math System (or SSMS) is just that. A self contained, simple system for doing symbolic math computations in the vein of Mathematica and Sage.

**Motivation**
There are quite a number of symbolic math packages, including Sage, Mathematica, and YACAS. Why not use them?

It's cool. When I was in college, I came across SMP (a predecessor to Mathematica). At the time I was blown away that computers could do such a thing. It was a while ago.

The biggest reason is that I want to be able to control the way expression simplification and expansion work. Sometimes you can see how to make an expression more readable, but a math package will not follow simplify in the way you want.

The second reason is to be able to handle some unusual syntax like wedge products and differential forms.

**Simplifying Assumptions**

 - All expressions are over the reals. No arbitrary fields.
 - The reals are represented by double precision floats. No arbitrary precision number support.
 - No expression parser. Expression trees are built up by hand in code.
 - Minimal performance optimization.
 - No use of LISP or any external AI system.

**Choice of Language**

The system is written in C#. Other choices might be Python, C## or even Javascript, but C# has good performance and an excellent development environment. Also, the system will have a deep tree hierarchy which should be easier construct correctly with strong typing.
 
**Basic Design**

An expression is composed of **SymNode**'s, which are what you would expect: summation, products, power, trig functions and so forth.

The tree of nodes is operated on by **Transform**'s, which are functions that replace some nodes with others.

**Division**

I decided not to have a separate division node type. Instead, I use a negative power node.

**Parsing**

Originally, I was not going to support parsing of string expressions. The idea was that you would create the expression tree by hand. This would save the complexity of a tokenizer & parser and make the system simpler.

Creating the node tree by hand is not too bad for a small number of expressions, but I eventually realized that I need a large number of expressions to get enough coverage with the unit tests. So, to make unit testing easier I added a simple parser.

The parser is handwritten, and does not use LEX/YACC, ANTLR or whatever. It's the shunting yard algorithm with some additions for unary operators and so forth.

The tokenizer doesn't support floating point numbers. Variable names have to consist entirely of letters.

Error handling is basic: Throw an ApplicationException with a brief description, but in theory the parser should not crash.

**Notation**

Differentials are represented by putting d_ in front of the variable name. For example d_x. The parser understand this syntax.

Wedge products presented a challenge: Normally, you would use the caret ^, but this is for exponentiation. I could potentially use some non-ASCII symbol but that would be hard to enter and look weird in console output. 

I settled on the combination of slash and backslash /\. This looks like a wedge, and provides some nice spacing. The biggest problem is the backslash is an escape character, so you need to escape it, or make string in C# literal by prefixing with @. The parser supports the /\ syntax.

**Simplification**

One of the most basic operations is simplification. For example $-a + a^1 = 0$. There are a whole lot of operations:

constant folding
merging of nodes (e.g. (a+(b+c)) becomes a (a+b+c)
expansion

This basic idea is to keep applying these in order as long as some sort of change is affected.

Some of the transforms act recursively (e.g. FoldConstants), but some do not. Transforms are marked depending on how they need to be applied, and the non-recursive transforms are applied by a separate DFO iterator.

**Sorting**

Ordering expressions to look nice turned out the be more complicated than I expected.

Nodes should be sorted based on the first variable in the expression: $a*b*c^2$ is sorted based on $a.$

Functions sort based on the function's name, $\cos(a) *\sin(b)$, and sort below all variable names.
We group expressions by differential, but the differential goes on the end of the expression. Thus, we have
x * d_x + a * d_y.

For each node, take the 1st child until you get a variable, or a function.

**To do**

 - A TexForm output to go with ToString. 
 - Make all the transforms recursive or not. 
 - Make use of C# iterators 
 - Add a natural log (ln)  node. 
	 - This would also allow differentiation of non-constant exponent
   functions.

