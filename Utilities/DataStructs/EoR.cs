public class EoR<TError, TResult>
{
    public TError errorObject;
    public TResult resultObject;

    protected bool successful_ = false;
    public bool successful
    {
        get { return successful_; }
    }

    public EoR(TError error)
    {
        successful_ = false;
        errorObject = error;
    }

    public EoR(TResult result)
    {
        successful_ = true;
        resultObject = result;
    }
}

public class Nothing { }
