
using System;
using System.Text.Json.Nodes;

namespace PogSharp.Utilities;

public static class JsonExtensions
{
  public static T GetIndexValue<T>(this JsonNode? node, string associativeIndex)
  {
    T? nullableValue = default;
    bool isCaptured = node?[associativeIndex]?.AsValue()?.TryGetValue(out nullableValue) ?? false;
    return isCaptured ? nullableValue! : (T)Activator.CreateInstance(typeof(T))! ?? throw new TypeLoadException($"Unable to convert to {typeof(T)}");
  }
}