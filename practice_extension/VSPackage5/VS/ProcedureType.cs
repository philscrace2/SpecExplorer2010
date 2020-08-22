// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.VS.ProcedureType
// Assembly: Microsoft.SpecExplorer.VS.Package, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 04778F4E-8525-4D68-B061-08FAB43841FA
// Assembly location: C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\Extensions\Microsoft\Spec Explorer 2010\Microsoft.SpecExplorer.VS.Package.dll

using Microsoft.Xrt;

namespace Microsoft.SpecExplorer.VS
{
  public class ProcedureType
  {
    public string FullName { get; private set; }

    public string ShortName { get; private set; }

    public bool IsAdapter { get; private set; }

    public ProcedureType(string fullName, string shortName, bool isAdapter)
    {
      this.FullName = fullName;
      this.ShortName = shortName;
      this.IsAdapter = isAdapter;
    }

    public ProcedureType(string fullName, string shortName)
      : this(fullName, shortName, false)
    {
      this.FullName = fullName;
      this.ShortName = shortName;
    }

    public ProcedureType(IType type)
      : this(type.IsAddressType || type.IsArrayType ? ((IMember) type.ElementType).FullName : ((IMember) type).FullName, ((IMember) type).ShortName, type.IsAdapter)
    {
    }

    public override int GetHashCode()
    {
      Hash32Builder hash32Builder = new Hash32Builder();
      ((Hash32Builder) hash32Builder).Add(typeof (ProcedureType).GetHashCode());
      ((Hash32Builder) hash32Builder).Add(this.FullName.GetHashCode());
      ((Hash32Builder) hash32Builder).Add(this.ShortName.GetHashCode());
      ((Hash32Builder) hash32Builder).Add(this.IsAdapter.GetHashCode());
      return ((Hash32Builder) hash32Builder).Result;
    }

    public override bool Equals(object obj)
    {
      if (!(obj is ProcedureType))
        return false;
      ProcedureType procedureType = (ProcedureType) obj;
      if (this.FullName == procedureType.FullName && this.ShortName == procedureType.ShortName)
        return this.IsAdapter == procedureType.IsAdapter;
      return false;
    }
  }
}
