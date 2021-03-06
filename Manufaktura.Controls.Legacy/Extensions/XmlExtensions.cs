﻿using Manufaktura.Music.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Manufaktura.Controls.Extensions
{
	public static class XmlExtensions
	{
		public static T? ParseAttribute<T>(this XElement element, string name) where T : struct
		{
			var childElement = element.Attributes().FirstOrDefault(d => d.Name == name);
			if (childElement == null) return null;
			return ParseValue<T>(childElement.Value);
		}

		public static string ParseAttribute(this XElement element, string name)
		{
			var childElement = element.Attributes().FirstOrDefault(d => d.Name == name);
			if (childElement == null) return null;
			return childElement.Value;
		}

		public static string ParseChildElement(this XElement element, string name)
		{
			var childElement = element.Descendants().FirstOrDefault(d => d.Name == name);
			if (childElement == null) return null;
			return childElement.Value;
		}

		public static T? ParseChildElement<T>(this XElement element, string name) where T : struct
		{
			var childElement = element.Descendants().FirstOrDefault(d => d.Name == name);
			if (childElement == null) return null;
			return ParseValue<T>(childElement.Value);
		}

		public static IEnumerable<T?> ParseChildElements<T>(this XElement element, string name) where T : struct
		{
			var childElements = element.Descendants().Where(d => d.Name == name);
			foreach (var el in childElements) yield return ParseValue<T>(el.Value);
		}

		public static IEnumerable<string> ParseChildElements(this XElement element, string name)
		{
			var childElements = element.Descendants().Where(d => d.Name == name);
			foreach (var el in childElements) yield return el.Value;
		}

		public static void ParseNodeWithDictionaryValue<T>(this XElement element, Action<T> useParseResultAction, string name, params dynamic[] valueFactory)
		{
			var childElement = element.Descendants().FirstOrDefault(d => d.Name == name);
			if (childElement == null) return;
			var matchedValue = valueFactory.FirstOrDefault(f => f.Key == childElement.Value);
			if (matchedValue != null) useParseResultAction(matchedValue.Value);
		}

		public static void ParseNodeWithDictionaryValue<T>(this XElement element, Action<T> useParseResultAction, string name, Dictionary<string, T> values)
		{
			var childElement = element.Descendants().FirstOrDefault(d => d.Name == name);
			if (childElement == null) return;
			if (values.ContainsKey(childElement.Value)) useParseResultAction(values[childElement.Value]);
		}

		private static T? ParseValue<T>(string value) where T : struct
		{
			if (typeof(T) == typeof(int)) return UsefulMath.TryParseInt(value) as T?;
			if (typeof(T) == typeof(double)) return UsefulMath.TryParse(value) as T?;
			if (typeof(T) == typeof(DateTime)) return UsefulMath.TryParseDateTime(value) as T?;
			throw new NotImplementedException("Type not supported");
		}
	}
}