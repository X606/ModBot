using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

public static class Injector
{
    public static Injection AddCallToMethodInMethod(string path, string className, string methodName, string readPath, string readClassName, string readMethod, int instructionInsertOffset = 0, bool insertAfter = false, bool relativeToEnd = false, string[] argumentTypes = null, bool isWriteMethodGeneric = false)
    {
        HalfInjected halfInjected = InjectionFirstStep(path, className, methodName, "", isWriteMethodGeneric);
        HalfInjected readModule = InjectionFirstStep(readPath, readClassName, readMethod);
        
        if (halfInjected == null)
        {
            Console.WriteLine("Method not found, skipping");
            readModule.Module.Dispose();
            return new Injection();
        }

        if (argumentTypes != null)
        {
            List<MethodDefinition> methods = halfInjected.Module.GetType(className).Methods.Where(a => a.Name == methodName).ToList();
            MethodDefinition method = null;
            foreach(MethodDefinition _method in methods)
            {
                List<ParameterDefinition> parameters = _method.Parameters.ToList();
                if (parameters.Count != argumentTypes.Length)
                    continue;

                bool trueForAll = true;
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (parameters[i].ParameterType.Name != argumentTypes[i])
                    {
                        trueForAll = false;
                        break;
                    }
                }
                if (trueForAll)
                {
                    method = _method;
                    break;
                }

            }

            if (method != null)
            {
                halfInjected.TargetMethod = method;
                halfInjected.IlProcessor = halfInjected.TargetMethod.Body.GetILProcessor();
            }

        }
        
        foreach(Instruction _instruction in halfInjected.IlProcessor.Body.Instructions)
        {
            if (_instruction.Operand is MethodReference)
            {
                MethodReference method = _instruction.Operand as MethodReference;
                
                if (method.Name == readModule.TargetMethod.Name && method.DeclaringType.Name == readModule.TargetMethod.DeclaringType.Name)
                {
                    Console.WriteLine("Method already imported, skipping");
                    halfInjected.Module.Dispose();
                    readModule.Module.Dispose();
                    return new Injection();
                }

            }

        }

        MethodReference methodReference = halfInjected.Module.ImportReference(readModule.TargetMethod);

        Instruction instruction = halfInjected.IlProcessor.Create(OpCodes.Call, methodReference);

        Instruction targetInstruction;
        if (relativeToEnd)
        {
            int endIndex = halfInjected.TargetMethod.Body.Instructions.Count - 1;
            targetInstruction = halfInjected.TargetMethod.Body.Instructions[endIndex + instructionInsertOffset];
        }
        else
        {
            targetInstruction = halfInjected.TargetMethod.Body.Instructions[instructionInsertOffset];
        }
        
        if (insertAfter)
        {
            halfInjected.IlProcessor.InsertAfter(targetInstruction, instruction);
        }
        else
        {
            halfInjected.IlProcessor.InsertBefore(targetInstruction, instruction);
        }

        readModule.Module.Dispose();

        return new Injection(halfInjected.IlProcessor, halfInjected.Module, halfInjected.Type);
    }

    public static void OverrideMethod(string pathToPasteTo, string pasteClassName, string pasteMethodName, string pathToCopyFrom, string copyClassName, string copyMethodName)
    {
        HalfInjected paste = InjectionFirstStep(pathToPasteTo, pasteClassName, pasteMethodName, "paste: ");
        if (paste == null)
            return;

        HalfInjected copy = InjectionFirstStep(pathToCopyFrom, copyClassName, copyMethodName, "copy: ");
        if (copy == null)
            return;

        OverwriteMethodIL(paste.TargetMethod, copy.TargetMethod, pasteClassName, pasteMethodName, paste.Module);


        paste.Module.Write();
        paste.Module.Dispose();
        copy.Module.Dispose();

        Console.WriteLine("Overwrote method \"" + pasteClassName + "." + pasteMethodName + "\" with \"" + copyClassName + "." + copyMethodName + "\"!");
    }

    public static void AddMethodToClass(string pathToWriteTo, string classToWriteTo, string newMethodName, string pathToCopyFrom, string classToCopyFrom, string methodToCopyFrom)
    {
        HalfInjected copy = InjectionFirstStep(pathToCopyFrom, classToCopyFrom, methodToCopyFrom, " copy: ");

        if (!File.Exists(pathToWriteTo))
        {
            Console.WriteLine("paste: Could not find Assembly-CSharp.dll at path \"" + pathToWriteTo + "\"");
            return;
        }
        ModuleDefinition writeModule = ModuleDefinition.ReadModule(pathToWriteTo, new ReaderParameters()
        {
            ReadWrite = true
        });
        
        TypeDefinition typeToWriteTo = writeModule.GetType(classToWriteTo);
        if (typeToWriteTo == null)
        {
            copy.Module.Dispose();
            Console.WriteLine("Write: Could not find class \"" + classToWriteTo + "\"");
            return;
        }
        if (typeToWriteTo.Methods.Where(a => a.Name == newMethodName).Count() >= 1)
        {
            copy.Module.Dispose();
            writeModule.Dispose();
            Console.WriteLine("Method \"" + newMethodName + "\" already exists, skipping!");
            return;
        }

        TypeReference returnType = writeModule.ImportReference(copy.TargetMethod.ReturnType);

        MethodDefinition newMethod = new MethodDefinition(newMethodName, copy.TargetMethod.Attributes, returnType);

        newMethod.Parameters.Clear();
        for (int i = 0; i < copy.TargetMethod.Parameters.Count; i++)
        {
            newMethod.Parameters.Add(copy.TargetMethod.Parameters[i]);
        }

        List<AssemblyNameReference> differance = Util.GetDifferenceBetweenLists(copy.Module.AssemblyReferences.ToList(), writeModule.AssemblyReferences.ToList());
        Console.WriteLine("Adding references to assemblies:");
        for (int i = 0; i < differance.Count; i++)
        {
            Console.WriteLine("\t" + differance[i].FullName);
            copy.Module.AssemblyReferences.Add(differance[i]);
        }

        OverwriteMethodIL(newMethod, copy.TargetMethod, classToWriteTo, newMethodName, writeModule, typeToWriteTo);

        typeToWriteTo.Methods.Add(newMethod);

        writeModule.Write();
        writeModule.Dispose();
        copy.Module.Dispose();
        Console.WriteLine("Added method \"" + classToWriteTo + "." + newMethodName + "\" from \"" + classToCopyFrom + "." + methodToCopyFrom + "\"");
    }

    public static void AddFieldToClass(string pathToWriteTo, string classToWriteTo, string newFieldName, string pathToCopyFrom, string classToCopyFrom, string fieldToCopyFrom)
    {
        #region write
        if (!File.Exists(pathToWriteTo))
        {
            Console.WriteLine("Write: Could not find Assembly-CSharp.dll at path \"" + pathToWriteTo + "\"");
            return;
        }
        ModuleDefinition writeModule = ModuleDefinition.ReadModule(pathToWriteTo, new ReaderParameters()
        {
            ReadWrite = true
        });

        TypeDefinition writeType = writeModule.GetType(classToWriteTo);
        if (writeType == null)
        {
            Console.WriteLine("Write: Could not find class \"" + classToWriteTo + "\"");
            writeModule.Dispose();
            return;
        }
        #endregion
        #region copy
        if (!File.Exists(pathToCopyFrom))
        {
            Console.WriteLine("Copy: Could not find Assembly-CSharp.dll at path \"" + pathToCopyFrom + "\"");
            writeModule.Dispose();
            return;
        }
        ModuleDefinition CopyModule = ModuleDefinition.ReadModule(pathToCopyFrom, new ReaderParameters()
        {
            ReadWrite = true
        });

        TypeDefinition copyType = CopyModule.GetType(classToCopyFrom);
        if (copyType == null)
        {
            Console.WriteLine("Copy: Could not find class \"" + classToCopyFrom + "\"");
            CopyModule.Dispose();
            writeModule.Dispose();
            return;
        }
        FieldDefinition copyField = copyType.Fields.FirstOrDefault(field => field.Name == fieldToCopyFrom);
        if (copyField == null)
        {
            Console.WriteLine("Copy: Could not find field \"" + classToCopyFrom + "." + fieldToCopyFrom + "\"");
            CopyModule.Dispose();
            writeModule.Dispose();
            return;
        }
        #endregion


        TypeReference fieldType = writeModule.ImportReference(copyField.FieldType);
        FieldDefinition newField = new FieldDefinition(copyField.Name, copyField.Attributes, fieldType);



        newField.CustomAttributes.Clear();
        for (int i = 0; i < copyField.CustomAttributes.Count; i++)
        {
            MethodReference constructor = writeModule.ImportReference(copyField.CustomAttributes[i].Constructor);
            newField.CustomAttributes.Add(new CustomAttribute(constructor));
        }

        writeType.Fields.Add(newField);


        writeModule.Write();
        writeModule.Dispose();
        CopyModule.Dispose();
        Console.WriteLine("Added field \"" + classToWriteTo + "." + newFieldName + "\" from \"" + classToCopyFrom + "." + fieldToCopyFrom + "\"");
    }

    public static void AddClassToAssembly(string pathToAssembly, string newClassName, string pathToCopyAssembly, string classToCopyFullName)
    {
        if (!File.Exists(pathToAssembly))
        {
            Console.WriteLine("Write: Could not find Assembly-CSharp.dll at path \"" + pathToAssembly + "\"");
            return;
        }
        ModuleDefinition writeModule = ModuleDefinition.ReadModule(pathToAssembly, new ReaderParameters()
        {
            ReadWrite = true
        });


        if (!File.Exists(pathToCopyAssembly))
        {
            Console.WriteLine("Copy: Could not find Assembly-CSharp.dll at path \"" + pathToCopyAssembly + "\"");
            writeModule.Dispose();
            return;
        }
        ModuleDefinition copyModule = ModuleDefinition.ReadModule(pathToCopyAssembly, new ReaderParameters()
        {
            ReadWrite = true
        });
        TypeDefinition copyType = copyModule.GetType(classToCopyFullName);
        if (copyType == null)
        {
            copyModule.Dispose();
            writeModule.Dispose();

            Console.WriteLine("Copy: Could not find class \"" + classToCopyFullName + "\"");
            return;
        }
        if (writeModule.GetType(newClassName) != null)
        {
            Console.WriteLine("Module already had class \"" + newClassName + "\", skipping...");
            copyModule.Dispose();
            writeModule.Dispose();
            return;
        }


        TypeReference baseTypeReference = null;
        if (copyType.BaseType != null)
        {
            baseTypeReference = writeModule.ImportReference(copyType.BaseType);
        }

        TypeDefinition newType = new TypeDefinition("", newClassName, copyType.Attributes, baseTypeReference);

        FieldDefinition[] fields = new FieldDefinition[copyType.Fields.Count];
        copyType.Fields.CopyTo(fields, 0);

        MethodDefinition[] methods = new MethodDefinition[copyType.Methods.Count];
        copyType.Methods.CopyTo(methods, 0);

        writeModule.Types.Add(newType);

        writeModule.Write();
        writeModule.Dispose();
        copyModule.Dispose();

        Console.WriteLine("Added Class Type \"" + newClassName + "\" from \"" + classToCopyFullName + "\"");

        Console.WriteLine("Adding fields...");
        for (int i = 0; i < fields.Length; i++)
        {
            AddFieldToClass(pathToAssembly, newClassName, fields[i].Name, pathToCopyAssembly, classToCopyFullName, fields[i].Name);
        }
        Console.WriteLine("Done!");
        Console.WriteLine("Adding methods...");
        for (int i = 0; i < methods.Length; i++)
        {
            AddMethodToClass(pathToAssembly, newClassName, methods[i].Name, pathToCopyAssembly, classToCopyFullName, methods[i].Name);
        }
        Console.WriteLine("Done!");
        Console.WriteLine("Fixing method references...");

        ModuleDefinition writeSecondTimeModule = ModuleDefinition.ReadModule(pathToAssembly, new ReaderParameters()
        {
            ReadWrite = true
        });
        ModuleDefinition copySecondTimeModule = ModuleDefinition.ReadModule(pathToCopyAssembly, new ReaderParameters()
        {
            ReadWrite = true
        });

        MethodDefinition[] writeMethods = writeSecondTimeModule.GetType(newClassName).Methods.ToArray();
        MethodDefinition[] copyMethods = copySecondTimeModule.GetType(classToCopyFullName).Methods.ToArray();

        if (writeMethods.Length != copyMethods.Length)
        {
            ErrorHandler.Crash("The number of methods did not match");
            return;
        }
        for (int i = 0; i < writeMethods.Length; i++)
        {
            FixMethodReferences(writeMethods[i], copyMethods[i]);
        }

        writeSecondTimeModule.Write();
        writeSecondTimeModule.Dispose();
        copySecondTimeModule.Dispose();
        Console.WriteLine("Done!");
        Console.WriteLine("Successfully added all members of class \"" + newClassName + "\" from \"" + classToCopyFullName + "\"");
    }

    private static void OverwriteMethodIL(MethodDefinition pasteMethod, MethodDefinition copyMethod, string pasteClassName, string pasteMethodName, ModuleDefinition pasteModule, TypeDefinition newClass = null)
    {
        if (newClass == null)
        {
            newClass = pasteMethod.DeclaringType;
        }

        pasteMethod.Body.Instructions.Clear();
        for (int i = 0; i < copyMethod.Body.Instructions.Count; i++)
        {
            object reference = copyMethod.Body.Instructions[i].Operand;

            if (reference is TypeReference)
            {
                TypeReference newTypeReference = pasteModule.ImportReference(reference as TypeReference);
                Instruction instruction = pasteMethod.Body.GetILProcessor().Create(copyMethod.Body.Instructions[i].OpCode, newTypeReference);
                Console.WriteLine("Writing \"" + instruction.OpCode.ToString() + "\" to " + pasteClassName + "." + pasteMethodName);
                pasteMethod.Body.GetILProcessor().Append(instruction);
            }
            else if (reference is MethodReference)
            {
                MethodReference newMethodReference = pasteModule.ImportReference(reference as MethodReference);
                Instruction instruction = pasteMethod.Body.GetILProcessor().Create(copyMethod.Body.Instructions[i].OpCode, newMethodReference);
                Console.WriteLine("Writing \"" + instruction.OpCode.ToString() + "\" to " + pasteClassName + "." + pasteMethodName);
                pasteMethod.Body.GetILProcessor().Append(instruction);
            }
            else if (reference is FieldReference)
            {
                FieldReference newFieldReference = null;
                FieldReference fieldReference = reference as FieldReference;

                Console.WriteLine("\n" + fieldReference.FieldType.FullName + "\n");

                TypeReference fieldType = pasteModule.ImportReference(fieldReference.FieldType);

                if (fieldReference.DeclaringType == copyMethod.DeclaringType)
                {
                    List<FieldDefinition> matchingFields = newClass.Fields.Where(field => field.Name == fieldReference.Name).ToList();
                    // Gets all fields with matching names to the referenced field but on pasteMethod.DeclaringType
                    if (matchingFields.Count == 0)
                    {
                        ErrorHandler.Crash("Could not find field \"" + fieldReference.FullName + "\"");
                        return;
                    }
                    if (matchingFields.Count > 1)
                    {
                        ErrorHandler.Crash("Found too many fields, if this happens you did something very, very, very wrong.");
                        return;
                    }

                    newFieldReference = pasteModule.ImportReference(matchingFields[0]);
                    //newFieldReference = new FieldReference((reference as FieldReference).Name, fieldType, (reference as FieldReference).DeclaringType);

                }
                else
                {
                    newFieldReference = pasteModule.ImportReference(reference as FieldReference);
                    //newFieldReference = new FieldReference((reference as FieldReference).Name, fieldType, (reference as FieldReference).DeclaringType);
                }

                Instruction instruction = pasteMethod.Body.GetILProcessor().Create(copyMethod.Body.Instructions[i].OpCode, newFieldReference);
                Console.WriteLine("Writing \"" + instruction.OpCode.ToString() + "\" to " + pasteClassName + "." + pasteMethodName);
                pasteMethod.Body.GetILProcessor().Append(instruction);
            }
            else
            {
                Console.WriteLine("Writing \"" + copyMethod.Body.Instructions[i].OpCode.ToString() + "\" to " + pasteClassName + "." + pasteMethodName);
                pasteMethod.Body.Instructions.Add(copyMethod.Body.Instructions[i]);
            }
        }

    }

    private static HalfInjected InjectionFirstStep(string path, string className, string methodName, string debugPre = "", bool isGeneric = false)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine(debugPre + "Could not find Assembly-CSharp.dll at path \"" + path + "\"");
            return null;
        }
        ModuleDefinition module = ModuleDefinition.ReadModule(path, new ReaderParameters()
        {
            ReadWrite = true
        });

        TypeDefinition type = module.GetType(className);
        if (type == null)
        {
            Console.WriteLine(debugPre + "Could not find class \"" + className + "\"");
            module.Dispose();
            return null;
        }
        MethodDefinition targetMethod = type.Methods.FirstOrDefault(method => method.Name == methodName && method.ContainsGenericParameter == isGeneric);
        if (targetMethod == null)
        {
            Console.WriteLine(debugPre + "Could not find Method \"" + methodName + "\" in class \"" + className + "\"");
            module.Dispose();
            return null;
        }
        ILProcessor iLProcessor = targetMethod.Body.GetILProcessor();
        return new HalfInjected(module, targetMethod, type, iLProcessor);
    }
    private static void FixMethodReferences(MethodDefinition paste, MethodDefinition copy)
    {
        for (int i = 0; i < paste.Body.Instructions.Count; i++)
        {
            object operand = paste.Body.Instructions[i].Operand;
            if (operand is MethodReference)
            {
                MethodReference methodReference = operand as MethodReference;
                if (methodReference.DeclaringType.FullName == copy.DeclaringType.FullName)
                {
                    List<MethodDefinition> matchingMethods = paste.DeclaringType.Methods.Where(method => method.Name == methodReference.Name && method.Parameters.SequenceEqual(methodReference.Parameters)).ToList();
                    if (matchingMethods.Count == 0)
                    {
                        ErrorHandler.Crash("Could not find method \"" + methodReference.FullName + "\"");
                        return;
                    }
                    if (matchingMethods.Count > 1)
                    {
                        ErrorHandler.Crash("Found too many methods, if this happens you did something very, very, very, very, very, very, very, very, very, very, very, very, very, very, very wrong.");
                        return;
                    }

                    MethodReference matchingMethodReference = paste.Module.ImportReference(matchingMethods[0]);
                    Console.WriteLine("Overriding operand " + paste.Body.Instructions[i].Operand.ToString() + " with " + matchingMethodReference.ToString());
                    paste.Body.Instructions[i].Operand = matchingMethodReference;

                }
            }
        }

    }

    public class HalfInjected
    {
        public HalfInjected(ModuleDefinition module, MethodDefinition target, TypeDefinition type, ILProcessor iLProcessor)
        {
            Module = module;
            TargetMethod = target;
            Type = type;
            IlProcessor = iLProcessor;
        }
        public ModuleDefinition Module;
        public MethodDefinition TargetMethod;
        public TypeDefinition Type;
        public ILProcessor IlProcessor;
    }


    public static MethodBase GetMethodWithNameInType(Type type, string name, Type returnType)
    {
        List<MethodInfo> methodInfos = type.GetMethods().Where(p => p.Name == name).ToList();
        for (int i = 0; i < methodInfos.Count; i++)
        {
            if (methodInfos[i].ReturnType == returnType)
            {
                return methodInfos[i];
            }
        }

        return null;
    }
}

public class Injection
{
    public Injection(ILProcessor _processor, ModuleDefinition _module, TypeDefinition type)
    {
        Processor = _processor;
        module = _module;
        Type = type;
    }
    public Injection()
    {
        isNull = true;
    }

    #region AddInstructionToTopSaves
    public void AddInstructionOverSafe(OpCode instruction, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, byte operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, CallSite operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, double operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, FieldReference operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction, operand));
    }
    public void AddInstructionOverSafe(OpCode instruction, float operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, int operandIndex, int index = 0, bool THIS_IS_A_INSTRUCTION = true)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;
        Instruction _instruction = Processor.Body.Instructions[operandIndex - 1];
        if (_instruction == null)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction, _instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, Instruction[] operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, int operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, long operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, MethodReference operand, int index = 0)
    {
        if(isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, ParameterDefinition operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, sbyte operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, string operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, TypeReference operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionOverSafe(OpCode instruction, VariableDefinition operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertBefore(Processor.Body.Instructions[index], Processor.Create(instruction));
    }




    public void AddInstructionUnderSafe(OpCode instruction, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, byte operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, CallSite operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, double operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, FieldReference operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, float operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, int operandIndex, int index = 0, bool THIS_IS_A_INSTRUCTION = true)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Instruction _instruction = Processor.Body.Instructions[operandIndex - 1];
        if (_instruction == null)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction, _instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, Instruction[] operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, int operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, long operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, MethodReference operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, ParameterDefinition operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, sbyte operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, string operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, TypeReference operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    public void AddInstructionUnderSafe(OpCode instruction, VariableDefinition operand, int index = 0)
    {
        if (isNull)
            return;
        if (index < 0 || index >= Processor.Body.Instructions.Count)
            return;

        Processor.InsertAfter(Processor.Body.Instructions[index], Processor.Create(instruction));
    }
    #endregion

    public int GetLengthOfInstructions()
    {
        if (isNull)
            return -1;

        return Processor.Body.Instructions.Count;
    }

    public void Write()
    {
        if (isNull)
            return;

        module.Write();
        Console.WriteLine("Injected!");
        module.Dispose();
    }

    public FieldReference GetFieldReferenceOnSameType(string name)
    {
        foreach(FieldDefinition field in Type.Fields)
        {
            if (field.Name == name)
            {
                return module.ImportReference(field);
            }
        }
        return null;
    }

    private readonly bool isNull;
    private ILProcessor Processor;
    private ModuleDefinition module;
    private TypeDefinition Type;
}