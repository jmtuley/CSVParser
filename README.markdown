# CSVParser

A CSV parsing tool written in C#, primarily for my own edification but also with some utility. I intend to continue learning about .NET metaprogramming and improve this code, in looks, features, and performance.

## What it does

Unlike many CSV parsers that read in a List of List<string>, or a DataTable object, this one uses reflection to produce a List of row-model objects. It also implements a weak form of duck-typing, since as it fills a row-model it looks for properties of a certain name and expects their type to have a `Parse` method -- but neither makes nor needs any other assumptions on their type.

## Usage

### Row model

A "row model" is simply a class with publicly-settable attributes that match the names of the column headers in the input CSV file. These should be typed *usefully,* so don't make them all strings. However, each of the types must posess a `public static Parse(string)` method. Examples of types that natively include such a method are `double`, `int`, and `DateTime`.

### Invocation

Oh, you already have a data model to represent your rows? Super. Now a line like

	var inputs = ReadStream<TestClass>(inputStream, true);

will read from `inputStream`, making `inputs` a `List<TestClass>`. The secon parameter (`true`) intsructs the parser to trim the excess whitespace around each comma-separated entity. (You don't want to try `false`; it's pretty broken.)

### Input

Your input file must have column names on the first row. They need to match -- exactly -- the names of the properties in your row-model class.