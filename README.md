# SharpINI - A simple INI Reader/Writer for C#

SharpINI is a simple .NET Standard 2.0 compatible library to parse and write INI strings.

INIs are somehow a problematic format, since there is no global standard like for JSON or XML. There are so many implementations that parse the files using different rules.
This library tries to provide an interface with parsing options to parse some of the INI files out there without creating too much complexity.

## Flexibility

This library allows the following things to be defined while parsing:
- Space chars
- Line breaks (limited)
- Comment chars (only at the beginning of a line)
- Behaviour when there are multiple sections with the same name
- Behaviour when there are multiple keys in a section with the same name
- Behaviour when there are strings outside of a section (when no section has been started)
- Removing spaces before and after value

## General parsing rules
These rules are hard and cannot be changed using parsing options

- Any space at the start of a line is removed
- Any space between the key of a key-value-pair and the `=` is removed
- Section titles must not contain closing brackets (`]`) in their name
- Any line must be a section title, a key-value pair, a comment line or an empty line (an empty line is just filled with space chars)

### Spaces
Whenever "Space" is used in this readme, it means every char that was defined as `spaceChars` in the parse options.

By default they are:
- `0x20` Space
- `0x09` Tab
- `0xA0` Non-breaking space (NBSP)


## Reading
SharpINI parses the string into the following format:
- Each Section is represented as `IDictionary<string, string>`
- The INI file is represented as `IDictionary<string, IDictionary<string, string>>`

See Types-section for more information

### Basic reading
```csharp
using SharpINI;

var myINIString = @"[MySection]
Key1=val1
Key2=val2

[MySection2]
Key1=val3
Key2=val4";

var parsed = INIReader.ReadINI(myINIString);
/*parsed:
[
	[MySection,
		[Key1, val1]
		[Key2, val2]
	],
	[MySection2,
		[Key1, val3]
		[Key2, val4]
	]
]
*/

var key = parsed["MySection"]["Key1"]; // => "val1"
```

### Reading with options
```csharp
using SharpINI;

var myINIString = @"[MySection]
Key1=val1
Key2=val2

[MySection]
Key1=val3
Key3=val4";


var options = new ParseOptions(multiSectionMode: MultiSectionMode.MERGE,
                               multiKeyMode: MultiKeyMode.OVERRIDE);

var parsed = INIReader.ReadINI(myINIString, options);
/*parsed:
[
	[MySection,
		[Key1, val3]
		[Key2, val2]
		[Key3, val4]
	]
]
*/
```

## Parse options
(The Markdown renderer on NuGet doesn't support tables. Please read the docu [on Github](https://github.com/JaegerMa/SharpINI/blob/master/README.md)).
|Name						|Type				|Default|Description|
|---						|---				|---|---|
|lineStartCommentChars		|`char[]`			|`;` `#`|If a line starts with one of these chars it's treated as comment and completely ignored|
|spaceChars					|`char[]`			|<ul><li>0x20</li><li>0x09 (`\t`)</li><li>0xA0 (NBSP)</li></ul>|Defines which chars are treated as space|
|lineBreaks					|`string[]`			|`\n` `\r\n`|Defines which strings are treated as line breaks.<br />**Note**: Due to the mechanism of reading the files, the lineBreaks-option is implemented using a simple `.Replace(lineBreak, "\n")`.<br />**Therefore** `\n` **will always be treated as line break**|
|trimSpaceBeforeValue		|`bool`				|`true`|If true, any space chars directly after the `=` of a key value pair are trimmed|
|trimSpaceAfterValue		|`bool`				|`true`|If true, any space chars at the end of a value are trimmed|
|multiKeyMode				|`MultiKeyMode`		|`DISALLOW`|Defines the behaviour if there are multiple keys with the same name in a section.<ul><li>`DISALLOW`: Throws an exception</li><li>`OVERRIDE`: Replaces the existing value</li><li>`IGNORE`: Discards the current/new value</li></ul>|
|multiSectionMode			|`MultiSectionMode`	|`DISALLOW`|Defines the behaviour if there are multiple sections with the same name<ul><li>`DISALLOW`: Throws an exception</li><li>`OVERRIDE`: Discards the old section and replaces it with a new one</li><li>`MERGE`: Treat the keys of the sections as if they were in one section. The behaviour if there are the same key names used is defined using `multiKeyMode`</li><li>`IGNORE`: Discards the current/new section</li></ul>|
|initialSectionName			|`string`			|empty string|If there are keys at beginning of the file where no section has been started/opened yet, these keys are stored in a section with this name. If this option is set to `null`, keys outside of sections are disallowed and an exception is thrown if there are any|


## Writing
When writing INI files, SharpINI accepts the same object type like it produces while reading: `IDictionary<string, IDictionary<string, string>>`

### Basic writing
```csharp
using SharpINI;

var iniString = INIWriter.WriteINI(parsed);
/*iniString: @"[MySection]
Key1 = val1
Key2 = val2

[MySection2]
Key1 = val3
Key2 = val4"
*/
```

As you see, there are now spaces before and after the `=`. You can define such render/write options using `RenderOptions` which are passed to `WriteINI`.

### Writing with options
```csharp
using SharpINI;

var options = new RenderOptions(spaceAfterKey: false,
                                spaceBeforeValue: false,
                                linesBetweenSections: 0);

var iniString = INIWriter.WriteINI(parsed, options);
/*iniString: @"[MySection]
Key1=val1
Key2=val2
[MySection2]
Key1=val3
Key2=val4"
*/
```


## RenderOptions
(The Markdown renderer on NuGet doesn't support tables. Please read the docu [on Github](https://github.com/JaegerMa/SharpINI/blob/master/README.md)).
|Name						|Type				|Default|Description|
|---						|---				|---|---|
|lineBreak					|string				|`\n`| The string used as line break|
|space						|string				|0x20| The string used as space|
|spaceAfterKey				|bool				|`true`| Whether to insert a space before the `=` char in key-value pairs|
|spaceBeforeValue			|bool				|`true`| Whether to insert a space after the `=` char in key-value pairs|
|linesBetweenSections		|int				|1|Number of blank lines to be inserted between the last key of a section and the next section|
|initialSectionName			|string				|empty string| Defines which section keys should be written at the start of the file without section-header. If it's set to `null` or if there is no section with this name, no keys are written without section|

### Convert parse options to render options
You can generate a `RenderOptions` object from your `ParseOptions` by calling the method `parseOptions.ToRenderOptions()`. This will convert your parse options to render options.

Following values are converted:
- `lineBreak` (first line break string)
- `space` (first space char)
- `spaceAfterKey`, based on `trimSpaceBeforeValue`
- `spaceBeforeValue`, based on `trimSpaceBeforeValue`
- `initialSelectionName`

## Types
When reading with `INIReader.ReadINI`, an object of type `INIFile` is returned. This type implements `IDictionary<string, IDictionary<string, string>>` but replaces the access through the index (`file["myKey"]`). You can use it like any normal `IDictionary`, but if you try to access a key which doesn't exists, `null` is returned instead of an exception being thrown. Additionally, instead of an `IDictionary<string, string>`, an `INISection` is returned which again inherits `IDictionary<string, string>`. This object has the same behaviour like `INIFile` and returns `null` if the given key doesn't exist.

**But**: This feature of `INIFile` and `INISection` can only be used if _you_ treat the object as an `INIFile` or `INISection`. As soon as your variable has the `IDictionary`-type instead of `INIFile` or `INISection`, the normal `Dictionary`-code is executed. So if you want to use this `null`-feature, always store your objects using `var`, `INIFile` or `INISection`.

```csharp
var file = INIReader.ReadINI(someINIString);
//Type of file: INIFile

var val = file["someNonExistentKey"];
// => val == null

Dictionary<string, IDictionary<string, string>> dicFile = file;
//Upcasting to Dictionary-type

var dicVal = dicFile["someNonExistentKey"];
//An exception is thrown
```

If you're accessing a section through `INIFile` which has been manually added by your code which is **not** a `INISection` but a normal `IDictionary<string, string>`, `null` is returned as the cast to `INISection` failed. So, if you're adding new sections, either use `new INISection()` or access it through the `IDictionary`-type instead of the `INIFile`-type.


```csharp
var file = INIReader.ReadINI(someINIString);

file["newSection"] = new INISection();
var section = file["newSection"];
//section: INISection


IDictionary<string, IDictionary<string, string>> dicFile = file;

dicFile["newSection"] = new Dictionary<string, string>();
section = file["newSection"];
//section: null
```

## Using internal methods
Beside `ReadINI`, `INIReader` contains methods like `ReadSectionTitle` and `ReadKeyValue`. These methods are used internally. If you want to use these methods for whatever reason, you have to tell SharpINI to perform additional checks in these methods, as by default they omit checks done by the SharpINI-caller method. To announce the usage of these methods, compile the library with the `SHARE_INTERNAL_METHODS` flag (obviously not possible when using the precompiled NuGet package).

To be more performant, the reading is done by simply moving a cursor over the string, so that a new substring does not have to be created for each element. This string-shifting is done by the internal class `StringView`. This class is also only shown if you activate the compiliation flag `SHARE_INTERNAL_METHODS`. So for using the internal methods, you have to create a `StringView` instance.


## License
SharpINI is licensed under the MIT License
