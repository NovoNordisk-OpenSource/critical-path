﻿namespace NovoNordisk.CriticalPath.Exceptions;

/// <summary>
/// This exception is thrown when a cyclic dependency is detected in the activity graph. When this happens the critical path cannot be determined.
/// </summary>
public class CyclicDependencyException : Exception
{
    /// <summary>
    /// Create a new instance of <see cref="CyclicDependencyException"/>
    /// </summary>
    public CyclicDependencyException(string? message) : base(message)
    {
    }
}