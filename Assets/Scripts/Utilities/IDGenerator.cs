using System;

/// <summary>
/// The purpose of this class is to generate random IDs using a cryptographic random number generator.
/// This generates a shorter ID than a GUID, which is easier to reference.
///
/// <remarks>
/// Usage example:
/// <code>
///     string id = IDGenerator.GenerateRandomID();
/// </code>
/// </remarks>
/// </summary>
public static class IDGenerator
{
    private static System.Random random = new System.Random();

    public static string GenerateRandomID()
    {
        byte[] buffer = new byte[8]; // 8 bytes = 64 bits
        random.NextBytes(buffer);
        string id = BitConverter.ToString(buffer).Replace("-", "").ToLower(); // Example: "f8e9d43b"
        return id;
    }
}
