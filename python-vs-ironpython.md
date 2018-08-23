---
description: 'SOURCE: https://github.com/gtalarico/au2017'
---

# Python vs IronPython

## **What is Python?**

Python is a high-level, general-purpose, open-source programming language. The language is widely used in a variety of fields, including mathematics, data-science, and web-development.  


Python is also an interpreted language, meaning,  its code is interpreted without requiring compilation. The program that interprets and executes the code is called an interpreter. Interpreters can be executed independently, or embedded within other applications.  


The most common Python interpreter implementation it’s CPython, which is written in the language “C” and it’s considered to be the main-stream or reference Python implementation.  
****

## **What is IronPython?**

IronPython is an implementation of Python created by Microsoft and it’s written in C\#. The C\# implementation allows the interpreted python code to interact directly with other applications that are part of the Microsoft .NET framework \(C\#, F\#, VB, etc\) through the Common Language Runtime \(clr module\).  


This language interoperability has made IronPython a popular language for creating embedded scripting environments within Windows applications that use the .NET framework. Some applications that offer embedded python scripting environments are Rhino + Grasshopper, Autodesk Maya, and of course, Dynamo.  


While Python code can be written to be compatible with both mainstream Python and IronPython, this is only true when the code doesn’t take advantage of implementation-specific features. For example, code that requires .NET interoperability requires the ironpython-exclusive clr module. Conversely, python code that depends on libraries that rely on native C code cannot run in IronPython \(Numpy, Pandas, etc\).  
****

## **Why Learn Python**

Recognized as having a shallow learning curve, Python has become a popular choice for education environments and beginners. The syntax and language-constructs are simple and promote code-readability - when well written; its code is often considered easy to read even by non-programmers.  


Python’s language design also makes it an excellent language to create short macro-like scripts and automation logic due to its conciseness and comprehensive standard library \(modules that are built-in and shipped with the language\).  
Lastly, adding a language like Python to your toolset enables you to develop tools and workflows that are often faster than pure visual-programming, but also allow you to overcome limitations where the programs fall short.

