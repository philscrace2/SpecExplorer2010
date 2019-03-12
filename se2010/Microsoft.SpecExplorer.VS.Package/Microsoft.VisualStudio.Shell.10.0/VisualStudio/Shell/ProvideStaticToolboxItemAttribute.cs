// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Shell.ProvideStaticToolboxItemAttribute
// Assembly: Microsoft.VisualStudio.Shell.10.0, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DDD5EE63-28D3-433C-9A6A-55426179C38C
// Assembly location: C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.VisualStudio.Shell.10.0\v4.0_10.0.0.0__b03f5f7f11d50a3a\Microsoft.VisualStudio.Shell.10.0.dll

using System;

namespace Microsoft.VisualStudio.Shell
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
  public sealed class ProvideStaticToolboxItemAttribute : RegistrationAttribute
  {
    public string GroupId { get; private set; }

    public string Name { get; private set; }

    public string Id { get; private set; }

    public string Formats { get; private set; }

    public string HelpKeyword { get; private set; }

    public string AssemblyName { get; private set; }

    public string TypeName { get; private set; }

    public string SupportedFrameworks { get; private set; }

    public string TargetedItemProvider { get; private set; }

    public string BitmapResourceId { get; private set; }

    public int BitmapIndex { get; set; }

    public int TransparentColor { get; private set; }

    public int Index { get; private set; }

    public string TipTitle { get; set; }

    public string TipVersion { get; set; }

    public string TipPublisher { get; set; }

    public string TipComponentType { get; set; }

    public string TipDescription { get; set; }

    public bool UseProjectTargetFrameworkVersionInTooltip { get; set; }

    public ProvideStaticToolboxItemAttribute(
      string groupId,
      string name,
      string id,
      string formats,
      string helpKeyword,
      string bitmapResourceId,
      int transparentColor)
    {
      this.GroupId = groupId;
      this.Name = name;
      this.Id = id;
      this.Formats = formats;
      this.HelpKeyword = helpKeyword;
      this.BitmapResourceId = bitmapResourceId;
      this.TransparentColor = transparentColor;
      this.BitmapIndex = -1;
      this.Index = -1;
    }

    public ProvideStaticToolboxItemAttribute(
      string groupId,
      string name,
      string id,
      string formats,
      string helpKeyword,
      string bitmapResourceId,
      int transparentColor,
      string assemblyName,
      string typeName,
      string targetedItemProvider,
      string supportedFrameworks)
      : this(groupId, name, id, formats, helpKeyword, bitmapResourceId, transparentColor)
    {
      this.AssemblyName = assemblyName;
      this.TypeName = typeName;
      this.SupportedFrameworks = supportedFrameworks;
      this.TargetedItemProvider = targetedItemProvider;
    }

    private void SetValueIfNotEmpty(RegistrationAttribute.Key key, string name, string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      key.SetValue(name, (object) value);
    }

    public override void Register(RegistrationAttribute.RegistrationContext context)
    {
      using (RegistrationAttribute.Key key = context.CreateKey(this.GetPackageRegKeyPath(context.ComponentType.GUID)))
      {
        using (RegistrationAttribute.Key subkey1 = key.CreateSubkey("Toolbox"))
        {
          using (RegistrationAttribute.Key subkey2 = subkey1.CreateSubkey("DefaultContent"))
          {
            using (RegistrationAttribute.Key subkey3 = subkey2.CreateSubkey(WeakNonCryptographicHash.RegKeyHash(this.GroupId)))
            {
              subkey3.SetValue("UniqueID", (object) this.GroupId);
              using (RegistrationAttribute.Key subkey4 = subkey3.CreateSubkey(WeakNonCryptographicHash.RegKeyHash(this.Id)))
              {
                subkey4.SetValue("UniqueID", (object) this.Id);
                subkey4.SetValue("", (object) this.Name);
                subkey4.SetValue("HelpKeyword", (object) this.HelpKeyword);
                this.SetValueIfNotEmpty(subkey4, "Formats", this.Formats);
                subkey4.SetValue("BitmapResourceID", (object) this.BitmapResourceId);
                subkey4.SetValue("TransparentColor", (object) this.TransparentColor);
                if (this.BitmapIndex >= 0)
                  subkey4.SetValue("BitmapIndex", (object) this.BitmapIndex);
                this.SetValueIfNotEmpty(subkey4, "AssemblyName", this.AssemblyName);
                this.SetValueIfNotEmpty(subkey4, "TypeName", this.TypeName);
                this.SetValueIfNotEmpty(subkey4, "TFMs", this.SupportedFrameworks);
                this.SetValueIfNotEmpty(subkey4, "TargetedItemProvider", this.TargetedItemProvider);
                this.SetValueIfNotEmpty(subkey4, "TipTitle", this.TipTitle);
                this.SetValueIfNotEmpty(subkey4, "TipVersion", this.TipVersion);
                this.SetValueIfNotEmpty(subkey4, "TipPublisher", this.TipPublisher);
                this.SetValueIfNotEmpty(subkey4, "TipType", this.TipComponentType);
                this.SetValueIfNotEmpty(subkey4, "TipDescription", this.TipDescription);
                if (this.UseProjectTargetFrameworkVersionInTooltip)
                  subkey4.SetValue("UseProjectTargetFrameworkVersionInTooltip", (object) 1);
                if (this.Index >= 0)
                  subkey4.SetValue("Index", (object) this.Index);
                context.Log.WriteLine(string.Format((IFormatProvider) Resources.Culture, Resources.Reg_NotifyStaticToolboxItem, (object) this.GroupId, (object) this.Id, (object) this.Name));
              }
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
      public const string Formats = "Formats";
      public const string Keyword = "HelpKeyword";
      public const string BitmapResourceId = "BitmapResourceID";
      public const string TransparentColor = "TransparentColor";
      public const string TipTitle = "TipTitle";
      public const string TipVersion = "TipVersion";
      public const string TipPublisher = "TipPublisher";
      public const string TipType = "TipType";
      public const string TipDescription = "TipDescription";
      public const string AssemblyName = "AssemblyName";
      public const string TypeName = "TypeName";
      public const string TFMs = "TFMs";
      public const string TargetedItemProvider = "TargetedItemProvider";
      public const string BitmapIndex = "BitmapIndex";
      public const string Index = "Index";
      public const string UseProjectTargetFrameworkVersionInTooltip = "UseProjectTargetFrameworkVersionInTooltip";
    }
  }
}
