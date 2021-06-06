using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightJson;

public static class JsonUtilitiesNocab {

    public static JsonObject initJson(string jsonType) {
        JsonObject result = new JsonObject();
        result["Type"] = jsonType;
        return result;
    }


    private static bool validateTypeKey(JsonObject jo) { return jo.ContainsKey("Type"); }
    private static bool validateExpectedType(JsonObject jo, string expectedType) { return jo["Type"] == expectedType; }

    public static bool validateJson(JsonObject jo, string expectedType) {
        return validateTypeKey(jo) && validateExpectedType(jo, expectedType);
    }

    public static void assertValidJson(JsonObject jo, string expectedType) {
        if (!validateTypeKey(jo)) {
            throw new InvalidLoadType("Invalid JsonObject. The provided JsonObject does NOT contain the key \"Type\" (capitilization is important).");
        }
        if (!validateExpectedType(jo, expectedType)) {
            throw new InvalidLoadType($"Invalid JsonObject. The provided JsonObject does NOT have the expected type. Expected: \"{expectedType}\" !=  Actual: \"{jo["Type"]}\".");
        }
    }

    public static string printJson(JsonObject jo, bool prettyJson = false) {
        string result = jo.ToString(prettyJson);
        Debug.Log(result);
        return result;
    }

    public static string printErrorJson(JsonObject jo, bool prettyJson = false) {
        string result = jo.ToString(prettyJson);
        Debug.LogError(result);
        return result;
    }

}
