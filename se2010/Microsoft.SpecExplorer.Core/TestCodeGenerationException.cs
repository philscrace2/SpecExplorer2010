// Decompiled with JetBrains decompiler
// Type: Microsoft.SpecExplorer.TestCodeGenerationException
// Assembly: Microsoft.SpecExplorer.Core, Version=2.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 442F5921-BF3A-42D5-916D-7CC5E2AD42CC
// Assembly location: C:\tools\Spec Explorer 2010\Microsoft.SpecExplorer.Core.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.SpecExplorer
{
  [Serializable]
  public sealed class TestCodeGenerationException : Exception
  {
    public TestCodeGenerationException()
    {
    }

    public TestCodeGenerationException(string message)
      : base(message)
    {
    }

    public TestCodeGenerationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private TestCodeGenerationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
