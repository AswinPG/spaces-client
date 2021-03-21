using System;

namespace SpacesClient.Core.Abstractions
{
    public interface ISpaceFile
    {
        DateTime CreatedAt { get; }
        string Key { get; }
    }
}