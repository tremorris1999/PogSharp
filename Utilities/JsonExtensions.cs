
using System;
using System.Text.Json.Nodes;

namespace PogSharp.Utilities;

/// <summary>
/// Class containing extension methods for JsonNode objects.
/// </summary>
public static class JsonExtensions
{
  /// <summary>
  /// Retrieves the value at a given index, or the default of T.
  /// </summary>
  /// <param name="node">JsonNode to fetch a value from.</param>
  /// <param name="associativeIndex">Index to fetch at.</param>
  /// <typeparam name="T">Type of requested data.</typeparam>
  /// <returns>The (T) value of the JsonNode, or default of T.</returns>
  public static T GetIndexValue<T>(this JsonNode? node, string associativeIndex)
  {
    T? nullableValue = default;
    bool isCaptured = node?[associativeIndex]?
      .AsValue()?
      .TryGetValue(out nullableValue)
        ?? false;

    return isCaptured
      ? nullableValue!
      : (T)(Activator.CreateInstance(typeof(T))
        ?? throw new TypeLoadException($"Unable to convert to {typeof(T)}"));
  }
}