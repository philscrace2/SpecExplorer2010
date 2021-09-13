// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.ModelingGuidance.GuidanceException
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer.ModelingGuidance
{
  [Serializable]
  public class GuidanceException : Exception
  {
    public GuidanceException()
    {
    }

    public GuidanceException(string message)
      : base(message)
    {
    }

    public GuidanceException(string message, Exception exc)
      : base(message, exc)
    {
    }

    protected GuidanceException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
