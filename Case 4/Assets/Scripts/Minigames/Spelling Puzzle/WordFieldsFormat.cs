using System.Collections.Generic;
using System;

[Serializable]
public class WordFieldsFormat
{
    public string Format;
    public List<WordField> FieldsOfWords = new List<WordField>();
}
