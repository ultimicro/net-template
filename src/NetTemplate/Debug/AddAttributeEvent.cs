namespace NetTemplate.Debug;

public class AddAttributeEvent : ConstructionEvent
{
    private readonly string name;
    private readonly object value; // unused really; leave for future

    public AddAttributeEvent(string name, object value)
    {
        this.name = name;
        this.value = value;
    }

    public string Name
    {
        get
        {
            return name;
        }
    }

    public object Value
    {
        get
        {
            return value;
        }
    }

    public override string ToString()
    {
        return "addEvent{" +
            ", name='" + name + '\'' +
            ", value=" + value +
                '}';
    }
}
