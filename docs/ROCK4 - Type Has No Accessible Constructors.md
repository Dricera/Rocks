# Type Has No Mockable Members
If the given type is a class that has no accessible constructors, a mock cannot be created.
```
public class TypeToMock 
{ 
	private TypeToMock()
		: base() { }
		
	public virtual void Foo() { }
}

...

// This will generate ROCK4
var rock = Rock.Create<TypeToMock>();
```