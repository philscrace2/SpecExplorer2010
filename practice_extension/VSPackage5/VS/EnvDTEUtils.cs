// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.EnvDTEUtils
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.SpecExplorer.VS
{
  internal static class EnvDTEUtils
  {
    public static IEnumerable<CodeImport> RetrieveNamespaceImports(
      CodeClass codeClass)
    {
      IEnumerable<CodeImport> first = codeClass.ProjectItem.FileCodeModel.CodeElements.OfType<CodeImport>();
      for (CodeNamespace parent = codeClass.Namespace; parent != null; parent = parent.Parent as CodeNamespace)
        first.Union<CodeImport>(parent.Members.OfType<CodeImport>());
      return first;
    }

    public static string ShortenTypeName(
      string typeName,
      IEnumerable<string> importedNamespaces,
      Project containerProject)
    {
      if (typeName == null || !typeName.Contains<char>('.'))
        return typeName;
      CodeType codeType = containerProject.CodeModel.CodeTypeFromFullName(typeName);
      if (codeType == null)
        return typeName;
      string typeNameV2 = importedNamespaces.Contains<string>(codeType.Namespace.FullName) ? typeName.Substring(codeType.Namespace.FullName.Length + 1) : typeName;
      string source1 = importedNamespaces.Count<string>((Func<string, bool>) (ns => containerProject.CodeModel.CodeTypeFromFullName(ns + "." + typeNameV2) != null)) != 1 ? typeName : typeNameV2;
      if (!source1.Contains<char>('<'))
        return source1;
      int length = source1.IndexOf('<');
      int num = source1.LastIndexOf('>');
      IEnumerable<string> source2 = EnvDTEUtils.SplitIntoTypes(source1.Substring(length + 1, num - length - 1)).Select<string, string>((Func<string, string>) (name => EnvDTEUtils.ShortenTypeName(name, importedNamespaces, containerProject)));
      return string.Format("{0}<{1}>", (object) source1.Substring(0, length), (object) source2.Aggregate<string>((Func<string, string, string>) ((combined, next) => combined + ", " + next)));
    }

    private static IEnumerable<string> SplitIntoTypes(string typeString)
    {
      int start = 0;
      int bracketBalance = 0;
      for (int i = 0; i < typeString.Length; ++i)
      {
        switch (typeString[i])
        {
          case ' ':
          case ',':
            if (bracketBalance == 0)
            {
              string type = typeString.Substring(start, i - start);
              start = i + 1;
              if (!string.IsNullOrEmpty(type))
              {
                yield return type;
                break;
              }
              break;
            }
            break;
          case '<':
            ++bracketBalance;
            break;
          case '>':
            --bracketBalance;
            break;
        }
      }
      string remaining = typeString.Substring(start, typeString.Length - start);
      if (!string.IsNullOrEmpty(remaining))
        yield return remaining;
    }
  }
}
