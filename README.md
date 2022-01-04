
# TemplateWriter

Take a string with placeholders and replace with values or by an object.

![NuGet](https://img.shields.io/nuget/v/NMyVision.TemplateWriter.svg?style=flat-square&logo=nuget)

``` cs
Install-Package  NMyVision.TemplateWriter
```

## Example

``` cs
var tw = CreateTemplateWriter();
tw.Add("GroupKey", 1221);
tw.Add("CompanyKey", 101);

var tmp = "{GroupKey}_{CompanyKey}_{missing}";

tw.Transform(tmp); // outputs: 1221_101_{missing}
```


``` cs
var o = new
{
  GroupKey = 1221,
  CompanyKey = 100
};

var x = TemplateWriter.Transform(tmp, o);

console.log(x); // outputs: 1221_101_{missing}
```

## Built In variables


``` cs
var dt = new DateTime(1980, 4, 6, 6, 30, 33);
var tw = new TemplateWriter(dt);

tw.Transform("{Current}"); // 4/6/1980 6:30:33 AM
tw.Transform("{Current_Date}"); // 19800406
tw.Transform("{Current_DateTime}"); // 19800406063033 (note: 24hr format)
tw.Transform("{Current_Time}"); // 063033
```

| Name               | Format            | Ouput                 |
|--------------------|-------------------| --------------------- |
|Current             | N/A               | 4/6/1980 6:30:33 AM   |
|Current_Date        | yyyyMMdd          | 19800406              |
|Current_DateTime    | yyyyMMddHHmmss    | 19800406063033        |
|Current_Time        | HHmmss|063033     |                       |
|Current_UniqueDate  | yyyyMMddHHmmssfff | 19800406063033000     |
|Index *             | N/A               | 0 ... N+1             | 
|UUID *              | GUID / UUID       |                       | 

Note:
  - **Index** default starts at 0 and increments by 1, this can be altered
  - **UUID** changes per transform call (v2.1)

## Create from FileInfo to add predifined variables

```
var tw = TemplateWriter.Create(new FileInfo("C:\temp\Sample.txt"));
```
| Name      | Ouput                   |
|-----------|-------------------------|
| Name      | Sample                  |
| Filename  | Sample.txt              |
| Extension | .txt                    |
| Fullname  | C:\temp\Sample.txt      |
| Directory | C:\temp                 |
| Created   | 4/25/2020 12:01:33 PM   |
| Modified  | 4/25/2020 3:02:59 PM    |


# Examples

Look in the test folder for more examples.

# Release Log
2.1.2
- Allow for spaces in placeholder names

2.1.1
- Clear method resets the `Index` variable
- Add CurrentIndex property to expose `Index` variable

2.1.0
- Increment variable renamed to IncrementValue (variable is still Index)   
- Add Increment method to manually increment the `{Index}`
- Add AutoIncrement variable to turn off/of increment functionality
- Add new `UUID` variable

2.0.2
- Add clear method

2.0.1
- Fix load to override an existing value if exists

2.0.0
- `null` can not be passed in the constructor anymore use TemplateWriter.Empty
- Removed Add(object) now use Load(object)
- Load(object) can accept an class object, Anonymous type, IDictionary or KeyValuePair 

1.2.0 
- Add Index variable for use when used in loops
- Suppress internal object Extension
- Expose GlobalFileVariables
