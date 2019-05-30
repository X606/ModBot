# Accessor

The accessor class can be used to interact with private or protected members of a class.

Read the source code [here](https://github.com/X606/ModBot/blob/master/Mod%20Bot/Accessor.cs)

# Methods

## CallPrivateMethod (Non-static)[<sup>[1]</sup>](#non-static-footnote)

Requires that an instance of the Accessor class is defined in order to be called.

Calls a private method from a class.  

CallPrivateMethod (Non-static) takes 1-2 arguments (2nd is null by default):  
The first argument is a string, this is the name if the private or protected method you want to call, note that it is case-sensitive.  
The second argument is an object array, this is the arguments that will be passed on to the method, if null, no arguments will be passed.  

Note that since this method is of type void, you can not currently get the return value of a private method.

## GetAllMethods (Non-static)[<sup>[1]</sup>](#non-static-footnote)

Requires that an instance of the Accessor class is defined in order to be called.

GetAllMethods (Non-static) takes no arguments and returns a MethodInfo array of all the methods in the specified type.

## SetPrivateField (Non-static)[<sup>[1]</sup>](#non-static-footnote)

Requires that an instance of the Accessor class is defined in order to be called.

Sets a private variable in a class to a specified value.  

SetPrivateField (Non-static) takes 2 arguments:  
The first argument is the name of the field (case sensitive).  
The second argument is the value to set the field to.

## GetPrivateField (Non-static)[<sup>[1]</sup>](#non-static-footnote)

Requires that an instance of the Accessor class is defined in order to be called.

Gets the value of a specified private field in a class.  

GetPrivateField (Non-static) takes a string, which is the name of the field to get the value of.  

Returns: The value of the specified field.

## SetPrivateProperty (Non-static)[<sup>[1]</sup>](#non-static-footnote)

Requires that an instance of the Accessor class is defined in order to be called.

Sets a private property of a class.  

SetPrivateProperty (Non-static) takes 2 arguments:  
The first argument is the name if the property (case-sensitive).  
The second argument is the new value to set it to.

[1] Non static members of a class need to have an instance to run on, whereas static members don't. [Read more](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/static-classes-and-static-class-members)