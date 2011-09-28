using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSVParser
{
	class Parser
	{
		public static void Main()
		{
			test3();
		}

		#region Tests

		// Basic test of Readline()
		public static void test1()
		{
			string testInput = "a, b, \"cdef, 1\", 2, 345";
			var L = ReadLine(testInput, true);
			for (int i = 0, l = L.Count; i < l; i++)
			{
				Console.WriteLine(String.Format("{0}-th part is {1}", i, L[i]));
			}
		}

		// Basic test of ReadStream()
		public static void test2()
		{
			string testInput2 = "a, b, c\nd, e, f\n\"g, h, i\", j, k";
			var L2 = ReadStream(new StringReader(testInput2), true);
			foreach (var ell in L2)
			{
				foreach (var w in ell)
				{
					Console.WriteLine(String.Format("found {0}", w));
				}
				Console.WriteLine("End of line.");
			}
		}

		class TestClass
		{
			public string Symbol { get; set; }
			public double Price { get; set; }
			public DateTime SaleDate { get; set; }
		}

		public static void test3()
		{
			string input = "Symbol,Price,SaleDate\nMSFT,35.00,2011-08-03 08:45:30\nGOOG,1035.73,2011-08-02 17:30:35";

			var inputs = ReadStream<TestClass>(new StringReader(input), true);

			foreach (var test in inputs)
			{
				Console.WriteLine("Symbol: {0} sold for ${1} on {2} ({3} UTC)", test.Symbol, test.Price, test.SaleDate.ToLongDateString() + " at " + test.SaleDate.ToLongTimeString(), test.SaleDate.ToUniversalTime());
			}
		}

		#endregion

		protected static List<string> ReadLine(string input, bool trim)
		{
			int startIndex = 0;
			int stopIndex = 0;
			int length = input.Length;

			List<string> R = new List<string>();

			while (startIndex < length)
			{
				if (trim && (input[startIndex] == ' ' || input[startIndex] == '\t'))
				{
					startIndex++;
					continue;
				}

				if (input[startIndex] == ',')
				{
					R.Add("");
					startIndex++;
					continue;
				}

				if (input[startIndex] == '"')
				{
					stopIndex = input.IndexOf('"', startIndex + 1);

					bool done = (input[stopIndex - 1] != '\\');
					while (!done)
					{
						stopIndex = input.IndexOf('"', stopIndex + 1);
						done = (input[stopIndex - 1] != '\\');
					}

					var word = input.Substring(startIndex + 1, stopIndex - startIndex - 1);
						
					R.Add(word);
					startIndex = stopIndex + 2; // stopIndex is a quote character
				}
				else
				{
					stopIndex = input.IndexOf(',', startIndex);
					stopIndex = (stopIndex == -1) ? length : stopIndex;
					string word = input.Substring(startIndex, stopIndex - startIndex);
					word = trim ? word.Trim() : word;
					R.Add(word);
					startIndex = stopIndex + 1;
				}
			}

			return R;
		}

		protected static T ReadLine<T>(List<string> names, string input, bool trim) where T : new()
		{
			return CSVParser.AnonymousParser.ParseData<T>(names, ReadLine(input, trim));
		}

		protected static List<List<string>> ReadStream(TextReader input, bool trim)
		{
			List<List<string>> R = new List<List<string>>();
			string line;
			while ((line = input.ReadLine()) != null)
			{
				R.Add(ReadLine(line, trim));
			}
			return R;
		}

		protected static List<T> ReadStream<T>(TextReader input, bool trim) where T : new()
		{
			List<T> R = new List<T>();
			string line;
			List<string> headers = ReadLine(input.ReadLine(), true);
			while ((line = input.ReadLine()) != null)
			{
				R.Add(ReadLine<T>(headers, line, true));
			}

			return R;
		}
	}
}
