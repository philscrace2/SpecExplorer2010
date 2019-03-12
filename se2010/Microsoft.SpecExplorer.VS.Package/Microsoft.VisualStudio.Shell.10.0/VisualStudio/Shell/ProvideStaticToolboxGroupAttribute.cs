// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideStaticToolboxGroupAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideStaticToolboxGroupAttribute : RegistrationAttribute
  {
    public string Name { get; private set; }

    public string Id { get; private set; }

    public int Index { get; private set; }

    public ProvideStaticToolboxGroupAttribute(string name, string id)
    {
      this.Name = name;
      this.Id = id;
      this.Index = -1;
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      using (RegistrationAttribute.Key key = context.CreateKey(this.GetPackageRegKeyPath(context.ComponentType.GUID)))
      {
        using (RegistrationAttribute.Key subkey1 = key.CreateSubkey("Toolbox"))
        {
          using (RegistrationAttribute.Key subkey2 = subkey1.CreateSubkey("DefaultContent"))
          {
            using (RegistrationAttribute.Key subkey3 = subkey2.CreateSubkey(WeakNonCryptographicHash.RegKeyHash(this.Id)))
            {
              subkey3.SetValue("", (object) this.Name);
              subkey3.SetValue("UniqueID", (object) this.Id);
              if (this.Index >= 0)
                subkey3.SetValue("Index", (object) this.Index);
              context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyStaticToolboxGroup, (object) this.Id, (object) this.Name));
            }
          }
        }
      }
    }

    public override void Unregister(RegistrationAttribute.RegistrationContext context)
    {
      context.RemoveKey(this.GetPackageRegKeyPath(context.ComponentType.GUID));
    }

    private static class RegistryStrings
    {
      public const string Name = "";
      public const string Id = "UniqueID";
      public const string Index = "Index";
    }
  }
}
