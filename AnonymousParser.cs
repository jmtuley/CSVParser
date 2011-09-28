using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CSVParser
{
	/// <summary>
	/// Proof-of-concept for duck-typed, generic, reflected object parser
	/// </summary>
	/// <typeparam name="T">Any class with an empty constructor</typeparam>
	public class AnonymousParser
	{
		/// <summary>
		/// Create an instance of T that maps the properties listed in Props to the values listed in Vals
		/// </summary>
		/// <typeparam name="T">The type of the parsed object</typeparam>
		/// <param name="Props">List of property names</param>
		/// <param name="Vals">List of property values</param>
		/// <returns>a new T with the given values</returns>
		/// <remarks>Each property type (U) must have a static U Parse(string str) method, or be string.</remarks>
		public static T ParseData<T>(List<string> Props, List<string> Vals) where T : new()
		{
			var obj = new T();

			for (int i = 0, len = Props.Count; i < len; i++)
			{
				// Get type of the property
				Type propType = obj.GetType().GetProperty(Props[i]).PropertyType;

				// If we come across a property name that isn't part of the class, just skip it.
				if (propType == null)
				{
					continue;
				}

				if (propType.Name == "String") // Strings don't need to be parsed, just collected.
				{
					MethodInfo propSetter = obj.GetType().GetProperty(Props[i]).GetSetMethod();
					propSetter.Invoke(obj, new object[] { Vals[i] });
				}
				else // Parse the string into the property type (t)
				{
					// Grab the Parse(string) method
					MethodInfo propParser = propType.GetMethod("Parse", new Type[] { typeof(string) });
					if (propParser != null) // If there *is* a Parse(string) method, use it
					{
						// Create an instance of the property's type
						var tempProp = Activator.CreateInstance(propType);
						// Invoke Parse, and drop it in x
						tempProp = propParser.Invoke(null, new object[] { Vals[i] });
						// Get the property's setter
						MethodInfo propSetter = obj.GetType().GetProperty(Props[i]).GetSetMethod();
						// Set the value
						propSetter.Invoke(obj, new object[] { tempProp });
					}
					else // If no Parse(string method) set this prop to null
					{
						// Get the setter
						MethodInfo propSetter = obj.GetType().GetProperty(Props[i]).GetSetMethod();
						// Set to null
						propSetter.Invoke(obj, new object[] { null });
					}
				}
			}
			return obj;
		}

		public static void Main()
		{
			List<string> Props = new List<string>() { "StringProp", "IntProp", "DoubleProp", "ListProp" };
			List<string> Vals = new List<string>() { "hello", "7", "15.5", "[3,5,7]" };

			TestMe tm = AnonymousParser.ParseData<TestMe>(Props, Vals);
			Console.Write("SP is {0}\nIP is {1}\nDP is {2}\nLP is {3}\n", tm.StringProp, tm.IntProp, tm.DoubleProp, tm.ListProp);
		}
	}
	// Simple test class. Should be able to parse the first three, but not the List property
	public class TestMe
	{
		public string StringProp { get; set; }
		public int IntProp { get; set; }
		public double DoubleProp { get; set; }
		public List<int> ListProp { get; set; }
	}
}
