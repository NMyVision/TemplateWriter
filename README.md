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
// Given:
var dt = new DateTime(1980, 4, 6, 6, 30, 33);
var tw = new TemplateWriter(dt);

tw.Transform("{Current}"); // 4/6/1980 6:30:33 AM
tw.Transform("{Current_Date}"); // 19800406
tw.Transform("{Current_DateTime}"); // 19800406063033
tw.Transform("{Current_Time}"); // 063033
```

| Name | Format | Ouput |
|------|--------| --- |
|Current            | N/A               | 4/6/1980 6:30:33 AM |
|Current_Date       | yyyyMMdd          | 19800406 |
|Current_DateTime   | yyyyMMddHHmmss    | 19800406063033|
|Current_Time       |HHmmss|063033      |
|Current_UniqueDate |yyyyMMddHHmmssfff  |19800406063033000|
