using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightJson;

public interface ExampleNocabInterface : JsonConvertible  { }

// NOTE: all JsonConvertable functions are virtual.
public abstract class ExampleNocabAbstract : ExampleNocabInterface {

    private string abstractData;

    protected ExampleNocabAbstract(string abstractData) { this.abstractData = abstractData; }

    protected ExampleNocabAbstract(JsonObject jo) { this.abstractLoadJson(jo); }

    public virtual string myJsonType() { return "ExampleNocabAbstractType"; }

    public virtual JsonObject toJson() {
        // Take data from this abstract class and convert to JsonObject
        JsonObject result = new JsonObject();

        // Convert the rest of this class into a JO
        result["abstractData"] = abstractData;

        return result;
    }

    protected void abstractLoadJson(JsonObject jo) {
        // TODO: Validate the Json before loading

        // Take the data from the JO and load it into this abstract class
        this.abstractData = jo["abstractData"];
    }

    public abstract void loadJson(JsonObject jo);
}


public class ExampleNocabChild : ExampleNocabAbstract {

    private int data;

    public ExampleNocabChild(int data) : base("random Data") { this.data = data; }

    public ExampleNocabChild(JsonObject jo) : base(jo["Base"].AsJsonObject) {
        /*  Order of events
         * 1) An ExampleNocabInherit is created via 'new ExampleNocabInherit(jo);'
         * 2) Base class constructor is called
         * 2.1) Base constructor runs the abstractLoadJson() function
         * 3) This class loads its own data from the JO
         */

        // The base class is already loaded, so only load data for this child class
        loadJsonThis(jo);
    }


    public override string myJsonType() { return "ExampleNocabInherit"; }
    public override JsonObject toJson() {
        // Convert the base abstract class into a JO
        JsonObject result = new JsonObject();
        result["Base"] = base.toJson();

        // Convert the rest of this class into a JO
        result["data"] = data;

        return result;
    }


    private void loadJsonBase(JsonObject baseJO) { base.abstractLoadJson(baseJO); } 
    private void loadJsonThis(JsonObject jo)     {
        // Load data from the JO into this class
        this.data = jo["data"];
    }

    public override void loadJson(JsonObject jo) {
        // TODO Validate the JO before loading

        // Load the base class first
        loadJsonBase(jo["Base"]);

        // Load the rest of the data from the JO into this class
        loadJsonThis(jo);
    }
}
