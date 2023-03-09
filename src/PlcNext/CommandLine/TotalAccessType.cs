#region Copyright
///////////////////////////////////////////////////////////////////////////////
//
//  Copyright (c) Phoenix Contact GmbH & Co KG
//  This software is licensed under Apache-2.0
//
///////////////////////////////////////////////////////////////////////////////
#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace PlcNext.CommandLine;

public readonly struct TotalAccessType : IEquatable<TotalAccessType>
{
    public TotalAccessType(Type type)
    {
        Type = type;
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    public Type Type { get; }

    public bool Equals(TotalAccessType other)
    {
        return Type == other.Type;
    }

    public override bool Equals(object obj)
    {
        return obj is TotalAccessType other && Equals(other);
    }

    public override int GetHashCode()
    {
        return (Type != null ? Type.GetHashCode() : 0);
    }

    public static bool operator ==(TotalAccessType left, TotalAccessType right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TotalAccessType left, TotalAccessType right)
    {
        return !left.Equals(right);
    }
}