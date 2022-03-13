namespace NetTemplate.Misc;

using System.Collections.Generic;

/** An automatically created aggregate of properties.
 *
 *  I often have lists of things that need to be formatted, but the list
 *  items are actually pieces of data that are not already in an object.  I
 *  need ST to do something like:
 *
 *  Ter=3432
 *  Tom=32234
 *  ....
 *
 *  using template:
 *
 *  $items:{it.name$=$it.type$}$
 *
 *  This example will call getName() on the objects in items attribute, but
 *  what if they aren't objects?  I have perhaps two parallel arrays
 *  instead of a single array of objects containing two fields.  One
 *  solution is allow Maps to be handled like properties so that it.name
 *  would fail getName() but then see that it's a Map and do
 *  it.get("name") instead.
 *
 *  This very clean approach is espoused by some, but the problem is that
 *  it's a hole in my separation rules.  People can put the logic in the
 *  view because you could say: "go get bob's data" in the view:
 *
 *  Bob's Phone: $db.bob.phone$
 *
 *  A view should not be part of the program and hence should never be able
 *  to go ask for a specific person's data.
 *
 *  After much thought, I finally decided on a simple solution.  I've
 *  added setAttribute variants that pass in multiple property values,
 *  with the property names specified as part of the name using a special
 *  attribute name syntax: "name.{propName1,propName2,...}".  This
 *  object is a special kind of HashMap that hopefully prevents people
 *  from passing a subclass or other variant that they have created as
 *  it would be a loophole.  Anyway, the ASTExpr.getObjectProperty()
 *  method looks for Aggregate as a special case and does a get() instead
 *  of getPropertyName.
 */
public class Aggregate
{
    private readonly Dictionary<string, object> _properties = new Dictionary<string, object>();

    public IDictionary<string, object> Properties
    {
        get
        {
            return _properties;
        }
    }

    /** Allow StringTemplate to add values, but prevent the end
     *  user from doing so.
     */
    public object this[string propertyName]
    {
        get
        {
            return _properties[propertyName];
        }

        internal set
        {
            _properties[propertyName] = value;
        }
    }

    public bool TryGetValue(string propertyName, out object value)
    {
        return _properties.TryGetValue(propertyName, out value);
    }
}
